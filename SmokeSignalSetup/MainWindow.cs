// Copyright 2012 Peter Potrebic.  All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Net;
using System.Configuration;
using System.Net.Configuration;
using System.IO;
using NotifierLibrary;
using System.Diagnostics;
using System.Net.Mail;
using System.ServiceProcess;
using System.Threading;
using System.Security.Permissions;
using System.Security.Principal;


namespace SmokeSignalSetup
{
    public partial class MainWindow : Form
    {
        #region initilization

        public MainWindow()
        {
            InitializeComponent();

            // Load SmokeSignal config
            LoadSmokeSignalConfig();

            // Load mail config from the *.config file of the Smoke Signal Service
            LoadMailSettings();
        }

        private Configuration config;
        private MailSettingsSectionGroup initialMailSettings;
        private string fullPathTmpConfig;
        private bool configurationDirty = false;
        private bool serviceConfigured = false;
        private bool StartingUp = true;

        private int InvalidControls = 0;

        private void InitSSConfig()
        {
            string myConfigPath = Assembly.GetExecutingAssembly().Location;
            string myDir = Path.GetDirectoryName(myConfigPath);
            string serviceExePath = Path.Combine(myDir, "SmokeSignalSrvc.exe");

            config = ConfigurationManager.OpenExeConfiguration(serviceExePath);
        }

        private void LoadMailSettings()
        {
            InitSSConfig();

            string tmpConfigLeaf = Path.GetFileName(config.FilePath);
            string tmpConfigDir = Path.GetTempPath();
            fullPathTmpConfig = Path.Combine(tmpConfigDir, tmpConfigLeaf);

            initialMailSettings = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");
            FromTextBox.Text = initialMailSettings.Smtp.From;
            HostnameTextBox.Text = initialMailSettings.Smtp.Network.Host;
            PortTextBox.Text = initialMailSettings.Smtp.Network.Port.ToString();
            UsernameTextBox.Text = initialMailSettings.Smtp.Network.UserName;
            PasswordTextBox.Text = initialMailSettings.Smtp.Network.Password;
            DefaultCredsCheckBox.Checked = initialMailSettings.Smtp.Network.DefaultCredentials;
        }

        /// <summary>
        /// Init SmokeSignal config to determine any "current" settings
        /// </summary>
        private void LoadSmokeSignalConfig()
        {
            string path = Utils.DataLocation;
            SmokeSignalConfig.Initialize(path);

            CampfireNameTextBox.Text = SmokeSignalConfig.Instance.CampfireName;
            CampfireTokenTextBox.Text = SmokeSignalConfig.Instance.CampfireToken;
            DefaultDelayTextBox.Text = SmokeSignalConfig.Instance.DelayBeforeSmokeSignalInMinutes.ToString();
            SendWelcomeEmailCheckBox.Checked = SmokeSignalConfig.Instance.SendNewUserEmail;
            ExtraEmailMessage.Text = SmokeSignalConfig.Instance.ExtraEmailMessage;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.ValidateChildren();

            serviceConfigured = (InvalidControls == 0);

            UpdateServiceStatus();

            StartingUp = false;

            OnDelay.InAMoment(CheckForUpdateSilent);
        }

        private void UpdateServiceStatus()
        {
            try
            {
                if (!ServiceManager.DoesServiceExist("SmokeSignalSrvc"))
                {
                    ServiceStatusLabel.Text = "Not installed!!!";
                    ServiceStatusLabel.ForeColor = Color.Red;
                }
                else
                {
                    ServiceControllerStatus status = ServiceManager.ServiceStatus("SmokeSignalSrvc");
                    string s = status.ToString();
                    Color c = Color.Green;
                    if (status != ServiceControllerStatus.Running)
                    {
                        c = Color.YellowGreen;
                    }

                    s += serviceConfigured ? ", configured" : ", not configured";

                    ServiceStatusLabel.ForeColor = c;
                    ServiceStatusLabel.Text = s;
                }
            }
            finally
            {
                OnDelay.Do(UpdateServiceStatus, 5 * 1000);
            }
        }

        #endregion Initilization

        #region field validation
        delegate bool ValidateDelegate(out bool isEmpty);

        private bool IsDataValid(Control target)
        {
            return target.Tag == null || (bool)target.Tag;
        }

        private void SomeField_Changed(object sender, EventArgs e)
        {
            if (!StartingUp)
            {
                ResetSaveButton(true);
            }
        }

