// Copyright 2012 Peter Potrebic.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NotifierLibrary;

namespace TestCampfireAPI
{
    class MockNotifier : INotifier
    {
        public static List<string> MessagesSentTo;

        public MockNotifier()
        {
            if (MessagesSentTo == null)
            {
            MessagesSentTo = new List<string>();
            }
        }

        #region INotifier Members

        public void SendSettings(CampfireState.UserInfo user, bool haveChanged)
        {
            LogSent(user.Name, null, "Settings-Email");
        }
        public void SendHelp(string userName, string emailAddr)
        {
            LogSent(userName, null, "Help-EMail");
        }
        public void SendWelcomeEmail(string userName, string emailAddr, string extraMessage)
        {
            LogSent(userName, null, "Welcome-Email");
        }

        public void Doit(string userName, string roomName, int roomId, string emailAddr, string campfireName, string requesterName, string context)
        {
            LogSent(userName, roomName);
        }
        public void LogSent(string userName, string roomName, string extra = null)
        {
            if (MessagesSentTo == null)
            {
                MessagesSentTo = new List<string>();
            }
            MessagesSentTo.Add(string.Format("{0}++{1}++{2}", userName, roomName ?? "", extra ?? ""));
        }

        #endregion
    }
}
