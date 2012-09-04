// Copyright 2012 Peter Potrebic.  All rights reserved.

using NotifierLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using CampfireAPI;
using System.Collections.Generic;
using System;

namespace TestCampfireAPI
{
    
    
    /// <summary>
    ///This is a test class for CampfireInfoTest and is intended
    ///to contain all CampfireInfoTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CampfireStateTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes

        private static string BackingStorePath = null;

        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            BackingStorePath = System.IO.Path.Combine(testContext.TestDir, "CampfireInfo.xml");
            Utils_Accessor.traceSwitch.Level = System.Diagnostics.TraceLevel.Verbose;
        }

        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}

        MockNotifier mockNotifier;

        //
        //Use TestInitialize to run code before running each test
        //
        [TestInitialize()]
        public void MyTestInitialize()
        {
            CampfireState_Accessor.Instance = null;
            if (System.IO.File.Exists(BackingStorePath))
            {
                System.IO.File.Delete(BackingStorePath);
            }
            mockNotifier = new MockNotifier();
            CampfireState.Initialize(TestContext.TestDir, mockNotifier);
            SmokeSignalConfig.Initialize(TestContext.TestDir);
        }
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        private void Match(CampfireState expected, CampfireState actual)
        {
            Assert.AreEqual(expected.Users.Count, actual.Users.Count);
            Assert.AreEqual(expected.Rooms.Count, actual.Rooms.Count);

            foreach (CampfireState.RoomInfo room in expected.Rooms)
            {
                Assert.IsTrue(actual.Rooms.Count(r => r.Id == room.Id && r.LastMessageId == room.LastMessageId && r.Name == room.Name) == 1);
            }

            foreach (CampfireState.UserInfo user in expected.Users)
            {
                Assert.IsTrue(actual.Users.Count(u => u.Id == user.Id && 
                                                        u.Name == user.Name && 
                                                        u.EmailAddress == user.EmailAddress &&
                                                        u.AltEmailAddress == user.AltEmailAddress) == 1);
            }
        }

        /// <summary>
        ///A test for Save
        ///</summary>
        [TestMethod()]
        public void SaveRestoreTest_NoUsersNoRooms()
        {
            CampfireState target = CampfireState.Instance;
            SmokeSignalConfig sstarget = SmokeSignalConfig.Instance;

            sstarget.SetAndSaveNameAndToken("fred", "9876543210");
            target.SetNameAndToken("fred", "9876543210");

            Assert.AreEqual(0, target.Users.Count);
            Assert.AreEqual(0, target.Rooms.Count);

            CampfireState newCampfireInfo = CampfireState_Accessor.Restore(BackingStorePath);
            Match(target, newCampfireInfo);
        }

        /// <summary>
        ///A test for Save
        ///</summary>
        [TestMethod()]
        public void SaveRestoreTest_UsersOnly()
        {
            CampfireState target = CampfireState.Instance;
            target.AddUser(1, "Peter", "peter@mail.com", null);
            target.AddUser(2, "Jake", "jake@mail.com", "2345@sms.att.net");

            Assert.AreEqual(2, target.Users.Count);
            Assert.AreEqual(0, target.Rooms.Count);

            CampfireState newCampfireInfo = CampfireState_Accessor.Restore(BackingStorePath);
            Match(target, newCampfireInfo);
        }

        /// <summary>
        ///A test for Save
        ///</summary>
        [TestMethod()]
        public void SaveRestoreTest_UsersOnly_WithDelay()
        {
            CampfireState target = CampfireState.Instance;
            target.AddUser(2, "Jake", "jake@mail.com", "2345@sms.att.net");

            Assert.AreEqual(1, target.Users.Count);
            Assert.AreEqual(0, target.Rooms.Count);

            target.UpdateUser(2, 4);
            CampfireState.UserInfo ui = target.Users.FirstOrDefault(u => u.Id == 2);
            Assert.AreEqual(4, ui.DelayInMinutes);

            target.AddUser(1, "Peter", "peter@mail.com", null);

            CampfireState newCampfireInfo = CampfireState_Accessor.Restore(BackingStorePath);
            Match(target, newCampfireInfo);

            ui = newCampfireInfo.Users.FirstOrDefault(u => u.Id == 2);
            Assert.AreEqual(4, ui.DelayInMinutes);
            ui = newCampfireInfo.Users.FirstOrDefault(u => u.Id == 1);
            Assert.AreEqual(-1, ui.DelayInMinutes);
        }

        /// <summary>
        ///A test for Save
        ///</summary>
        [TestMethod()]
        public void SaveRestoreTest_RoomsOnly()
        {

            CampfireState target = CampfireState.Instance;
            target.AddRoom(1, "Room1", 12);
            target.AddRoom(101, "Room2", 0);

            Assert.AreEqual(0, target.Users.Count);
            Assert.AreEqual(2, target.Rooms.Count);

            CampfireState newCampfireInfo = CampfireState_Accessor.Restore(BackingStorePath);
            Match(target, newCampfireInfo);
        }

        /// <summary>
        ///A test for AddRooms
        ///</summary>
        [TestMethod()]
        public void AddRoomsTest()
        {
            CampfireState target = CampfireState.Instance;
            target.SetNameAndToken("fred", "9876543210");
            target.AddRoom(1, "Room1", 12);
            target.AddRoom(101, "Room2", 0);

            List<Room> newRooms = new List<Room>();
            Room rm;

            rm = new Room(3, "Room3");        // new room
            newRooms.Add(rm);

            rm = new Room(101, "Room2");        // duplicate
            newRooms.Add(rm);

            rm = new Room(1, "RoomOne");        // existing room, new name
            newRooms.Add(rm);

            target.AddRooms(newRooms);

            Assert.AreEqual(3, target.Rooms.Count);
            Assert.IsTrue(target.Rooms.Count(r => r.Id == 1 && r.Name == "RoomOne") == 1, "verify renamed room");
            Assert.IsTrue(target.Rooms.Count(r => r.Id == 3 && r.Name == "Room3") == 1, "verify new room");

            CampfireState newCampfireInfo = CampfireState_Accessor.Restore(BackingStorePath);
            Match(target, newCampfireInfo);
        }

        /// <summary>
        ///A test for UpdateLastMessageId
        ///</summary>
        [TestMethod()]
        public void UpdateLastMessageIdTest()
        {
            CampfireState target = CampfireState.Instance;
            target.SetNameAndToken("fred", "9876543210");
            target.AddRoom(1, "Room1", 12);
            target.AddRoom(101, "Room2", 0);

            target.UpdateLastMessageId(101, 1234);

            CampfireState.RoomInfo room = target.Rooms.First(r => r.Id == 101);
            Assert.AreEqual(1234, room.LastMessageId);

            CampfireState newCampfireInfo = CampfireState_Accessor.Restore(BackingStorePath);
            room = newCampfireInfo.Rooms.First(r => r.Id == 101);
            Assert.AreEqual(1234, room.LastMessageId);
        }

    
        /// <summary>
        ///A test for Notifier.Doit
        ///</summary>
        [TestMethod()]
        public void NotifierTest()
        {
            CampfireState target = CampfireState.Instance;
            int prev = MockNotifier.MessagesSentTo.Count;
            target.Notifier.Doit("Peter", "Startup/Trial Discussion", 340248, "peter.potrebic@hotmail.com", "couponsinc", "Jake", "Hey @Peter, come here");
            Assert.AreEqual(prev+1, MockNotifier.MessagesSentTo.Count);
        }

        /// <summary>
        ///A test for AddPendingNotification
        ///</summary>
        [TestMethod()]
        public void AddPendingNotification_LaterTimeTest()
        {
            CampfireState_Accessor target = new CampfireState_Accessor();
            int userId = 11;
            int roomId = 51;
            CampfireState.UserReference ri = new CampfireState.UserReference("", new CampfireState.UserInfo(userId, "Fake User", "fake@mail.com", ""));
            ri.Room = new CampfireState.RoomInfo("fake room", roomId, 0);

            DateTime triggerTime = DateTime.Now.AddHours(1);
            target.AddPendingNotification(ri, triggerTime);
            Assert.AreEqual(1, target.PendingNotifications.Count);
            Assert.AreEqual(triggerTime, target.PendingNotifications[0].TriggerTime);
            DateTime laterTrigger = triggerTime.AddMinutes(1);

            // Trying to add a notification for a same user/room, but with a later time is a No-op
            target.AddPendingNotification(ri, laterTrigger);
            Assert.AreEqual(1, target.PendingNotifications.Count);
            Assert.AreEqual(triggerTime, target.PendingNotifications[0].TriggerTime);
        }

        /// <summary>
        ///A test for AddPendingNotification
        ///</summary>
        [TestMethod()]
        public void AddPendingNotification_EarlierTimeTest()
        {
            CampfireState_Accessor target = new CampfireState_Accessor();
            int userId = 11;
            int roomId = 51;
            DateTime triggerTime = DateTime.Now.AddHours(1);
            CampfireState.UserReference ri = new CampfireState.UserReference("", new CampfireState.UserInfo(userId, "Fake User", "fake@mail.com", ""));
            ri.Room = new CampfireState.RoomInfo("fake room", roomId, 0);

            target.AddPendingNotification(ri, triggerTime);
            Assert.AreEqual(1, target.PendingNotifications.Count);
            Assert.AreEqual(triggerTime, target.PendingNotifications[0].TriggerTime);
            DateTime earlierTrigger = triggerTime.AddMinutes(-1);

            // Trying to add a notification for a same user/room, but with a earlier time will cause the pending trigger time to adjust
            target.AddPendingNotification(ri, earlierTrigger);
            Assert.AreEqual(1, target.PendingNotifications.Count);
            Assert.AreEqual(earlierTrigger, target.PendingNotifications[0].TriggerTime);
        }
    }
}
