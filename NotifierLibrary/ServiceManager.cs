// Copyright 2012 Peter Potrebic.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using System.Runtime.InteropServices;

namespace NotifierLibrary
{

    /// <summary>
    /// API and managing the service. Can stop, start, install, uninstall
    /// </summary>
    public class ServiceManager
    {
        public static bool DoesServiceExist(string serviceName)
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

        public static void StartService(string serviceName)
        {
            StartService(serviceName, null);
        }

        public static ServiceControllerStatus ServiceStatus(string serviceName)
        {
            ServiceControllerStatus status = ServiceControllerStatus.Stopped;
            if (ServiceManager.DoesServiceExist(serviceName))
            {
                ServiceController sc = new ServiceController(serviceName);
                status = sc.Status;
            }
            return status;
        }

        public static void StartService(string serviceName, List<string> args)
        {
            if (ServiceManager.DoesServiceExist(serviceName))
            {
                ServiceController sc = new ServiceController(serviceName);
                if (args != null && args.Count > 0)
                {
                    sc.Start(args.ToArray());
                }
                else
                {
                    sc.Start();
                }
                TimeSpan timeout = TimeSpan.FromMilliseconds(15 * 1000);
                try
                {
                    sc.WaitForStatus(ServiceControllerStatus.Running, timeout);
                }
                catch (System.ServiceProcess.TimeoutException ex)
                {
                }
            }
        }

        public static void StopService(string serviceName)
        {
            ServiceController sc = new ServiceController(serviceName);
            TimeSpan timeout = TimeSpan.FromMilliseconds(15 * 1000);

            sc.Stop();
            try
            {
                sc.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch (System.ServiceProcess.TimeoutException ex)
            {
            }
        }

        #region DLLImport

        [DllImport("advapi32.dll")]
        public static extern IntPtr OpenSCManager(string lpMachineName, string lpSCDB, int scParameter);

        [DllImport("Advapi32.dll")]
        public static extern IntPtr CreateService(IntPtr SC_HANDLE, string lpSvcName, string lpDisplayName,
        int dwDesiredAccess, int dwServiceType, int dwStartType, int dwErrorControl, string lpPathName,
        string lpLoadOrderGroup, int lpdwTagId, string lpDependencies, string lpServiceStartName, string lpPassword);

        [DllImport("advapi32.dll")]
        public static extern void CloseServiceHandle(IntPtr SCHANDLE);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern IntPtr OpenService(IntPtr SCHANDLE, string lpSvcName, int dwNumServiceArgs);

        [DllImport("advapi32.dll")]
        public static extern int DeleteService(IntPtr SVHANDLE);

        [DllImport("kernel32.dll")]
        public static extern int GetLastError();

        #endregion DLLImport

        /// This method installs and runs the service in the service control manager.
        /// </summary>
        /// <param name="svcPath">The complete path of the service.</param>
        /// <param name="svcName">Name of the service.</param>
        /// <param name="svcDispName">Display name of the service.</param>
        /// <returns>True if the process went thro successfully. False if there was any  error.</returns>
        public static void InstallService(string machineName, string svcPath, string svcName, string svcDispName)
        {
            #region Constants declaration.
            int SC_MANAGER_CREATE_SERVICE = 0x0002;
            int SERVICE_WIN32_OWN_PROCESS = 0x00000010;
            int SERVICE_INTERACTIVE_PROCESS = 0x00000100;
            int SERVICE_ERROR_NORMAL = 0x00000001;
            int STANDARD_RIGHTS_REQUIRED = 0xF0000;
            int SERVICE_QUERY_CONFIG = 0x0001;
            int SERVICE_CHANGE_CONFIG = 0x0002;
            int SERVICE_QUERY_STATUS = 0x0004;
            int SERVICE_ENUMERATE_DEPENDENTS = 0x0008;
            int SERVICE_START = 0x0010;
            int SERVICE_STOP = 0x0020;
            int SERVICE_PAUSE_CONTINUE = 0x0040;
            int SERVICE_INTERROGATE = 0x0080;
            int SERVICE_USER_DEFINED_CONTROL = 0x0100;
            int SERVICE_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED |
            SERVICE_QUERY_CONFIG |
            SERVICE_CHANGE_CONFIG |
            SERVICE_QUERY_STATUS |
            SERVICE_ENUMERATE_DEPENDENTS |
            SERVICE_START |
            SERVICE_STOP |
            SERVICE_PAUSE_CONTINUE |
            SERVICE_INTERROGATE |
            SERVICE_USER_DEFINED_CONTROL);
            int SERVICE_AUTO_START = 0x00000002;
            #endregion Constants declaration.

            IntPtr sc_handle = new IntPtr(0);
            IntPtr sv_handle = new IntPtr(0);

            try
            {
                sc_handle = OpenSCManager(machineName,
                                            null,
                                            SC_MANAGER_CREATE_SERVICE
                                            );

                if (sc_handle.ToInt32() == 0)
                {
                    throw new ApplicationException(String.Format("OpenSCManager returned 0: GetLastError = {0}", GetLastError()));
                }

                sv_handle = CreateService(sc_handle,
                                            svcName,
                                            svcDispName,
                                            SERVICE_ALL_ACCESS,
                                            SERVICE_WIN32_OWN_PROCESS | SERVICE_INTERACTIVE_PROCESS,
                                            SERVICE_AUTO_START,
                                            SERVICE_ERROR_NORMAL,
                                            svcPath,
                                            null,
                                            0,
                                            null,
                                            null,
                                            null);
                if (sv_handle.ToInt32() == 0)
                {
                    throw new ApplicationException(String.Format("CreateService returned 0: GetLastError = {0}", GetLastError()));
                }
            }
            finally
            {
                if (sc_handle.ToInt32() != 0)
                {
                    CloseServiceHandle(sc_handle);
                }
                if (sv_handle.ToInt32() != 0)
                {
                    CloseServiceHandle(sv_handle);
                }
            }
        }
 
        /// <summary>
        /// This method uninstalls the service from the service conrol manager.
        /// </summary>
        /// <param name="svcName">Name of the service to uninstall.</param>
        public static void UnInstallService(string machineName, string svcName)
        {
            int GENERIC_WRITE = 0x40000000;

            IntPtr sc_handle = new IntPtr(0);
            IntPtr sv_handle = new IntPtr(0);

            try
            {

                sc_handle = OpenSCManager(machineName,
                                               null,
                                               GENERIC_WRITE);
                if (sc_handle.ToInt32() == 0)
                {
                    throw new ApplicationException(String.Format("OpenSCManager returned 0: GetLastError = {0}", GetLastError()));
                }

                int DELETE = 0x10000;
                sv_handle = OpenService(sc_handle,
                                               svcName,
                                               DELETE);

                if (sv_handle.ToInt32() == 0)
                {
                    throw new ApplicationException(String.Format("OpenService returned 0: GetLastError = {0}", GetLastError()));
                }

                if (DeleteService(sv_handle) == 0)
                {
                    throw new ApplicationException(String.Format("DeleteService failed to uninstall service: GetLastError() = {0}", GetLastError()));
                }
            }
            finally
            {
                if (sc_handle.ToInt32() != 0)
                {
                    CloseServiceHandle(sc_handle);
                }
                if (sv_handle.ToInt32() != 0)
                {
                    CloseServiceHandle(sv_handle);
                }
            }
        }

    }
}