        private void Validate(Control target, Label label, ValidateDelegate validator)
        {
            bool prevValid = IsDataValid(target);
            bool isEmpty;
            bool isValid = validator(out isEmpty);

            target.Tag = isValid;           // save 'valid' state in the tag field

            target.ForeColor = !isValid && !isEmpty ? Color.Red : Color.Black;
            label.ForeColor = !isValid ? Color.Red : Color.Black;
            InvalidControls += (prevValid == isValid) ? 0 : (!isValid ? 1 : -1);
            InvalidFieldsMessage.Visible = (InvalidControls > 0);
        }

        private void FromTextBox_Validating(object sender, CancelEventArgs e)
        {
            Validate(FromTextBox, FromLabel,
                delegate(out bool isEmpty) 
                {
                    isEmpty = EmptyText(FromTextBox);
                    return isEmpty || Utils.IsValidEmail(FromTextBox.Text);
                });
        }

        private void UsernameTextBox_Validating(object sender, CancelEventArgs e)
        {
            Validate(UsernameTextBox, UsernameLabel,
                delegate(out bool isEmpty) 
                {
                    isEmpty = EmptyText(UsernameTextBox);
                    return isEmpty || Utils.IsValidEmail(UsernameTextBox.Text); 
                });
        }

        private void PortTextBox_Validating(object sender, CancelEventArgs e)
        {
            Validate(PortTextBox, PortLabel,
                delegate(out bool isEmpty) {
                    int value;
                    isEmpty = EmptyText(PortTextBox);
                    return Int32.TryParse(PortTextBox.Text, out value) && (value > 0 && value <= 65535);
                });
        }

        private void HostnameTextBox_Validating(object sender, CancelEventArgs e)
        {
            Validate(HostnameTextBox, HostnameLabel,
                delegate(out bool isEmpty)
                {
                    isEmpty = EmptyText(HostnameTextBox);
                    return !isEmpty;
                });
        }

        private void DefaultDelayTextBox_Validating(object sender, CancelEventArgs e)
        {
            Validate(DefaultDelayTextBox, DefaultDelayLabel,
                delegate(out bool isEmpty)
                {
                    int value;
                    isEmpty = EmptyText(DefaultDelayTextBox);
                    return Int32.TryParse(DefaultDelayTextBox.Text, out value) && (value > 1 && value <= 10);
                });
        }

        private void CampfireTokenTextBox_Validating(object sender, CancelEventArgs e)
        {
            Validate(CampfireTokenTextBox, CampfireTokenLabel,
                delegate(out bool isEmpty)
                {
                    isEmpty = EmptyText(CampfireTokenTextBox);
                    return !isEmpty;
                });
            ValidateCampfireCredentials();
        }

        private void CampfireNameTextBox_Validating(object sender, CancelEventArgs e)
        {
            Validate(CampfireNameTextBox, CampfireNameLabel,
                delegate(out bool isEmpty)
                {
                    isEmpty = EmptyText(CampfireNameTextBox);
                    return !isEmpty;
                });
            ValidateCampfireCredentials();
        }

        #endregion     // field validation

        #region Close & Save Changes

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!configurationDirty)
            {
                Close();
                return;
            }

            if (InvalidControls > 0)
            {
                DialogResult result = MessageBox.Show("Some required fields are missing or invalid. Save changes anyway?", "Save changes?", MessageBoxButtons.OKCancel);
                if (result == DialogResult.Cancel)
                {
                    return;
                }
                // else 'yes' so continue with the save...
            }

            bool somethingChanged = false;
            List<string> args = new List<string>();
            string fullPath = null;

            if ((fullPath = SaveMailSettingsIfChanged()) != null)
            {
                args.Add(fullPath);
                somethingChanged = true;
            }
            if ((fullPath = SaveSmokeSignalSettingsIfChanged()) != null)
            {
                args.Add(fullPath);
                somethingChanged = true;
            }

            // resart the service if it is running in order for it to pickup new config
            if (somethingChanged && (ServiceManager.ServiceStatus("SmokeSignalSrvc") == ServiceControllerStatus.Running))
            {
                using (ChangeCursor c = new ChangeCursor(Cursors.WaitCursor))
                {
                    if (ServiceManager.DoesServiceExist("SmokeSignalSrvc"))
                    {
                        // need to restart the Smoke Signal Service
                        ServiceManager.StopService("SmokeSignalSrvc");
                        ServiceManager.StartService("SmokeSignalSrvc", args);
                    }
                }
                serviceConfigured = (InvalidControls == 0);
            }

