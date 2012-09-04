// Copyright 2012 Peter Potrebic.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using CampfireAPI;
using System.Diagnostics;

namespace NotifierLibrary
{
    public class CampfireState : IXmlSerializable
    {
        #region Data Classes
        [Flags]
        public enum NotificationFlavor
        {
            Email = 1,
            SMS = 2,
        }

        public class PendingNotify
        {
            public UserReference Reference;
            public DateTime TriggerTime;

            public PendingNotify(UserReference reference, DateTime triggerTime)
            {
                this.Reference = reference;
                this.TriggerTime = triggerTime;
            }

            public override string ToString()
            {
                return string.Format("{0}: user:{1}, room:{2}", this.TriggerTime.ToShortTimeString(), this.Reference.TargetUser.Name, this.Reference.Room.Name);
            }
        }

        public class RoomInfo
        {
            public string Name { get; private set; }
            public int Id { get; private set; }
            public int LastMessageId { get; private set; }

            public RoomInfo(string name, int roomId, int lastMessageId)
            {
                this.Name = name;
                this.Id = roomId;
                this.LastMessageId = lastMessageId;
            }

            public override string ToString()
            {
                return string.Format("{0}: {1}, lastMsgId:{2}", this.Id, this.Name, this.LastMessageId);
            }
        }

        public class UserInfo
        {
            public UserInfo(int id, string name, string email, string altemail)
                : this(name, email, altemail, id)
            {
            }
            
            public UserInfo(string name, string email, string altemail, int id)
                : this(name, email, altemail, id, -1)
            {
            }

            public UserInfo(string name, string email, string altemail, int id, int delay)
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentNullException("UserInfo 'name' cannot be null or empty");
                }
                if (string.IsNullOrEmpty(email))
                {
                    throw new ArgumentNullException("UserInfo 'email' cannot be null or empty");
                }
                
                this.Name = name;
                this.EmailAddress = email;
                this.AltEmailAddress = (altemail ?? string.Empty);
                this.Id = id;
                this.possibleAbbreviations = BuildAbbreviations();

                if (delay < 1) delay = -1;
                else if (delay > 10) delay = 10;

                this.DelayInMinutes = delay;
            }

            public override string ToString()
            {
                return string.Format("{0}: {1}, {2}, {3}", this.Id, this.Name, this.EmailAddress, this.AltEmailAddress);
            }

            public string Name { get; private set; }
            public string EmailAddress { get; private set; }
            public string AltEmailAddress { get; private set; }
            public int Id { get; private set; }
            public int DelayInMinutes { get; private set; }

            // email to use for smoke signals
            public string SmokeEmail
            {
                get
                {
                    string email = !string.IsNullOrEmpty(this.AltEmailAddress) ? this.AltEmailAddress : this.EmailAddress;
                    return email;
                }
            }

            /// <summary>
            /// First Name + Last Initial. Like "JackN"
            /// </summary>
            public string NickName
            {
                get
                {
                    string[] parts = this.Name.Split(' ');
                    int lastIndex = parts.Length - 1;
                    string lastInitial = (lastIndex != 0) ? parts[lastIndex].Substring(0,1) : "";

                    string nickName = string.Format("{0}{1}", parts[0], lastInitial);
                    return nickName;
                }
            }

            // not persisted data
            private List<string> possibleAbbreviations;
            public IList<string> PossibleAbbreviations
            {
                get
                {
                    return this.possibleAbbreviations.AsReadOnly();
                }
            }

