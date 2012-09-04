// Copyright 2012 Peter Potrebic.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CampfireAPI;
using System.Diagnostics;
using System.Reflection;
using System.IO;

namespace NotifierLibrary
{
    public class MessageProcessor
    {

        delegate void DoWork(CampfireState campfireInfo, ICampfireAPI api);
        class WorkerInfo
        {
            public DoWork WorkFunc;
            public int SleepSeconds;
            public bool SleepAtStart;
            public bool CreateEventLogEntryIfNotConfigured;
            public CampfireState CampfireInfo;
            public ICampfireAPI API;
        }

        private CampfireState CampfireInfo;
        private List<Thread> WorkerThreads;
        private volatile bool StopProcessing = false;

        /// <summary>
        /// Several duties. Starts a thread which looks for new rooms every X minutes.
        /// Starts a thread which read messages for rooms.
        /// </summary>
        public MessageProcessor(CampfireState campfireInfo)
        {
            this.CampfireInfo = campfireInfo;
            this.WorkerThreads = new List<Thread>();
        }

        /// <summary>
        /// main entry to where all the work happens
        /// </summary>
        /// <param name="block"></param>
        public void Run(bool block)
        {
            // 3 worker threads do all the smoke signal work
            WorkerThreads.Add(InitializeAddRemoveRoomWorker());
            WorkerThreads.Add(InitializeUserChangesWorker());
            WorkerThreads.Add(InitializeNewMessagesForAllRoomsWorker());

            if (block)
            {
                foreach (Thread th in WorkerThreads)
                {
                    th.Join();
                }
            }
        }

        #region private worker thread methods

        private Thread InitializeAddRemoveRoomWorker()
        {
            return InitializeGenericWorker(Work_ScanForAddOrRemoveRooms, Settings1.Default.ScanForNewRoomIntervalSeconds, false);
        }

        private Thread InitializeUserChangesWorker()
        {
            return InitializeGenericWorker(Work_ScanForUserChanges, Settings1.Default.ScanForUserChangesIntervalSeconds, false);
        }

        private Thread InitializeNewMessagesForAllRoomsWorker()
        {
            return InitializeGenericWorker(Work_ProcessNewMessagesForAllRooms, Settings1.Default.ScanForNewMessagesIntervalSeconds, true);
        }

        private bool firstWorker = true;
        private Thread InitializeGenericWorker(DoWork workMethod, int SleepSeconds, bool sleepAtStart)
        {
            WorkerInfo workerInfo = new WorkerInfo();
            workerInfo.CampfireInfo = CampfireInfo;
            workerInfo.SleepSeconds = SleepSeconds;
            workerInfo.SleepAtStart = sleepAtStart;
            workerInfo.WorkFunc = workMethod;
            workerInfo.CreateEventLogEntryIfNotConfigured = firstWorker;
            firstWorker = false;
            workerInfo.API = new CampfireAPI.API(SmokeSignalConfig.Instance.CampfireName, SmokeSignalConfig.Instance.CampfireToken);

            return StartWorker(workerInfo);
        }

        private Thread StartWorker(WorkerInfo workerInfo)
        {
            ParameterizedThreadStart threadDelegate = new ParameterizedThreadStart(Worker);
            Thread thread = new Thread(threadDelegate);
            thread.Start((object)workerInfo);
            return thread;
        }

        private int delayWhileNotConfigedSeconds = 1;
        private int bmIndex = 0;
        private int bmCounter = 0;
        private int[] iterationsBeforeMessaging = { 1, 6, 6 * 5 /* 5 minutes */, 6 * 10 /* 10 min */, 6 * 20 /* 60 */, 6 * 60 };

        private void GenerateLogEntryIfAppropriate(WorkerInfo workerInfo)
        {
            if (!workerInfo.CreateEventLogEntryIfNotConfigured) return;
            
            bmCounter++;
            if (bmCounter >= iterationsBeforeMessaging[bmIndex])
            {
                bmIndex = (bmIndex + 1) % iterationsBeforeMessaging.Length;

                string myConfigPath = Assembly.GetExecutingAssembly().Location;
                string myDir = Path.GetDirectoryName(myConfigPath);
                string setupPath = Path.Combine(myDir, "SmokeSignalSetup.exe");
                Utils.TraceMessage(TraceLevel.Warning, string.Format(
                    "Smoke Signal is not configured, please run the Smoke Signal Startup app - {0}",
                    setupPath));
            }
        }

