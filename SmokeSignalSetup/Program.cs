// Copyright 2012 Peter Potrebic.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;

namespace SmokeSignalSetup
{
    static class Program
    {
        public static string RegistryRootPath = @"Software\NoDotO\SmokeSignal";
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
        }

        public static string VersionForUI
        {
            get
            {
                string v = Version;

                if (v.EndsWith(".0.0"))
                {
                    v = v.Substring(0, v.Length - ".0.0".Length);
                }
                else if (v.EndsWith(".0"))
                {
                    v = v.Substring(0, v.Length - ".0".Length);
                }
                else
                {
                    // trim off the last segment
                    int idx = v.LastIndexOf('.');
                    v = v.Substring(0, idx);
                }
                return string.Format("v{0}", v);
            }
        }

        public static string Version
        {
            get
            {
                Version v = Assembly.GetExecutingAssembly().GetName().Version;
                return v.ToString();
            }
        }
    }
}