            private List<string> BuildAbbreviations()
            {
                char[] delimiterChars = { ' ', '\t' };
                string[] words = this.Name.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
                return Utils.BuildAbbreviations(words);
            }
        }

        public class UserReference
        {
            public UserReference(string text, CampfireState.UserInfo target)
            {
                this.TargetUser = target;
                this.NearByText = text;
            }

            public RoomInfo Room { get; set; }
            public UserInfo SourceUser { get; set; }
            public UserInfo TargetUser { get; set; }
            public string NearByText { get; set; }
        }

        #endregion

        #region static API

        private static string BackingStorePath;

        public static CampfireState Instance
        {
            get;
            private set;
        }

        public static bool Initialize(string storePath, INotifier notifier)
        {
            bool success = true;
            if (Instance == null)
            {
                BackingStorePath = string.Format(@"{0}\{1}", storePath, "CampfireInfo.xml");

                Instance = Restore(BackingStorePath);

                if (Instance == null)
                {
                    // Data in the xml file was corrupt/unreadable
                    success = false;
                    Instance = new CampfireState();
                }

                if (notifier == null)
                {
                    notifier = new Notifier();
                }

                Instance.Notifier = notifier;
            }
            return success;
        }

        private static CampfireState Restore(string fromPath)
        {
            CampfireState campInfo = null;
            bool error = false;

            if (!System.IO.File.Exists(fromPath))
            {
                campInfo = new CampfireState();
                campInfo.Save();
                return campInfo;
            }

            Exception caughtException = null;
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(fromPath);

                XmlSerializer ser = new XmlSerializer(typeof(CampfireState));
                using (StringReader reader = new StringReader(xmlDoc.OuterXml))
                {
                    campInfo = (CampfireState)ser.Deserialize(reader);
                    reader.Close();
                }
            }
            catch (XmlException ex) { caughtException = ex; }
            catch (IOException ex) { caughtException = ex; }
            catch (UnauthorizedAccessException ex) { caughtException = ex; }
            catch (System.Security.SecurityException ex) { caughtException = ex; }
            catch (InvalidOperationException ex) { caughtException = ex; }

            if (caughtException != null)
            {
                // xml file was corrupt. Make a backup and delete original
                Utils.TraceException(TraceLevel.Error, caughtException, string.Format("Campfile.xml was corrupt. Making a backup and starting with a new file."));
                error = Utils.SafeBackupAndDelete(fromPath);
            }

            return campInfo;
        }

        private static void SaveToStore(CampfireState state)
        {
            CampfireState.SaveToStore(state, BackingStorePath);
        }

        private static void SaveToStore(CampfireState state, string toPath)
        {
            if (!System.IO.File.Exists(toPath))
            {
                string dir = System.IO.Path.GetDirectoryName(toPath);
                if (!System.IO.Directory.Exists(dir))
                {
                    System.IO.Directory.CreateDirectory(dir);
                }
            }

            XmlSerializer ser = new XmlSerializer(typeof(CampfireState));

            System.IO.StreamWriter file = new System.IO.StreamWriter(toPath);

            ser.Serialize(file, state);
            file.Close();
        }

        #endregion

        #region persisted data

        /// <summary>
        /// (Thread safe) returns a read-only duplicate of the current list of Rooms
        /// </summary>
        public IList<RoomInfo> Rooms
        {
            get
            {
                lock (this.rooms)
                {
                    List<RoomInfo> dup = new List<RoomInfo>(this.rooms.Count);
                    dup.AddRange(this.rooms);
                    return dup.AsReadOnly();
                }
            }
        }
        private List<RoomInfo> rooms;

        /// <summary>
        /// (Thread safe) returns a read-only duplicate of the current list of Users
        /// </summary>
        public IList<UserInfo> Users
        {
            get
            {
                lock (this.users)
                {
                    List<UserInfo> dup = new List<UserInfo>(this.users.Count);
                    dup.AddRange(this.users);
                    return dup.AsReadOnly();
                }
            }
        }
        private List<UserInfo> users;

        #endregion

        #region transient data
        
        private Queue<Message> EnteredUserQueue;
        private Queue<Message> MessageQueue;
        public INotifier Notifier;

        /// <summary>
        /// (Thread safe) returns a read-only duplicate of the current list of PendingNotifications
        /// </summary>
        public IList<PendingNotify> PendingNotifications
        {
            get
            {
                lock (this.pendingNotifications)
                {
                    List<PendingNotify> dup = new List<PendingNotify>(this.pendingNotifications.Count);
                    dup.AddRange(this.pendingNotifications);
                    return dup.AsReadOnly();
                }
            }
        }
        private List<PendingNotify> pendingNotifications;

        #endregion

        #region public API

        private CampfireState()
        {
            this.users = new List<UserInfo>();
            this.rooms = new List<RoomInfo>();

            this.EnteredUserQueue = new Queue<Message>();
            this.MessageQueue = new Queue<Message>();
            this.pendingNotifications = new List<PendingNotify>();
            this.Notifier = null;
        }

        /// <summary>
        /// (Thread safe) Adds a "pending notification" for the given "user reference", set to trigger at the specified time.
        /// If there's already a pending notification for this user/room then nothing happens
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="triggerTime"></param>
        public void AddPendingNotification(UserReference reference, DateTime triggerTime)
        {
            lock (this.pendingNotifications)
            {
                PendingNotify pending = pendingNotifications.FirstOrDefault(pn => pn.Reference.TargetUser.Id == reference.TargetUser.Id 
                    && pn.Reference.Room.Id == reference.Room.Id);
                if (pending != null)
                {
                    // already a pending notification for this user/room. Compare 'Trigger Times' to determine what to do.
                    if (triggerTime < pending.TriggerTime)
                    {
                        // move up the trigger time
                        pending.TriggerTime = triggerTime;
                    }
                    return;
                }

                Utils.TraceVerboseMessage(string.Format("Add Pending Notification: User: {0}, Room: {1}, triggerAt: {2}",
                    reference.TargetUser.NickName, reference.Room.Name, triggerTime.ToString()));

                PendingNotify notifier = new PendingNotify(reference, triggerTime);
                pendingNotifications.Add(notifier);
            }
        }

        /// <summary>
        /// (Thread safe) Remove a pending notification for the given user/room. Harmless if user/room aren't in pending list
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roomId"></param>
        public void RemovePendingNotification(int userId, int roomId)
        {
            RemovePendingNotification(userId, roomId, DateTime.MinValue, false);
        }

        /// <summary>
        /// (Thread safe) Remove a pending notification for the given user/room. Harmless if user/room aren't in pending list
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roomId"></param>
        /// <param name="timeStamp">when did the event occur that is causing this remove</param>
        /// <param name="logIt">whether or not to generate a log entry</param>
        public void RemovePendingNotification(int userId, int roomId, DateTime timeStamp, bool logIt)
        {
            lock (this.pendingNotifications)
            {
                PendingNotify pn = pendingNotifications.FirstOrDefault(p => p.Reference.TargetUser.Id == userId && p.Reference.Room.Id == roomId);
                if (pn != null)
                {
                    if (logIt)
                    {
                        Utils.TraceVerboseMessage(string.Format("Remove Pending Notification: User: {0}, Room: {1}, Event: {2}",
                            pn.Reference.TargetUser.NickName, pn.Reference.Room.Name, timeStamp));
                    }
                    pendingNotifications.Remove(pn);
                }
            }
        }

        /// <summary>
        /// (Thread safe) add a new Text/Paste/Enter message into a queue, the queue of waiting to be processed messages
        /// </summary>
        /// <param name="msg"></param>
        public void QueueMessage(Message msg)
        {
            lock (this.MessageQueue)
            {
                this.MessageQueue.Enqueue(msg);
            }
        }

        /// <summary>
        /// (Thread safe) fetch the next waiting to be processed message
        /// </summary>
        /// <returns></returns>
        public Message PopMessage()
        {
            lock (this.MessageQueue)
            {
                if (this.MessageQueue.Count() == 0)
                    return null;
                return this.MessageQueue.Dequeue();
            }
        }

        private object saveLock = new object();
        /// <summary>
        /// (Thread Safe) Save state to persistant store
        /// </summary>
        public void Save()
        {
            lock (this.saveLock)
            {
                CampfireState.SaveToStore(this);
            }
        }

        private object nameTokenLock = new object();
        
        /// <summary>
        /// (Thread Safe)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="token"></param>
        public void SetNameAndToken(string name, string token)
        {
            lock (this.nameTokenLock)
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentNullException("CampfireInfo 'name' cannot be null or empty");
                }
                if (string.IsNullOrEmpty(token))
                {
                    throw new ArgumentNullException("CampfireInfo 'token' cannot be null or empty");
                }

                this.Save();
            }
        }

        /// <summary>
        /// (Thread Safe) add a new room
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="lastMessageId"></param>
        /// <returns>true if a room was added</returns>
        public bool AddRoom(int id, string name, int lastMessageId)
        {
            bool added = false;
            lock (this.rooms)
            {
                if (AddOrReplaceRoom(id, name, 0))
                {
                    this.Save();
                    added = true;
                }
            }
            return added;
        }

        /// <summary>
        /// (Thread Safe) add any new rooms from the given list
        /// </summary>
        /// <param name="rooms"></param>
        /// <returns>true if a room(s) was added</returns>
        public bool AddRooms(List<CampfireAPI.Room> newRooms)
        {
            bool modified = false;

            lock (this.rooms)
            {
                newRooms.ForEach(
                  delegate(CampfireAPI.Room room)
                  {
                      if (AddOrReplaceRoom(room.Id, room.Name, 0))
                      {
                          modified = true;
                      }
                  });

                if (modified)
                {
                    this.Save();
                }
            }

            return modified;
        }

        /// <summary>
        /// (Thread Safe) update the lastMessage read for the specific room
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="lastMessageId"></param>
        public void UpdateLastMessageId(int roomId, int lastMessageId)
        {
            lock (this.rooms)
            {
                RoomInfo existing = this.rooms.Find(r => r.Id == roomId);
                if ((existing != null) && (lastMessageId != existing.LastMessageId))
                {
                    ReplaceRoom(existing, existing.Name, lastMessageId);
                    this.Save();
                }
            }
        }

        /// <summary>
        /// (Thread Safe) delete the specified room
        /// </summary>
        /// <param name="id"></param>
        /// <returns>true if a room was deleted</returns>
        public bool DeleteRoom(int id)
        {
            bool wasDeleted = false;
            lock (this.rooms)
            {
                RoomInfo existing = this.rooms.Find(r => r.Id == id);
                if (existing != null)
                {
                    Utils.TraceVerboseMessage(string.Format("A room was deleted: Room: {0} RoomId: {1}", existing.Name, existing.Id));
                    this.rooms.Remove(existing);
                    this.Save();
                    wasDeleted = true;
                }
            }
            return wasDeleted;
        }

        public bool IsUserKnown(int id)
        {
            lock (this.users)
            {
                return this.users.Any(u => u.Id == id);
            }
        }

        public bool IsRoomKnown(int id)
        {
            lock (this.rooms)
            {
                return this.rooms.Any(r => r.Id == id);
            }
        }

        /// <summary>
        /// (Thread Safe) add a new user with the specified id, name, emails, smsAddr
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <param name="smsAddr"></param>
        public void AddUser(int id, string name, string email, string smsAddr)
        {
            lock (this.users)
            {
                // ignore adds of the same user (via Id)
                if (!this.users.Any(u => u.Id == id))
                {
                    _AddUser(id, name, email, smsAddr);
                    this.Save();
                }
            }
        }

        private void _AddUser(int id, string name, string email, string smsAddr)
        {
            UserInfo u = new UserInfo(id, name, email, smsAddr);
            this.users.Add(u);
            if (SmokeSignalConfig.Instance.SendNewUserEmail)
            {
                Notifier.SendWelcomeEmail(u.Name, u.SmokeEmail, SmokeSignalConfig.Instance.ExtraEmailMessage);
            }
        }
        /// <summary>
        /// (Thread Safe) add any new rooms from the given list
        /// </summary>
        /// <param name="rooms"></param>
        public void AddUsers(List<CampfireAPI.User> newUsers)
        {
            bool modified = false;

            lock (this.users)
            {
                newUsers.ForEach(
                  delegate(CampfireAPI.User user)
                  {
                      if (AddOrReplaceUser(user.Id, user.Name, user.Email))
                      {
                          modified = true;
                      }
                  });

                if (modified)
                {
                    this.Save();
                }
            }
        }

        /// <summary>
        /// (Thread Save) update the name/email of a given user - if there's a change
        /// </summary>
        /// <param name="id">id of user that has changed</param>
        /// <param name="name">A potentially different user name</param>
        /// <param name="email">A potentially different email address</param>
        public UserInfo UpdateUser(int id, string name, string email)
        {
            lock (this.users)
            {
                UserInfo existing = this.users.Find(u => u.Id == id);
                UserInfo newUser = existing;
                if (existing != null && (!string.Equals(existing.Name, name) || !string.Equals(existing.EmailAddress, email)))
                {
                    Utils.TraceVerboseMessage(string.Format("A user's specs were changed: oldName: {0}, newName: {1}, oldEmail: {2}, newEmail: {3}",
                        existing.Name, name, existing.EmailAddress, email));
                    newUser = new UserInfo(name, email, existing.AltEmailAddress, existing.Id, existing.DelayInMinutes);
                    this.ReplaceUser(existing, newUser);
                    this.Save();
                }
                return newUser;
            }
        }

        /// <summary>
        /// (Thread Save) update the altEmail of a given user - if there's a change
        /// </summary>
        /// <param name="id">id of user that has changed</param>
        /// <param name="altEmail">A potentially different email address</param>
        public UserInfo UpdateAltEmail(int id, string altEmail)
        {
            lock (this.users)
            {
                UserInfo existing = this.users.Find(u => u.Id == id);
                UserInfo newUser = existing;
                if (existing != null && !string.Equals(existing.AltEmailAddress, altEmail))
                {
                    Utils.TraceVerboseMessage(string.Format("A user's AltEmail was changed: name: {0}, old AltEmail: {1}, new AltEmail: {2}",
                        existing.NickName, existing.AltEmailAddress, altEmail));
                    newUser = new UserInfo(existing.Name, existing.EmailAddress, altEmail, existing.Id, existing.DelayInMinutes);
                    this.ReplaceUser(existing, newUser);
                    this.Save();
                }
                return newUser;
            }
        }

        /// <summary>
        /// (Thread Save) update the "delay in minutes" before email of a given user - if there's a change
        /// </summary>
        /// <param name="id">id of user that has changed</param>
        /// <param name="delay">A potentially different 'delay' before sending a smoke signal</param>
        public UserInfo UpdateUser(int id, int delayInMinutes)
        {
            lock (this.users)
            {
                UserInfo existing = this.users.Find(u => u.Id == id);
                UserInfo newUser = existing;
                if (existing != null && (existing.DelayInMinutes != delayInMinutes))
                {
                    Utils.TraceVerboseMessage(string.Format("A user's 'delay' was changed: User: {0}, oldDelay: {1}, newDelay: {2}",
                        existing.NickName, existing.DelayInMinutes, delayInMinutes));
                    newUser = new UserInfo(existing.Name, existing.EmailAddress, existing.AltEmailAddress, existing.Id, delayInMinutes);
                    this.ReplaceUser(existing, newUser);
                    this.Save();
                }
                return newUser;
            }
        }

        /// <summary>
        /// (Thread Safe) delete the specified user
        /// </summary>
        /// <param name="id"></param>
        public void DeleteUser(int id)
        {
            lock (this.users)
            {
                UserInfo existing = this.users.Find(u => u.Id == id);
                if (existing != null)
                {
                    Utils.TraceVerboseMessage(string.Format("User deleted: UserId: {0}, Name: {1}", existing.Id, existing.Name));
                    this.users.Remove(existing);
                    this.Save();
                }
            }
        }

        /// <summary>
        /// (Thread Safe) gets a list of ids for all the known users
        /// </summary>
        /// <returns></returns>
        public List<int> UserIds()
        {
            List<int> ids = new List<int>();
            lock (this.users)
            {
                foreach (UserInfo ui in this.users)
                {
                    ids.Add(ui.Id);
                }
            }
            return ids;
        }
        /// <summary>
        /// (Thread Safe) gets a list of ids for all the known Room
        /// </summary>
        /// <returns></returns>
        public List<int> RoomIds()
        {
            List<int> ids = new List<int>();
            lock (this.rooms)
            {
                foreach (RoomInfo ri in this.rooms)
                {
                    ids.Add(ri.Id);
                }
            }
            return ids;
        }

        #endregion

        #region private helpers

        private bool AddOrReplaceRoom(int id, string name, int lastMessageId)
        {
            bool modified = false;
            RoomInfo existing = this.rooms.Find(r => r.Id == id);

            if (existing == null)
            {
                modified = true;
                Utils.TraceVerboseMessage(string.Format("A room was added: Room: {0}, RoomId: {1}", name, id));
                this.rooms.Add(new RoomInfo(name, id, lastMessageId));
            }
            else if (!string.Equals(name, existing.Name))
            {
                Utils.TraceVerboseMessage(string.Format("A room's name changed: oldName: {0} newName: {1} RoomId: {2}", existing.Name, name, existing.Id));
                ReplaceRoom(existing, name, existing.LastMessageId);
                modified = true;
            }
            return modified;
        }

        private bool AddOrReplaceUser(int id, string name, string email)
        {
            bool modified = false;
            UserInfo existing = this.users.Find(u => u.Id == id);

            if (existing == null)
            {
                modified = true;
                _AddUser(id, name, email, string.Empty);
            }
            else if (!string.Equals(name, existing.Name))
            {
                UserInfo newUser = new UserInfo(name, email, existing.AltEmailAddress, existing.Id, existing.DelayInMinutes);
                ReplaceUser(existing, newUser);
                modified = true;
            }
            return modified;
        }

        private void ReplaceRoom(RoomInfo old, string name, int lastMessageId)
        {
            // to maintain the read-only semantics of RoomInfo... can't modify it. Must delete old, and add new.
            if (this.rooms.Remove(old))
            {
                RoomInfo newRoom = new RoomInfo(name, old.Id, lastMessageId);
                this.rooms.Add(newRoom);
            }
            else
            {
                throw new ApplicationException("oops");
            }
        }

        private void ReplaceUser(UserInfo oldUser, UserInfo newUser)
        {
            // to maintain the read-only semantics of UserInfo... can't modify it. Must delete old, and add new.
            if (this.users.Remove(oldUser))
            {
                this.users.Add(newUser);
            }
            else
            {
                throw new ApplicationException("oops");
            }
        }

        #endregion

        #region IXmlSerializable

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        private void ReadRoomsXml(XmlReader reader)
        {
            reader.ReadStartElement();      // read start of "Rooms" element

            if (!reader.EOF && string.Equals("RoomInfo", reader.Name))
            {
                // read an arbitrary number of "RoomInfo" nodes
                while (!reader.EOF && string.Equals("RoomInfo", reader.Name))
                {
                    reader.ReadStartElement();
                    string name = reader.ReadElementContentAsString("Name", "");
                    int id = reader.ReadElementContentAsInt("Id", "");
                    int lastMsgId = reader.ReadElementContentAsInt("LastMessageId", "");

                    RoomInfo r = new RoomInfo(name, id, lastMsgId);
                    this.rooms.Add(r);

                    reader.ReadEndElement();
                }
                reader.ReadEndElement();        // read end of "Rooms" element
            }
        }

        private void ReadUsersXml(XmlReader reader)
        {
            reader.ReadStartElement();      // read start of "Users" element

            if (!reader.EOF && string.Equals("UserInfo", reader.Name))
            {
                // read an arbitrary number of "UserInfo" nodes
                ReadAllUsers(reader);
                reader.ReadEndElement();        // read end of "Users" element
            }
        }

        private void ReadAllUsers(XmlReader reader)
        {
            while (!reader.EOF && string.Equals("UserInfo", reader.Name))
            {
                reader.ReadStartElement();

                string name = null;
                string email = null;
                string altemail = null;
                int id = -1;
                int delay = -1;

                while (true)
                {
                    if (reader.NodeType == XmlNodeType.EndElement)
                    {
                        break;
                    }
                    switch (reader.LocalName)
                    {
                        case "Id":
                            id = reader.ReadElementContentAsInt("Id", "");
                            break;
                        case "Name":
                            name = reader.ReadElementContentAsString("Name", "");
                            break;
                        case "Email":
                            email = reader.ReadElementContentAsString("Email", "");
                            break;
                        case "AltEmail":
                            altemail = reader.ReadElementContentAsString("AltEmail", "");
                            break;
                        case "Delay":
                            delay = reader.ReadElementContentAsInt("Delay", "");
                            break;
                        default:
                            reader.ReadElementContentAsString(reader.LocalName, "");
                            break;
                    }
                }

                if (id == -1 || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name))
                {
                    throw new ApplicationException("Invalid CampfireInfo.xml, missing Id, or Email, or Name for a User");
                }

                if (delay < 1) delay = -1;
                else if (delay > 10) delay = 10;

                UserInfo ui = new UserInfo(name, email, altemail, id, delay);
                this.users.Add(ui);

                reader.ReadEndElement();
            }
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();

            ReadRoomsXml(reader);
            ReadUsersXml(reader);

            reader.ReadEndElement();
        }

        private void WriteUsersXml(XmlWriter writer)
        {
            writer.WriteStartElement("Users");

            foreach (UserInfo ui in this.Users)
            {
                writer.WriteStartElement("UserInfo");
                writer.WriteElementString("Name", ui.Name);
                writer.WriteElementString("Email", ui.EmailAddress);
                writer.WriteElementString("AltEmail", ui.AltEmailAddress);
                writer.WriteElementString("Id", ui.Id.ToString());
                if (ui.DelayInMinutes > 0)
                {
                    writer.WriteElementString("Delay", ui.DelayInMinutes.ToString());
                }
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        private void WriteRoomsXml(XmlWriter writer)
        {
            writer.WriteStartElement("Rooms");

            foreach (RoomInfo room in this.Rooms)
            {
                writer.WriteStartElement("RoomInfo");
                writer.WriteElementString("Name", room.Name);
                writer.WriteElementString("Id", room.Id.ToString());
                writer.WriteElementString("LastMessageId", room.LastMessageId.ToString());
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            WriteRoomsXml(writer);
            WriteUsersXml(writer);
        }

        #endregion
    }
}
