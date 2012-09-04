// Copyright 2012 Peter Potrebic.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SmokeSignalSetup
{
    public class OnDelay
    {
        public delegate void VoidFunc();

        private static void DelayHook(Object myObject, EventArgs myEventArgs)
        {
            Timer timer = myObject as Timer;
            VoidFunc func = timer.Tag as VoidFunc;
            timer.Stop();

            func();
        }

        public static void InAMoment(VoidFunc func)
        {
            Do(func, 1);
        }

        public static void Do(VoidFunc func, int msDelay)
        {
            Timer delayTimer = new Timer();
            delayTimer.Tag = func;

            delayTimer.Tick += new EventHandler(DelayHook);

            delayTimer.Interval = msDelay;
            delayTimer.Start();
        }
    }
}
