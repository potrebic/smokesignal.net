// Copyright 2012 Peter Potrebic.  All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Xml;
using System.Diagnostics;
using Microsoft.Win32;

namespace SmokeSignalSetup
{
    public partial class UpdateAvailable : Form
    {
        #region static

        private static string hostname = "xxx";
        private static string DefaultUpdateURLRoot = "http://somelocation/";

        enum Destination { InstallerBits, Info };

        private static string formatLocationURL = null;

        private static string LocationURL(Destination which)
        {
            if (formatLocationURL == null)
            {
                // look for "OverrideUpdateLocation" registry entry. This allows for testing a new release (in a temp location)
                // before making the new release public.
                using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey(Program.RegistryRootPath, false))
                {
                    object obj = regKey.GetValue("OverrideUpdateLocation");
                    if (obj != null && (obj is string))
                    {
                        formatLocationURL = (string)obj;
                    }
                }

                if (string.IsNullOrEmpty(formatLocationURL))
                {
                    formatLocationURL = DefaultUpdateURLRoot;
                }
            }

            string url = string.Format(formatLocationURL, hostname);

            return url;
        }

        /// <summary>
        /// Check for updates.
        /// </summary>
        /// <param name="forceCheck">if false respect the lazy logic in checking. If 'true' check for real.</param>
        /// <returns>the version string of the update, null if there is no update</returns>
        public static string IsUpdateAvailable(bool forceCheck)
        {
            DateTime now = DateTime.Now;

            UpdaterChecker checker;

            SmokeSignalSetup.UpdaterChecker.VersionGetter getLatestVersion;

            if (forceCheck)
            {
                getLatestVersion = GetLatestVersion;
            }
            else
            {
                getLatestVersion = GetLatestVersionCheckRegistry;
            }

            checker = new UpdaterChecker(
                delegate() { return Program.Version; },
                getLatestVersion
                );

            string newVersion = checker.Check();

            return newVersion;
        }

        /// <summary>
        /// The registry can call out a version to skip. Respect such a value.
        /// </summary>
        /// <returns></returns>
        private static string GetLatestVersionCheckRegistry()
        {
            string ver = GetLatestVersion();
            string skipVersion = null;

            // look for "SkipUpdate" registry entry
            using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey(Program.RegistryRootPath, false))
            {
                object obj = regKey.GetValue("SkipUpdate");
                if (obj != null && (obj is string))
                {
                    skipVersion = (string)obj;
                }
            }

            if (skipVersion != null && string.Equals(skipVersion, ver))
            {
                ver = null;
            }
            return ver;
        }

        private static string GetLatestVersion()
        {
            // need to find the right place to host new version of smokesignal

            return null;
#if false
            try
            {
                WebClient wc = new WebClient();
                Stream s = wc.OpenRead(string.Format("{0}{1}", LocationURL(Destination.Info), "smokesignalversion.xml"));
                StreamReader sr = new StreamReader(s);
                string data = sr.ReadToEnd();

                XmlDocument xdoc = new XmlDocument();
                xdoc.InnerXml = data;

                return xdoc["currentversion"].InnerText;
            }
            catch (WebException ex)
            {
                return null;
            }
#endif
        }

        private static string GetReleaseNotes(string version)
        {
            try
            {
                WebClient wc = new WebClient();
                Stream s = wc.OpenRead(string.Format("{0}{1}.{2}", LocationURL(Destination.Info), version, "smokesignalreleasenotes.txt"));
                StreamReader sr = new StreamReader(s);
                string data = sr.ReadToEnd();

                return data;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// Remove excessive trailing ".0" from the version string. For major release return "2.0" instead of just "2"
        /// </summary>
        /// <param name="versionString"></param>
        /// <returns></returns>
        public static string NormalizeVersionString(string versionString)
        {
            versionString = versionString.TrimEnd(new char[] { '.', '0' });

            // want at least a ".0" as in "2.0".
            if (!versionString.Contains("."))
            {
                versionString += ".0";
            }

            return versionString;
        }

        #endregion

        private string rawVersionString;

        public UpdateAvailable(string version)
        {
            rawVersionString = version;

            InitializeComponent();

            string notes = GetReleaseNotes(version);
            if (string.IsNullOrEmpty(notes))
            {
                notes = string.Format("Please consider upgrading to version {0}.",
                    NormalizeVersionString(rawVersionString));
            }

            releaseNotes.Text = notes;
            titleTextBox.Text = string.Format(titleTextBox.Text, NormalizeVersionString(rawVersionString));
        }

        #region private

        private void laterButton_Click(object sender, EventArgs e)
        {
            // bump the 'check for update' time so that we don't check for another 2 weeks
            //Desktop.Properties.Settings.Default.CheckForUpdateDate = DateTime.Now + CheckForUpdateIntervalLong;
            //this.DialogResult = DialogResult.Cancel;
            //this.Close();
        }

        private void installButton_Click(object sender, EventArgs e)
        {
            WebClient wc = new WebClient();

            DialogResult result = MessageBox.Show("Smoke Signal will automatically quit once the installation of the new version commences.",
                SmokeSignalSetup.Properties.Resources.DialogTitle, MessageBoxButtons.OKCancel);
            this.DialogResult = DialogResult.OK;

            if (result != DialogResult.OK)
            {
                this.DialogResult = result;
                return;
            }

            // delete any "SkipUpdate" registry entry
            //using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey(Program.RegistryRootPath, true))
            //{
            //    regKey.DeleteValue("SkipUpdate", false);
            //}

            try
            {
                //string flavor64 = OSUtils.IsOS64Bit() ? ".64x" : "";
                string flavor64 = "";
                string folder = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);
                string filename = System.IO.Path.Combine(folder, string.Format("SSInstaller{0}.msi", flavor64));

                using (ChangeCursor cc = new ChangeCursor(Cursors.WaitCursor))
                {
                    wc.DownloadFile(string.Format("{0}{1}.{2}{3}{4}", LocationURL(Destination.InstallerBits), this.rawVersionString, "SSInstaller", flavor64, ".msi"), filename);

                    // Launch the msi file to start the installation
                    ProcessStartInfo psi = new ProcessStartInfo(filename);
                    psi.UseShellExecute = true;
                    Process.Start(psi);
                }
            }
            catch (Exception ex)
            {
                this.DialogResult = DialogResult.Abort;
                MessageBox.Show(string.Format("Installation failed: {0}", ex.Message), SmokeSignalSetup.Properties.Resources.DialogTitle);
            }

            this.Close();
        }

        //private void ignoreUpdateButton_Click(object sender, EventArgs e)
        //{
        //    DialogResult r = MessageBox.Show(
        //            string.Format("Version {0} will be ignored.{1}{2}If you change your mind you can always install updates manually from the About Window.",
        //            NormalizeVersionString(rawVersionString), Environment.NewLine, Environment.NewLine),
        //        SmokeSignalSetup.Properties.Resources.DialogTitle, MessageBoxButtons.OKCancel);

        //    if (r == DialogResult.Cancel)
        //    {
        //        // user aborted the "ignore" action. So leave them looking at the "update available" dialog
        //        return;
        //    }

        //    this.DialogResult = DialogResult.Cancel;            // causes 'this' to dismiss itself AND not trigger the installation

        //    // set registry entry indicating that the current update should be ignored.
        //    using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey(Program.RegistryRootPath, true))
        //    {
        //        regKey.SetValue("SkipUpdate", rawVersionString, RegistryValueKind.String);
        //    }
        //}

        #endregion
    }
}
