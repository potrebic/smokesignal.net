// Copyright 2012 Peter Potrebic.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Diagnostics;

namespace SmokeSignal
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {

            if (!EventLog.SourceExists("SmokeSignal"))
            {
                EventSourceCreationData cd = new EventSourceCreationData("SmokeSignal", "Application");

                EventLog.CreateEventSource(cd);
            }
 
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new SmokeSignal() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}
