// Copyright 2012 Peter Potrebic.  All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SmokeSignalSetup
{
    public partial class AboutWindow : Form
    {
        public AboutWindow()
        {
            InitializeComponent();
            versionTextBox.Text = Program.VersionForUI;
        }

        private void LearnMoreLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            UIUtils.LaunchURL("http://www.smokesignalnow.com");
        }
    }
}