            ResetSaveButton(false);
        }

        private void ResetSaveButton(bool dirty)
        {
            configurationDirty = dirty;
            bool isRunning = ServiceManager.ServiceStatus("SmokeSignalSrvc") == ServiceControllerStatus.Running;

            SaveButton.Text = isRunning && configurationDirty ? "Configure" : "Done";
        }

        private string SaveSmokeSignalSettingsIfChanged()
        {
            bool changed = false;

            if (IsDataValid(CampfireNameTextBox) && (CampfireNameTextBox.Text != SmokeSignalConfig.Instance.CampfireName))
            {
                SmokeSignalConfig.Instance.SetName(CampfireNameTextBox.Text);
                changed = true;
            }

            if (IsDataValid(CampfireTokenTextBox) && (CampfireTokenTextBox.Text != SmokeSignalConfig.Instance.CampfireToken))
            {
                SmokeSignalConfig.Instance.SetToken(CampfireTokenTextBox.Text);
                changed = true;
            }

            int delay;
            if (IsDataValid(DefaultDelayTextBox) && 
                int.TryParse(DefaultDelayTextBox.Text, out delay) && 
                (delay != SmokeSignalConfig.Instance.DelayBeforeSmokeSignalInMinutes))
            {
                SmokeSignalConfig.Instance.SetDelay(delay);
                changed = true;
            }
            bool sendEmail = SendWelcomeEmailCheckBox.Checked;
            if (sendEmail != SmokeSignalConfig.Instance.SendNewUserEmail)
            {
                SmokeSignalConfig.Instance.SetSendWelcomeEmail(sendEmail);
                changed = true;
            }
            if (ExtraEmailMessage.Text != SmokeSignalConfig.Instance.ExtraEmailMessage)
            {
                SmokeSignalConfig.Instance.SetExtraEmailMessage(ExtraEmailMessage.Text);
                changed = true;
            }

            string xmlFile = null;
            if (changed)
            {
                SmokeSignalConfig.SaveToStore(SmokeSignalConfig.Instance, Path.GetTempPath());
                xmlFile = Path.Combine(Path.GetTempPath(), Path.GetFileName(SmokeSignalConfig.BackingStoreFile));
            }

            return xmlFile;
        }

        private string SaveMailSettingsIfChanged()
        {
            bool changed = false;

            int port;
            if (IsDataValid(PortTextBox) && int.TryParse(PortTextBox.Text, out port) && (port != initialMailSettings.Smtp.Network.Port))
            {
                changed = true;
                initialMailSettings.Smtp.Network.Port = port;
            }

            if (IsDataValid(FromTextBox) && (FromTextBox.Text != initialMailSettings.Smtp.From) && 
                !(string.IsNullOrEmpty(FromTextBox.Text) && string.IsNullOrEmpty(initialMailSettings.Smtp.From)))
            {
                changed = true;
                initialMailSettings.Smtp.From = FromTextBox.Text;
            }

            if (IsDataValid(HostnameTextBox) && (HostnameTextBox.Text != initialMailSettings.Smtp.Network.Host))
            {
                changed = true;
                initialMailSettings.Smtp.Network.Host = HostnameTextBox.Text;
            }

            if (IsDataValid(UsernameTextBox) && (UsernameTextBox.Text != initialMailSettings.Smtp.Network.UserName))
            {
                changed = true;
                initialMailSettings.Smtp.Network.UserName = UsernameTextBox.Text;
            }

            if (IsDataValid(PasswordTextBox) && (PasswordTextBox.Text != initialMailSettings.Smtp.Network.Password))
            {
                changed = true;
                initialMailSettings.Smtp.Network.Password = PasswordTextBox.Text;
            }

            if (IsDataValid(DefaultCredsCheckBox) && (DefaultCredsCheckBox.Checked != initialMailSettings.Smtp.Network.DefaultCredentials))
            {
                changed = true;
                initialMailSettings.Smtp.Network.DefaultCredentials = DefaultCredsCheckBox.Checked;
            }

            string configFile = null;
            if (changed)
            {
                EventLog elog = new EventLog("Application");
                elog.Source = "SmokeSignal";
                elog.WriteEntry(string.Format("saving config: {0}", fullPathTmpConfig));

                config.SaveAs(fullPathTmpConfig);
                InitSSConfig();         // need to re-init 'config' because the "SaveAs" messes things up ???
                configFile = fullPathTmpConfig;
            }

            return configFile;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (configurationDirty && ServiceManager.ServiceStatus("SmokeSignalSrvc") == ServiceControllerStatus.Running)
            {
                DialogResult r = MessageBox.Show("Changes won't be saved?", "Close?", MessageBoxButtons.OKCancel);
                if (r == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        #endregion Close & Save changes

        private void DefaultCredsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UsernameTextBox.Enabled = !DefaultCredsCheckBox.Checked;
            PasswordTextBox.Enabled = !DefaultCredsCheckBox.Checked;
        }

        private void SendWelcomeEmailCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ExtraEmailMessage.Enabled = SendWelcomeEmailCheckBox.Checked;
        }
        private bool EmptyText(TextBox tb)
        {
            string t = tb.Text;
            if (t != null)
            {
                t = t.Trim();
            }
            return string.IsNullOrEmpty(t);
        }

        private void ValidateCampfireCredentials()
        {
            bool showStatus = !EmptyText(CampfireTokenTextBox) && !EmptyText(CampfireNameTextBox);

            CampfireCredsStatusPanel.Visible = showStatus;
            if (!showStatus)
            {
                return;
            }

            string campfireName = CampfireNameTextBox.Text;
            string token = CampfireTokenTextBox.Text;
            CampfireAPI.API api = new CampfireAPI.API(campfireName, token);
            List<CampfireAPI.Room> rooms = null;
            try
            {
                rooms = api.Rooms();
            }
            catch (Exception ex)
            {
            }

            bool validCreds = rooms != null && rooms.Count > 0;

            if (validCreds)
            {
                CampfireAPIStatus.Image = CampfireAPIStatus.InitialImage;
                RoomsFoundLabel.Text = string.Format("{0} rooms", rooms.Count);
            }
            else
            {
                CampfireAPIStatus.Image = CampfireAPIStatus.ErrorImage;
                RoomsFoundLabel.Text = string.Format("failed");
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            UIUtils.LaunchURL("http://www.smokesignalnow.com");
        }

        private void AboutButton_Click(object sender, EventArgs e)
        {
            AboutWindow aw = new AboutWindow();
            DialogResult result = aw.ShowDialog();
            aw.Close();

            if (result == DialogResult.OK)
            {
                OnDelay.InAMoment(CheckForUpdateUI);
            }
        }

        private void CheckForUpdate(bool silent)
        {
            string version;
            if ((version = UpdateAvailable.IsUpdateAvailable(true)) != null)
            {
                UpdateAvailable form = new UpdateAvailable(version);
                DialogResult r = form.ShowDialog();
                if (r == DialogResult.OK)
                {
                    this.Close();       // install has started so quit
                }
            }
            else if (!silent)
            {
                // Desktop.Properties.Resources.DialogTitle
                MessageBox.Show("There are no updates available.");
            }
        }

        private void CheckForUpdateUI()
        {
            CheckForUpdate(false);
        }

        private void CheckForUpdateSilent()
        {
            CheckForUpdate(true);
        }

        private void SendTestEmailButton_Click(object sender, EventArgs e)
        {
            int top = this.Bounds.Top;
            int right = this.Bounds.Right;

            if (!((bool)HostnameTextBox.Tag) || !((bool)PortTextBox.Tag))
            {
                MessageBox.Show("A valid Hostname and Port are required, at a minimum, to send a test email");
                return;
            }

            string emailAddr = Microsoft.VisualBasic.Interaction.InputBox(  
                "Enter \"To\" email addr. A test email will be sent, verifying that your email configuration is working.",
                "Test email", "", right - 70, top + 130);

            if (string.IsNullOrEmpty(emailAddr))
            {
                MessageBox.Show("No test email was sent.");
                return;
            }
            if (!Utils.IsValidEmail(emailAddr))
            {
                MessageBox.Show(string.Format("\"{0}\" is an invalid email address. No test email was sent.", emailAddr));
                return;
            }

            try
            {
                using (ChangeCursor cc = new ChangeCursor(Cursors.WaitCursor))
                {
                    Notifier mailer = new Notifier();
                    int port = 0;
                    Int32.TryParse(PortTextBox.Text, out port);
                    mailer.SentTestEmail(emailAddr, FromTextBox.Text, HostnameTextBox.Text, port,
                        UsernameTextBox.Text, PasswordTextBox.Text, DefaultCredsCheckBox.Checked, SendWelcomeEmailCheckBox.Checked ? ExtraEmailMessage.Text : null);
                }
                MessageBox.Show("Mail was successfully sent, please check your InBox");
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(string.Format("Sending mail failed. Exception: {0}, Message: {1}", ex.GetType().ToString(), ex.Message));
            }
            catch (SmtpException ex)
            {
                MessageBox.Show(string.Format("Sending mail failed. Exception: {0}, Message: {1}", ex.GetType().ToString(), ex.Message));
            }
        }

        private void OpenLogButton_Click(object sender, EventArgs e)
        {
        }
        private void ExtraEmailMessage_Enter(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(SmokeSignalConfig.Instance.ExtraEmailMessage))
            {
                ExtraEmailMessage.Text = string.Empty;
            }
        }
    }
}
