// Copyright 2012 Peter Potrebic.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Diagnostics;
using System.Runtime.InteropServices;
using NotifierLibrary;


namespace SmokeSignal
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();

            //System.Windows.Forms.MessageBox.Show("ProjectInstaller: " + DoesServiceExist("SmokeSignal").ToString());
            
            this.BeforeInstall += new InstallEventHandler(ProjectInstaller_BeforeInstall);
            this.AfterInstall += new InstallEventHandler(ProjectInstaller_AfterInstall);
        }

        static bool DoesServiceExist(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();

            // try to find service name
            foreach (ServiceController service in services)
            {
                if (service.ServiceName == serviceName)
                    return true;
            }
            return false;
        }

        void ProjectInstaller_BeforeInstall(object sender, InstallEventArgs e)
        {
            //System.Windows.Forms.MessageBox.Show("Already installed: " + DoesServiceExist("SmokeSignal").ToString());

            //EventLog elog = new EventLog("Application");
            //elog.Source = "SmokeSignal";
            //elog.WriteEntry(string.Format("ProjectInstaller_BeforeInstall, exists:{0}", DoesServiceExist("SmokeSignal")));

            if (!ServiceManager.DoesServiceExist("SmokeSignalSrvc"))
            {
                //System.Windows.Forms.MessageBox.Show("!DoesServiceExist(SmokeSignal) == true");
                return;
            }
            try
            {
                ServiceManager.StopService("SmokeSignalSrvc");

                //elog.WriteEntry(string.Format("ProjectInstaller_BeforeInstall, Stopped"));
                //System.Windows.Forms.MessageBox.Show("After Stop");

                ServiceManager.UnInstallService("localhost", "SmokeSignalSrvc");
            }
            catch (System.ServiceProcess.TimeoutException ex)
            {
                //System.Windows.Forms.MessageBox.Show("TimeoutException");
                //elog.WriteEntry(string.Format("ProjectInstaller_BeforeInstall, TimeoutException"));
            }
        }

        void ProjectInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            ServiceManager.StartService("SmokeSignalSrvc");
        }
    }
}
