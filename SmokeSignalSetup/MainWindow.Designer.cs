namespace SmokeSignalSetup
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.FromTextBox = new System.Windows.Forms.TextBox();
            this.FromLabel = new System.Windows.Forms.Label();
            this.HostnameLabel = new System.Windows.Forms.Label();
            this.HostnameTextBox = new System.Windows.Forms.TextBox();
            this.PortLabel = new System.Windows.Forms.Label();
            this.PortTextBox = new System.Windows.Forms.TextBox();
            this.UsernameLabel = new System.Windows.Forms.Label();
            this.PasswordLabel = new System.Windows.Forms.Label();
            this.UsernameTextBox = new System.Windows.Forms.TextBox();
            this.PasswordTextBox = new System.Windows.Forms.TextBox();
            this.DefaultCredsCheckBox = new System.Windows.Forms.CheckBox();
            this.SaveButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.CampfireNameLabel = new System.Windows.Forms.Label();
            this.CampfireTokenLabel = new System.Windows.Forms.Label();
            this.CampfireNameTextBox = new System.Windows.Forms.TextBox();
            this.CampfireTokenTextBox = new System.Windows.Forms.TextBox();
            this.DefaultDelayTextBox = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.AboutButton = new System.Windows.Forms.Button();
            this.OpenLogButton = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label10 = new System.Windows.Forms.Label();
            this.DefaultDelayLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.InvalidFieldsMessage = new System.Windows.Forms.Label();
            this.CampfireCredsStatusPanel = new System.Windows.Forms.Panel();
            this.RoomsFoundLabel = new System.Windows.Forms.Label();
            this.CampfireAPIStatus = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.SendTestEmailButton = new System.Windows.Forms.Button();
            this.ServiceStatusLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SendWelcomeEmailCheckBox = new System.Windows.Forms.CheckBox();
            this.ExtraEmailMessage = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.CampfireCredsStatusPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CampfireAPIStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // FromTextBox
            // 
            this.FromTextBox.Location = new System.Drawing.Point(71, 26);
            this.FromTextBox.Name = "FromTextBox";
            this.FromTextBox.Size = new System.Drawing.Size(227, 20);
            this.FromTextBox.TabIndex = 40;
            this.FromTextBox.TextChanged += new System.EventHandler(this.SomeField_Changed);
            this.FromTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.FromTextBox_Validating);
            // 
            // FromLabel
            // 
            this.FromLabel.AutoSize = true;
            this.FromLabel.Location = new System.Drawing.Point(11, 29);
            this.FromLabel.Name = "FromLabel";
            this.FromLabel.Size = new System.Drawing.Size(30, 13);
            this.FromLabel.TabIndex = 2;
            this.FromLabel.Text = "From";
            // 
            // HostnameLabel
            // 
            this.HostnameLabel.AutoSize = true;
            this.HostnameLabel.Location = new System.Drawing.Point(11, 55);
            this.HostnameLabel.Name = "HostnameLabel";
            this.HostnameLabel.Size = new System.Drawing.Size(55, 13);
            this.HostnameLabel.TabIndex = 3;
            this.HostnameLabel.Text = "Hostname";
            // 
            // HostnameTextBox
            // 
            this.HostnameTextBox.Location = new System.Drawing.Point(71, 52);
            this.HostnameTextBox.Name = "HostnameTextBox";
            this.HostnameTextBox.Size = new System.Drawing.Size(160, 20);
            this.HostnameTextBox.TabIndex = 50;
            this.HostnameTextBox.TextChanged += new System.EventHandler(this.SomeField_Changed);
            this.HostnameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.HostnameTextBox_Validating);
            // 
            // PortLabel
            // 
            this.PortLabel.AutoSize = true;
            this.PortLabel.Location = new System.Drawing.Point(240, 55);
            this.PortLabel.Name = "PortLabel";
            this.PortLabel.Size = new System.Drawing.Size(26, 13);
            this.PortLabel.TabIndex = 5;
            this.PortLabel.Text = "Port";
            // 
            // PortTextBox
            // 
            this.PortTextBox.Location = new System.Drawing.Point(266, 52);
            this.PortTextBox.Name = "PortTextBox";
            this.PortTextBox.Size = new System.Drawing.Size(32, 20);
            this.PortTextBox.TabIndex = 60;
            this.PortTextBox.TextChanged += new System.EventHandler(this.SomeField_Changed);
            this.PortTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.PortTextBox_Validating);
            // 
            // UsernameLabel
            // 
            this.UsernameLabel.AutoSize = true;
            this.UsernameLabel.Location = new System.Drawing.Point(11, 81);
            this.UsernameLabel.Name = "UsernameLabel";
            this.UsernameLabel.Size = new System.Drawing.Size(55, 13);
            this.UsernameLabel.TabIndex = 7;
            this.UsernameLabel.Text = "Username";
            // 
            // PasswordLabel
            // 
            this.PasswordLabel.AutoSize = true;
            this.PasswordLabel.Location = new System.Drawing.Point(11, 108);
            this.PasswordLabel.Name = "PasswordLabel";
            this.PasswordLabel.Size = new System.Drawing.Size(53, 13);
            this.PasswordLabel.TabIndex = 8;
            this.PasswordLabel.Text = "Password";
            // 
            // UsernameTextBox
            // 
            this.UsernameTextBox.Location = new System.Drawing.Point(71, 78);
            this.UsernameTextBox.Name = "UsernameTextBox";
            this.UsernameTextBox.Size = new System.Drawing.Size(160, 20);
            this.UsernameTextBox.TabIndex = 70;
            this.UsernameTextBox.TextChanged += new System.EventHandler(this.SomeField_Changed);
            this.UsernameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.UsernameTextBox_Validating);
            // 
            // PasswordTextBox
            // 
            this.PasswordTextBox.Location = new System.Drawing.Point(71, 105);
            this.PasswordTextBox.Name = "PasswordTextBox";
            this.PasswordTextBox.Size = new System.Drawing.Size(160, 20);
            this.PasswordTextBox.TabIndex = 80;
            this.PasswordTextBox.UseSystemPasswordChar = true;
            this.PasswordTextBox.TextChanged += new System.EventHandler(this.SomeField_Changed);
            // 
            // DefaultCredsCheckBox
            // 
            this.DefaultCredsCheckBox.AutoSize = true;
            this.DefaultCredsCheckBox.Location = new System.Drawing.Point(71, 132);
            this.DefaultCredsCheckBox.Name = "DefaultCredsCheckBox";
            this.DefaultCredsCheckBox.Size = new System.Drawing.Size(115, 17);
            this.DefaultCredsCheckBox.TabIndex = 90;
            this.DefaultCredsCheckBox.Text = "Default Credentials";
            this.DefaultCredsCheckBox.UseVisualStyleBackColor = true;
            this.DefaultCredsCheckBox.CheckedChanged += new System.EventHandler(this.DefaultCredsCheckBox_CheckedChanged);
            this.DefaultCredsCheckBox.TextChanged += new System.EventHandler(this.SomeField_Changed);
            // 
            // SaveButton
            // 
            this.SaveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SaveButton.Location = new System.Drawing.Point(414, 412);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 23);
            this.SaveButton.TabIndex = 130;
            this.SaveButton.Text = "Done";
            this.toolTip1.SetToolTip(this.SaveButton, "Save changes and restart the Smoke Signal Service if required");
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.FromLabel);
            this.groupBox1.Controls.Add(this.FromTextBox);
            this.groupBox1.Controls.Add(this.DefaultCredsCheckBox);
            this.groupBox1.Controls.Add(this.HostnameLabel);
            this.groupBox1.Controls.Add(this.PasswordTextBox);
            this.groupBox1.Controls.Add(this.HostnameTextBox);
            this.groupBox1.Controls.Add(this.UsernameTextBox);
            this.groupBox1.Controls.Add(this.PortLabel);
            this.groupBox1.Controls.Add(this.PasswordLabel);
            this.groupBox1.Controls.Add(this.PortTextBox);
            this.groupBox1.Controls.Add(this.UsernameLabel);
            this.groupBox1.Location = new System.Drawing.Point(10, 249);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(309, 157);
            this.groupBox1.TabIndex = 35;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Email Config";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(232, 82);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "(email addr)";
            // 
            // CampfireNameLabel
            // 
            this.CampfireNameLabel.AutoSize = true;
            this.CampfireNameLabel.Location = new System.Drawing.Point(73, 73);
            this.CampfireNameLabel.Name = "CampfireNameLabel";
            this.CampfireNameLabel.Size = new System.Drawing.Size(77, 13);
            this.CampfireNameLabel.TabIndex = 14;
            this.CampfireNameLabel.Text = "Campfire name";
            // 
            // CampfireTokenLabel
            // 
            this.CampfireTokenLabel.AutoSize = true;
            this.CampfireTokenLabel.Location = new System.Drawing.Point(73, 100);
            this.CampfireTokenLabel.Name = "CampfireTokenLabel";
            this.CampfireTokenLabel.Size = new System.Drawing.Size(78, 13);
            this.CampfireTokenLabel.TabIndex = 15;
            this.CampfireTokenLabel.Text = "Campfire token";
            // 
            // CampfireNameTextBox
            // 
            this.CampfireNameTextBox.Location = new System.Drawing.Point(200, 70);
            this.CampfireNameTextBox.Name = "CampfireNameTextBox";
            this.CampfireNameTextBox.Size = new System.Drawing.Size(121, 20);
            this.CampfireNameTextBox.TabIndex = 10;
            this.toolTip1.SetToolTip(this.CampfireNameTextBox, "The XYZ portion of XYZ.campfire.now URL");
            this.CampfireNameTextBox.TextChanged += new System.EventHandler(this.SomeField_Changed);
            this.CampfireNameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.CampfireNameTextBox_Validating);
            // 
            // CampfireTokenTextBox
            // 
            this.CampfireTokenTextBox.Location = new System.Drawing.Point(157, 96);
            this.CampfireTokenTextBox.Name = "CampfireTokenTextBox";
            this.CampfireTokenTextBox.Size = new System.Drawing.Size(255, 20);
            this.CampfireTokenTextBox.TabIndex = 20;
            this.toolTip1.SetToolTip(this.CampfireTokenTextBox, "Campfire\'s API token, required to access the campfire room");
            this.CampfireTokenTextBox.TextChanged += new System.EventHandler(this.SomeField_Changed);
            this.CampfireTokenTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.CampfireTokenTextBox_Validating);
            // 
            // DefaultDelayTextBox
            // 
            this.DefaultDelayTextBox.Location = new System.Drawing.Point(157, 126);
            this.DefaultDelayTextBox.Name = "DefaultDelayTextBox";
            this.DefaultDelayTextBox.Size = new System.Drawing.Size(34, 20);
            this.DefaultDelayTextBox.TabIndex = 30;
            this.toolTip1.SetToolTip(this.DefaultDelayTextBox, "Default delay before sending out a Smoke Signal (email) to the targetted person.\r" +
                    "\nValid values are 1 thru 10 minutes.");
            this.DefaultDelayTextBox.TextChanged += new System.EventHandler(this.SomeField_Changed);
            this.DefaultDelayTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.DefaultDelayTextBox_Validating);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(57, 39);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(387, 1);
            this.panel1.TabIndex = 21;
            // 
            // AboutButton
            // 
            this.AboutButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.AboutButton.Location = new System.Drawing.Point(331, 412);
            this.AboutButton.Name = "AboutButton";
            this.AboutButton.Size = new System.Drawing.Size(75, 23);
            this.AboutButton.TabIndex = 120;
            this.AboutButton.Text = "About";
            this.AboutButton.UseVisualStyleBackColor = true;
            this.AboutButton.Click += new System.EventHandler(this.AboutButton_Click);
            // 
            // OpenLogButton
            // 
            this.OpenLogButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OpenLogButton.Location = new System.Drawing.Point(414, 351);
            this.OpenLogButton.Name = "OpenLogButton";
            this.OpenLogButton.Size = new System.Drawing.Size(75, 23);
            this.OpenLogButton.TabIndex = 110;
            this.OpenLogButton.Text = "View Log";
            this.toolTip1.SetToolTip(this.OpenLogButton, "View and configure Smoke Signal\'s activity log");
            this.OpenLogButton.UseVisualStyleBackColor = true;
            this.OpenLogButton.Visible = false;
            this.OpenLogButton.Click += new System.EventHandler(this.OpenLogButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(321, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 13);
            this.label2.TabIndex = 26;
            this.label2.Text = ".campfirenow.com";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(194, 129);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(49, 13);
            this.label9.TabIndex = 27;
            this.label9.Text = "(minutes)";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel1.Location = new System.Drawing.Point(374, 12);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(107, 20);
            this.linkLabel1.TabIndex = 200;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Smoke Signal";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(336, 282);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(144, 13);
            this.label10.TabIndex = 29;
            this.label10.Text = "Smoke Signal Service Status";
            // 
            // DefaultDelayLabel
            // 
            this.DefaultDelayLabel.AutoSize = true;
            this.DefaultDelayLabel.Location = new System.Drawing.Point(73, 129);
            this.DefaultDelayLabel.Name = "DefaultDelayLabel";
            this.DefaultDelayLabel.Size = new System.Drawing.Size(69, 13);
            this.DefaultDelayLabel.TabIndex = 131;
            this.DefaultDelayLabel.Text = "Default delay";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(157, 73);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 132;
            this.label5.Text = "https://";
            // 
            // InvalidFieldsMessage
            // 
            this.InvalidFieldsMessage.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.InvalidFieldsMessage.AutoSize = true;
            this.InvalidFieldsMessage.ForeColor = System.Drawing.Color.Red;
            this.InvalidFieldsMessage.Location = new System.Drawing.Point(46, 422);
            this.InvalidFieldsMessage.Name = "InvalidFieldsMessage";
            this.InvalidFieldsMessage.Size = new System.Drawing.Size(233, 13);
            this.InvalidFieldsMessage.TabIndex = 201;
            this.InvalidFieldsMessage.Text = "Fields in red are required and/or currently invalid";
            this.InvalidFieldsMessage.Visible = false;
            // 
            // CampfireCredsStatusPanel
            // 
            this.CampfireCredsStatusPanel.Controls.Add(this.RoomsFoundLabel);
            this.CampfireCredsStatusPanel.Controls.Add(this.CampfireAPIStatus);
            this.CampfireCredsStatusPanel.Controls.Add(this.panel2);
            this.CampfireCredsStatusPanel.Location = new System.Drawing.Point(418, 68);
            this.CampfireCredsStatusPanel.Name = "CampfireCredsStatusPanel";
            this.CampfireCredsStatusPanel.Size = new System.Drawing.Size(79, 57);
            this.CampfireCredsStatusPanel.TabIndex = 202;
            this.CampfireCredsStatusPanel.Visible = false;
            // 
            // RoomsFoundLabel
            // 
            this.RoomsFoundLabel.Location = new System.Drawing.Point(11, 35);
            this.RoomsFoundLabel.Name = "RoomsFoundLabel";
            this.RoomsFoundLabel.Size = new System.Drawing.Size(59, 18);
            this.RoomsFoundLabel.TabIndex = 204;
            this.RoomsFoundLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // CampfireAPIStatus
            // 
            this.CampfireAPIStatus.ErrorImage = global::SmokeSignalSetup.Properties.Resources.image_No;
            this.CampfireAPIStatus.InitialImage = global::SmokeSignalSetup.Properties.Resources.image_Yes;
            this.CampfireAPIStatus.Location = new System.Drawing.Point(25, 10);
            this.CampfireAPIStatus.Name = "CampfireAPIStatus";
            this.CampfireAPIStatus.Size = new System.Drawing.Size(26, 22);
            this.CampfireAPIStatus.TabIndex = 203;
            this.CampfireAPIStatus.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Location = new System.Drawing.Point(4, 6);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(2, 42);
            this.panel2.TabIndex = 203;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "image_No");
            this.imageList1.Images.SetKeyName(1, "image_Yes");
            // 
            // SendTestEmailButton
            // 
            this.SendTestEmailButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SendTestEmailButton.Location = new System.Drawing.Point(331, 381);
            this.SendTestEmailButton.Name = "SendTestEmailButton";
            this.SendTestEmailButton.Size = new System.Drawing.Size(75, 23);
            this.SendTestEmailButton.TabIndex = 91;
            this.SendTestEmailButton.Text = "Test Email...";
            this.SendTestEmailButton.UseVisualStyleBackColor = true;
            this.SendTestEmailButton.Click += new System.EventHandler(this.SendTestEmailButton_Click);
            // 
            // ServiceStatusLabel
            // 
            this.ServiceStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ServiceStatusLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ServiceStatusLabel.Location = new System.Drawing.Point(333, 303);
            this.ServiceStatusLabel.Name = "ServiceStatusLabel";
            this.ServiceStatusLabel.Size = new System.Drawing.Size(149, 21);
            this.ServiceStatusLabel.TabIndex = 203;
            this.ServiceStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(10, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(479, 25);
            this.label1.TabIndex = 204;
            this.label1.Text = "Admin priviledges needed in order to control/start/stop the Smoke Signal Service";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(23, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(354, 20);
            this.label3.TabIndex = 205;
            this.label3.Text = "Please fill in the following information to configure";
            // 
            this.SendWelcomeEmailCheckBox.AutoSize = true;
            this.SendWelcomeEmailCheckBox.Location = new System.Drawing.Point(57, 152);
            this.SendWelcomeEmailCheckBox.Name = "SendWelcomeEmailCheckBox";
            this.SendWelcomeEmailCheckBox.Size = new System.Drawing.Size(127, 17);
            this.SendWelcomeEmailCheckBox.TabIndex = 206;
            this.SendWelcomeEmailCheckBox.Text = "Send Welcome Email";
            this.SendWelcomeEmailCheckBox.UseVisualStyleBackColor = true;
            this.SendWelcomeEmailCheckBox.CheckedChanged += new System.EventHandler(this.SendWelcomeEmailCheckBox_CheckedChanged);
            this.SendWelcomeEmailCheckBox.TextChanged += new System.EventHandler(this.SomeField_Changed);
            this.ExtraEmailMessage.Enabled = false;
            this.ExtraEmailMessage.Location = new System.Drawing.Point(76, 175);
            this.ExtraEmailMessage.Multiline = true;
            this.ExtraEmailMessage.Name = "ExtraEmailMessage";
            this.ExtraEmailMessage.Size = new System.Drawing.Size(404, 57);
            this.ExtraEmailMessage.TabIndex = 207;
            this.ExtraEmailMessage.Text = "[Extra Message]";
            this.ExtraEmailMessage.TextChanged += new System.EventHandler(this.SomeField_Changed);
            this.ExtraEmailMessage.Enter += new System.EventHandler(this.ExtraEmailMessage_Enter);
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 447);
            this.Controls.Add(this.ExtraEmailMessage);
            this.Controls.Add(this.SendWelcomeEmailCheckBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ServiceStatusLabel);
            this.Controls.Add(this.SendTestEmailButton);
            this.Controls.Add(this.CampfireCredsStatusPanel);
            this.Controls.Add(this.InvalidFieldsMessage);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.DefaultDelayLabel);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.OpenLogButton);
            this.Controls.Add(this.AboutButton);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.DefaultDelayTextBox);
            this.Controls.Add(this.CampfireTokenTextBox);
            this.Controls.Add(this.CampfireNameTextBox);
            this.Controls.Add(this.CampfireTokenLabel);
            this.Controls.Add(this.CampfireNameLabel);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.SaveButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindow";
            this.Text = "Smoke Signal Startup";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.CampfireCredsStatusPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.CampfireAPIStatus)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox FromTextBox;
        private System.Windows.Forms.Label FromLabel;
        private System.Windows.Forms.Label HostnameLabel;
        private System.Windows.Forms.TextBox HostnameTextBox;
        private System.Windows.Forms.Label PortLabel;
        private System.Windows.Forms.TextBox PortTextBox;
        private System.Windows.Forms.Label UsernameLabel;
        private System.Windows.Forms.Label PasswordLabel;
        private System.Windows.Forms.TextBox UsernameTextBox;
        private System.Windows.Forms.TextBox PasswordTextBox;
        private System.Windows.Forms.CheckBox DefaultCredsCheckBox;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox DefaultDelayTextBox;
        private System.Windows.Forms.TextBox CampfireTokenTextBox;
        private System.Windows.Forms.TextBox CampfireNameTextBox;
        private System.Windows.Forms.Label CampfireTokenLabel;
        private System.Windows.Forms.Label CampfireNameLabel;
        private System.Windows.Forms.Button AboutButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button OpenLogButton;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label DefaultDelayLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label InvalidFieldsMessage;
        private System.Windows.Forms.Panel CampfireCredsStatusPanel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox CampfireAPIStatus;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label RoomsFoundLabel;
        private System.Windows.Forms.Button SendTestEmailButton;
        private System.Windows.Forms.Label ServiceStatusLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox SendWelcomeEmailCheckBox;
        private System.Windows.Forms.TextBox ExtraEmailMessage;
    }
}

