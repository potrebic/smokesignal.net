namespace SmokeSignalSetup
{
    partial class AboutWindow
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
            this.titleTextBox = new System.Windows.Forms.TextBox();
            this.versionTextBox = new System.Windows.Forms.TextBox();
            this.DoneButton = new System.Windows.Forms.Button();
            this.CheckForUpdatesButton = new System.Windows.Forms.Button();
            this.LearnMoreLink = new System.Windows.Forms.LinkLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // titleTextBox
            // 
            this.titleTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.titleTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleTextBox.Location = new System.Drawing.Point(12, 13);
            this.titleTextBox.Multiline = true;
            this.titleTextBox.Name = "titleTextBox";
            this.titleTextBox.ReadOnly = true;
            this.titleTextBox.Size = new System.Drawing.Size(260, 24);
            this.titleTextBox.TabIndex = 4;
            this.titleTextBox.TabStop = false;
            this.titleTextBox.Text = "This is Smoke Signal";
            this.titleTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // versionTextBox
            // 
            this.versionTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.versionTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.versionTextBox.Location = new System.Drawing.Point(12, 43);
            this.versionTextBox.Multiline = true;
            this.versionTextBox.Name = "versionTextBox";
            this.versionTextBox.ReadOnly = true;
            this.versionTextBox.Size = new System.Drawing.Size(260, 24);
            this.versionTextBox.TabIndex = 5;
            this.versionTextBox.TabStop = false;
            this.versionTextBox.Text = "<v>";
            this.versionTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // DoneButton
            // 
            this.DoneButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.DoneButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.DoneButton.Location = new System.Drawing.Point(197, 185);
            this.DoneButton.Name = "DoneButton";
            this.DoneButton.Size = new System.Drawing.Size(75, 23);
            this.DoneButton.TabIndex = 6;
            this.DoneButton.Text = "Done";
            this.DoneButton.UseVisualStyleBackColor = true;
            // 
            // CheckForUpdatesButton
            // 
            this.CheckForUpdatesButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.CheckForUpdatesButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.CheckForUpdatesButton.Location = new System.Drawing.Point(12, 185);
            this.CheckForUpdatesButton.Name = "CheckForUpdatesButton";
            this.CheckForUpdatesButton.Size = new System.Drawing.Size(121, 23);
            this.CheckForUpdatesButton.TabIndex = 7;
            this.CheckForUpdatesButton.Text = "Check For Updates";
            this.CheckForUpdatesButton.UseVisualStyleBackColor = true;
            // 
            // LearnMoreLink
            // 
            this.LearnMoreLink.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.LearnMoreLink.AutoSize = true;
            this.LearnMoreLink.Location = new System.Drawing.Point(81, 156);
            this.LearnMoreLink.Name = "LearnMoreLink";
            this.LearnMoreLink.Size = new System.Drawing.Size(118, 13);
            this.LearnMoreLink.TabIndex = 8;
            this.LearnMoreLink.TabStop = true;
            this.LearnMoreLink.Text = "Click here to learn more";
            this.LearnMoreLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LearnMoreLink_LinkClicked);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::SmokeSignalSetup.Properties.Resources.SS_icon_64;
            this.pictureBox1.Location = new System.Drawing.Point(110, 79);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(64, 64);
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            // 
            // AboutWindow
            // 
            this.AcceptButton = this.CheckForUpdatesButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.DoneButton;
            this.ClientSize = new System.Drawing.Size(284, 220);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.LearnMoreLink);
            this.Controls.Add(this.CheckForUpdatesButton);
            this.Controls.Add(this.DoneButton);
            this.Controls.Add(this.versionTextBox);
            this.Controls.Add(this.titleTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutWindow";
            this.Text = "About Smoke Signal";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox titleTextBox;
        private System.Windows.Forms.TextBox versionTextBox;
        private System.Windows.Forms.Button DoneButton;
        private System.Windows.Forms.Button CheckForUpdatesButton;
        private System.Windows.Forms.LinkLabel LearnMoreLink;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}