        private void WaitUntilConfigured(WorkerInfo workerInfo)
        {
            while (!SmokeSignalConfig.Instance.IsSmokeSignalConfiged())
            {
                GenerateLogEntryIfAppropriate(workerInfo);
                System.Threading.Thread.Sleep(delayWhileNotConfigedSeconds * 1000);
            }

            if (workerInfo.CreateEventLogEntryIfNotConfigured)
            {
                EventLog theEventLog = new System.Diagnostics.EventLog();
                theEventLog.Log = "Application";
                theEventLog.Source = "SmokeSignal";
                theEventLog.WriteEntry("Smoke Signal is configured and ready to do its thing");
            }
        }

        /// <summary>
        /// The generic "worker" thread. Executes various tasks periodically
        /// </summary>
        /// <param name="workerInfoObject"></param>
        private void Worker(object workerInfoObject)
        {
            WorkerInfo workerInfo = (WorkerInfo)workerInfoObject;

            WaitUntilConfigured(workerInfo);

            try
            {
                if (workerInfo.SleepAtStart)
                {
                    // sleep a little at start
                    System.Threading.Thread.Sleep(FuzzValue(workerInfo.SleepSeconds / 4) * 1000);
                }

                while (true)
                {
                    try
                    {
                        workerInfo.WorkFunc(workerInfo.CampfireInfo, workerInfo.API);
                    }
                    catch (System.Net.WebException webEx)
                    {
                        Utils.TraceException(TraceLevel.Warning, webEx, string.Format("HandledException in Work Thread: {0}. Thread will continue.", workerInfo.WorkFunc.Method.Name));
                    }

                    if (StopProcessing) break;
                    System.Threading.Thread.Sleep(FuzzValue(workerInfo.SleepSeconds) * 1000);
                    if (StopProcessing) break;
                }
            }
            catch (Exception ex)
            {
                Utils.TraceException(TraceLevel.Error, ex, string.Format("UnHandledException in Work Thread: {0}. Thread aborting", workerInfo.WorkFunc.Method.Name));
                // continuing here will cause this thread to die
            }
        }
        #endregion

        #region The Bodies of various Worker Threads

        private static void TriggerNotification(CampfireState campfireInfo, CampfireState.UserReference userRef)
        {
            if ((userRef.TargetUser != null) && (userRef.Room != null))
            {
                string email = userRef.TargetUser.SmokeEmail;

                Utils.TraceVerboseMessage(string.Format("Sending notification: User: {0}, Room: {1}, SmokeEmail: {2}",
                    userRef.TargetUser.Name, userRef.Room.Name, userRef.TargetUser.SmokeEmail));

                string userName = (userRef.SourceUser != null) ? userRef.SourceUser.Name : "guest";

                campfireInfo.Notifier.Doit(userRef.TargetUser.Name, userRef.Room.Name, userRef.Room.Id, email,
                    SmokeSignalConfig.Instance.CampfireName, userName, userRef.NearByText);
            }
        }

        private static void QueueMessage(CampfireState campfireInfo, Message msg)
        {
            if (msg.Type == Message.MType.TextMessage)
            {
                // only process messages posted within the last 1 hour
                if (msg.PostedAt >= DateTime.Now.AddHours(-1))
                {
                    // put msg into the Text Queue
                    Utils.TraceVerboseMessage(msg, "Msg");
                    campfireInfo.QueueMessage(msg);
                }
            }
            else if (msg.Type == Message.MType.EnterMessage)
            {
                // put msg into the Enter Queue
                Utils.TraceVerboseMessage(msg, "Enter");
                campfireInfo.QueueMessage(msg);
            }
        }

        /// <summary>
        /// Retrieve all 'new' messages from each room. And then "process" them.
        /// </summary>
        /// <param name="campfireInfo"></param>
        /// <param name="api"></param>
        private static void Work_ProcessNewMessagesForAllRooms(CampfireState campfireInfo, ICampfireAPI api)
        {
            /*
             * 2 phases:
             *  - fetch all new message for all the rooms. Plase messages into appropriate queue
             *  - Process all those queue up messages, in order
             *  
             * Structuring this as 2 phases in case they are even executes by different threads. E.g. a thread for each room "waiting" on messages.
             */
            ProcessMessages_FetchNewMessages(campfireInfo, api);
            ProcessMessages_ProcessQueuedMessages(campfireInfo, api);
        }

        private static void ProcessMessages_FetchNewMessages(CampfireState campfireInfo, ICampfireAPI api)
        {
            IList<CampfireState.RoomInfo> rooms = campfireInfo.Rooms;

            foreach (CampfireState.RoomInfo room in rooms)
            {
                List<Message> msgs;
                try
                {
                    msgs = api.RecentMessages(room.Id, room.LastMessageId, Message.MType.TextMessage | Message.MType.EnterMessage);
                }
                catch (System.Net.WebException ex)
                {
                    // oops. Let's break out and then try again later
                    Utils.TraceException(TraceLevel.Warning, ex, "Exception calling API.RecentMessages");
                    break;
                }

                int lastMsgId = 0;
                foreach (Message msg in msgs)
                {
                    lastMsgId = msg.Id;
                    QueueMessage(campfireInfo, msg);
                }

                if (lastMsgId != 0)
                {
                    // remember that we've now processed through this message Id. So next time we're only fetch newer messages
                    campfireInfo.UpdateLastMessageId(room.Id, lastMsgId);
                }
            }
        }
        
