// Copyright 2012 Peter Potrebic.  All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Reflection;
using NotifierLibrary;
using System.Net.Configuration;
using System.Configuration;
using System.IO;

namespace SmokeSignal
{
    public partial class SmokeSignal : ServiceBase
    {
        public SmokeSignal()
        {
            InitializeComponent();

            if (!EventLog.SourceExists("SmokeSignal"))
            {
                EventLog.CreateEventSource("SmokeSignal", "Application");
            }
        }

        private void ProcessArguments(string[] args)
        {
            foreach (string path in args)
            {
                if (path.ToLowerInvariant().EndsWith(".config"))
                {
                    // Changes in the app.config file for the exe.
                    string myConfigPath = Assembly.GetExecutingAssembly().Location;
                    string myDir = Path.GetDirectoryName(myConfigPath);
                    myConfigPath = Path.Combine(myDir, "SmokeSignalSrvc.exe.config");

                    File.Copy(path, myConfigPath, true);

                    ConfigurationManager.RefreshSection("system.net");
                }
                if (path.ToLowerInvariant().EndsWith(".xml"))
                {
                    // Changes in SmokeSignalConfig.xml
                    string leaf = Path.GetFileName(path);
                    string dest = Path.Combine(Utils.DataLocation, leaf);
                    File.Copy(path, dest, true);
                }
            }
        }

        protected override void OnStart(string[] args)
        {
            string path = Utils.DataLocation;
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            StringBuilder sb = new StringBuilder();
            if (args != null)
            {
                sb.AppendFormat("args: {0}, {1}", args.Length > 0 ? args[0] : "xxx", args.Length > 1 ? args[1] : "xxx");
            }
            else
            {
                sb.AppendFormat("args: NONE");
            }

            sb.AppendLine();

            Configuration userConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);

            sb.AppendFormat("config.FilePath: {0}", config.FilePath);
            sb.AppendLine();
            sb.AppendFormat("userConfig.FilePath: {0}", userConfig.FilePath);
            sb.AppendLine();
            sb.AppendFormat("DataLocation: {0}", path);
            sb.AppendLine();

            theEventLog.WriteEntry(string.Format("SmokeSignal starting. {0}", sb.ToString()));

            if (args != null && args.Length > 0)
            {
                ProcessArguments(args);
            }
            
            SmokeSignalConfig.Initialize(path);
            CampfireState.Initialize(path, null);
            MessageProcessor mp = new MessageProcessor(CampfireState.Instance);
            mp.Run(false);
        }

        protected override void OnStop()
        {
        }
    }
}