        private static void ProcessMessages_ProcessQueuedMessages(CampfireState campfireInfo, ICampfireAPI api)
        {
            /*
             For each message that comes from any room:
                 Remove any entry in “PendingNotification” collection where PendingNotification.user is message.author (and room's match)
                     (e.g he’s already participating after being called out so no need to notify)
                 If there’s a User Reference match
                     Enter (user, time, room) into a PendingNotification collection 
                         (unless user/room already exists in collection)
                    
             For each entry in PendingNotification
                 If entry.timestamp  is < Now – entry.user.ConfiguredDelay then
                      Send that user a notification that he’s been called out in entry.room
            */
            
            Message msg;
            while ((msg = campfireInfo.PopMessage()) != null)
            {
                CheckForUnknownUserOrRoom(campfireInfo, msg, api);

                if (msg.Type == Message.MType.EnterMessage)
                {
                    ProcessEnterMessage(campfireInfo, msg, api);
                }
                else if (msg.Type == Message.MType.TextMessage)
                {
                    ProcessTextMessage(campfireInfo, msg, api);
                }

            }

            TriggerNotifications(campfireInfo);         // Now look to fire any pending notifications
        }

        private static void CheckForUnknownUserOrRoom(CampfireState campfireInfo, Message msg, ICampfireAPI api)
        {
            // If this userId isn't already known...
            if (!campfireInfo.IsUserKnown(msg.UserId))
            {
                // fetch all the user Info and then add
                User newUser = api.GetUser(msg.UserId);
                if ((newUser != null) && (newUser.Type == User.UserType.Member) && !string.IsNullOrEmpty(newUser.Name) && !string.IsNullOrEmpty(newUser.Email))
                {
                    Utils.TraceVerboseMessage(string.Format("Found a new User: {0}, Id: {1}", newUser.Name, newUser.Id));
                    campfireInfo.AddUser(newUser.Id, newUser.Name, newUser.Email, string.Empty);
                }
            }

            // If this roomId isn't already known...
            if (!campfireInfo.IsRoomKnown(msg.RoomId))
            {
                // fetch all the user Info and then add
                Room newRoom = api.GetRoom(msg.RoomId);
                if (newRoom != null)
                {
                    Utils.TraceVerboseMessage(string.Format("Found a new Room: {0}, Id: {1}", newRoom.Name, newRoom.Id));
                    campfireInfo.AddRoom(newRoom.Id, newRoom.Name, 0);
                }
            }
        }

        private static void TriggerNotifications(CampfireState campfireInfo)
        {
            IList<CampfireState.PendingNotify> pending = campfireInfo.PendingNotifications;

            foreach (CampfireState.PendingNotify pn in pending)
            {
                if (pn.TriggerTime < DateTime.Now)
                {
                    TriggerNotification(campfireInfo, pn.Reference);                                            // send notification
                    campfireInfo.RemovePendingNotification(pn.Reference.TargetUser.Id, pn.Reference.Room.Id);   // remove from pending list
                }
            }
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            System.Text.RegularExpressions.Regex _Regex = new System.Text.RegularExpressions.Regex(strRegex);
            if (_Regex.IsMatch(email))
                return (true);
            else
                return (false);
        }

        private static void ProcessSmokeSignalCommands(int userId, CampfireState.UserReference ri)
        {
            // ??? Should a syntax error generate an email to the user explaining the problem?

            int pos = ri.NearByText.IndexOf(':');
            if (pos < 0) return;

            string cmd = ri.NearByText.Remove(0, pos + 1);
            string[] args = cmd.Split('=');

            switch (args[0].ToLowerInvariant())
            {
                case "help":
                    CampfireState.Instance.Notifier.SendHelp(ri.SourceUser.Name, ri.SourceUser.EmailAddress);
                    break;
                case "settings":
                    CampfireState.Instance.Notifier.SendSettings(ri.SourceUser, false);
                    break;
                case "delay":
                    int newDelay;
                    string newDelayStr = "";
                    if (args.Length > 2) return;
                    if (args.Length == 2) newDelayStr = args[1];
                    if (string.IsNullOrEmpty(newDelayStr)) newDelayStr = "-1";

                    if (Int32.TryParse(newDelayStr, out newDelay))
                    {
                        ri.SourceUser = CampfireState.Instance.UpdateUser(userId, newDelay);
                        CampfireState.Instance.Notifier.SendSettings(ri.SourceUser, true);
                    }
                    break;
                case "altemail":
                    string newEmail = null;
                    if (args.Length > 2) return;
                    if (args.Length == 2) newEmail = args[1];

                    if (!string.IsNullOrEmpty(newEmail) && !IsValidEmail(newEmail))
                    {
                        return;
                    }

                    ri.SourceUser = CampfireState.Instance.UpdateAltEmail(ri.SourceUser.Id, newEmail);
                    CampfireState.Instance.Notifier.SendSettings(ri.SourceUser, true);
                    break;
                default:
                    break;
            }
        }

        private static void ProcessTextMessage(CampfireState campfireInfo, Message msg, ICampfireAPI api)
        {
            // The person that posted this message... If they have a pending notification in the room... then cancel it... they've spoken
            campfireInfo.RemovePendingNotification(msg.UserId, msg.RoomId, msg.PostedAt, true);

            IList<CampfireState.UserInfo> allUsers = campfireInfo.Users;

            IList<CampfireState.UserReference> lazyNotificationUsers;
            IList<CampfireState.UserReference> immediateNotificationUsers;
            IList<CampfireState.UserReference> smokeSignalReferences;
            Utils.FindUserReferences(msg.Body, allUsers, out lazyNotificationUsers, out immediateNotificationUsers, out smokeSignalReferences);

            CampfireState.UserInfo source = allUsers.FirstOrDefault(u => u.Id == msg.UserId);
            CampfireState.RoomInfo room = CampfireState.Instance.Rooms.FirstOrDefault(r => r.Id == msg.RoomId);

            // special smoke signal commands only make sense if a legitimate user issued them
            if (source != null)
            {
                foreach (CampfireState.UserReference ri in smokeSignalReferences)
                {
                    ri.SourceUser = source;
                    ri.Room = room;
                    ProcessSmokeSignalCommands(source.Id, ri);
                }
            }

            foreach (CampfireState.UserReference ri in immediateNotificationUsers)
            {
                ri.SourceUser = source;
                ri.Room = room;
                campfireInfo.AddPendingNotification(ri, msg.PostedAt.AddSeconds(0));
            }
            foreach (CampfireState.UserReference ri in lazyNotificationUsers)
            {
                ri.SourceUser = source;
                ri.Room = room;

                int delay = ri.TargetUser.DelayInMinutes > 0 ? ri.TargetUser.DelayInMinutes : SmokeSignalConfig.Instance.DelayBeforeSmokeSignalInMinutes;
                campfireInfo.AddPendingNotification(ri, msg.PostedAt.AddSeconds(delay * 60));
            }
        }

        private static void ProcessEnterMessage(CampfireState campfireInfo, Message msg, ICampfireAPI api)
        {
            // remove any pending notifications for this user in the room in which this Enter message appeared
            campfireInfo.RemovePendingNotification(msg.UserId, msg.RoomId, msg.PostedAt, true);
        }

        /// <summary>
        /// This is called every so often to update the known list of rooms
        /// </summary>
        /// <param name="campfireInfo"></param>
        private static void Work_ScanForAddOrRemoveRooms(CampfireState campfireInfo, ICampfireAPI api)
        {
            List<int> roomIds = campfireInfo.RoomIds();

            List<Room> rooms = api.Rooms();

            foreach (Room r in rooms)
            {
                // since r.id exists, remove it from 'roomIds'
                roomIds.Remove(r.Id);
            }
            List<int> roomsToDelete = roomIds;
            foreach (int roomToDeleteId in roomsToDelete)
            {
                campfireInfo.DeleteRoom(roomToDeleteId);
            }

            campfireInfo.AddRooms(rooms);
        }

        /// <summary>
        /// This is called every so often to check if users Names or Email address has changed, or User has been Deleted
        /// </summary>
        /// <param name="campfireInfo"></param>
        private static void Work_ScanForUserChanges(CampfireState campfireInfo, ICampfireAPI api)
        {
            // iterate of all known users... and see if name or email address has changed
            // Must do this in a thread safe fashion by first getting all the ids.
            List<int> userIds = campfireInfo.UserIds();

            foreach (int uid in userIds)
            {
                User user = api.GetUser(uid);
                if (user != null && !string.IsNullOrEmpty(user.Name) && !string.IsNullOrEmpty(user.Email))
                {
                    campfireInfo.UpdateUser(user.Id, user.Name, user.Email);
                }
                else
                {
                    campfireInfo.DeleteUser(uid);
                }
            }
        }

        #endregion

        #region private helpers

        private static Random rand = new Random();
        private static int FuzzValue(int val)
        {
            int fuzz = rand.Next(val / 10);
            fuzz = fuzz - val / 20;
            return val + fuzz;
        }

        #endregion
    }
}
