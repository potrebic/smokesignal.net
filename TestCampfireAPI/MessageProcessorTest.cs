// Copyright 2012 Peter Potrebic.  All rights reserved.

using NotifierLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CampfireAPI;
//using System.Messaging;
using System.Linq;
using System;
using System.Collections.Generic;

namespace TestCampfireAPI
{
    
    
    /// <summary>
    ///This is a test class for MessageProcessorTest and is intended
    ///to contain all MessageProcessorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MessageProcessorTest
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
            MockNotifier mn = new MockNotifier();
            CampfireState.Initialize(TestContext.TestDir, mn);
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

#if false
        public class Order
        {
            public int orderId;
            public DateTime orderTime;
        };
        public static void SendMessage()
        {

            // Create a new order and set values.
            Order sentOrder = new Order();
            sentOrder.orderId = 3;
            sentOrder.orderTime = DateTime.Now;

            // Connect to a queue on the local computer.
            MessageQueue myQueue = new MessageQueue(".\\myQueue");

            // Send the Order to the queue.
            myQueue.Send(sentOrder);

            return;
        }


        //**************************************************
        // Receives a message containing an Order.
        //**************************************************

        public static void ReceiveMessage()
        {
            // Connect to the a queue on the local computer.
            MessageQueue myQueue = new MessageQueue(".\\myQueue");

            // Set the formatter to indicate body contains an Order.
            myQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(Order) });

            try
            {
                // Receive and format the message. 
                System.Messaging.Message myMessage = myQueue.Receive();
                Order myOrder = (Order)myMessage.Body;

                // Display message information.
                Console.WriteLine("Order ID: " +
                    myOrder.orderId.ToString());
                Console.WriteLine("Sent: " +
                    myOrder.orderTime.ToString());
            }

            catch (MessageQueueException)
            {
                // Handle Message Queuing exceptions.
            }

            // Handle invalid serialization format.
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e.Message);
            }

            // Catch other exceptions as necessary.

            return;
        }

        [TestMethod()]
        public void MessageQueueTest()
        {
            // Send a message to a queue.
            SendMessage();

            // Receive a message from a queue.
            ReceiveMessage();
        }
#endif

        /// <summary>
        ///A test for Work_ConsumeQueuedEnterMessages
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NotifierLibrary.dll")]
        public void Work_ConsumeQueuedEnterMessagesTest()
        {
            CampfireState campInfo = CampfireState.Instance;
            MockCampfireAPI api = new MockCampfireAPI();

            Assert.AreEqual(0, campInfo.Users.Count);

            int fakeUserId = 12;

            api.FakeUsers.Add(new User("Peter", "peter@mail.com", fakeUserId));

            // Create a Entered Message for the fakeUser
            campInfo.QueueMessage(new CampfireAPI.Message(1, CampfireAPI.Message.MType.EnterMessage, 23, fakeUserId, string.Empty));

            // The process step should now process that queued Enter Message
            MessageProcessor_Accessor.ProcessMessages_ProcessQueuedMessages(campInfo, api);
            Assert.AreEqual(1, campInfo.Users.Count);
            Assert.IsTrue(campInfo.Users.Any(u => u.Id == fakeUserId && u.Name == "Peter"));
        }


        /// <summary>
        ///A test for a real sceanrio. Adding a pending notificaiton.And then having it canlced because person enters the room
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NotifierLibrary.dll")]
        public void Scenario_AddAndRemovePendingNotif_Test()
        {
            CampfireState campInfo = CampfireState.Instance;
            MockCampfireAPI api = new MockCampfireAPI();

            Assert.AreEqual(0, campInfo.Users.Count);

            int msgId = 1;
            int fakeUserId = 12;
            api.FakeUsers.Add(new User("Peter Potrebic", "peter@mail.com", fakeUserId));
            int fakeUserId2 = 13;
            api.FakeUsers.Add(new User("Joe Cabral", "joe@mail.com", fakeUserId2));

            int roomId1 = 24;
            int roomId2 = 25;
            api.FakeRooms.Add(new Room(roomId1, "room #24"));
            api.FakeRooms.Add(new Room(roomId2, "room #25"));

            MessageProcessor_Accessor.Work_ScanForAddOrRemoveRooms(campInfo, api);
            Assert.AreEqual(2, campInfo.Rooms.Count);

            List<Message> msgs;
            msgs = new List<Message>();
            api.MessagesInRoom[roomId1] = msgs;
            msgs = new List<Message>();
            api.MessagesInRoom[roomId2] = msgs;

            // Create a Entered Message for the fakeUser
            api.MessagesInRoom[roomId1].Add(new CampfireAPI.Message(msgId++, CampfireAPI.Message.MType.EnterMessage, roomId1, fakeUserId, string.Empty));
            api.MessagesInRoom[roomId2].Add(new CampfireAPI.Message(msgId++, CampfireAPI.Message.MType.EnterMessage, roomId2, fakeUserId2, string.Empty));

            MessageProcessor_Accessor.Work_ProcessNewMessagesForAllRooms(campInfo, api);
            Assert.AreEqual(2, campInfo.Users.Count);
            Assert.IsTrue(campInfo.Users.Any(u => u.Id == fakeUserId && u.Name == "Peter Potrebic"));
            Assert.IsTrue(campInfo.Users.Any(u => u.Id == fakeUserId2 && u.Name == "Joe Cabral"));
            campInfo.UpdateUser(fakeUserId2, 10);

            // Create a Text Message for the fakeUser
            api.MessagesInRoom[roomId1].Add(new CampfireAPI.Message(msgId++, CampfireAPI.Message.MType.TextMessage, roomId1, fakeUserId, "Hey, @JoeC care to join us"));

            // The process step should now process that Text Message
            MessageProcessor_Accessor.Work_ProcessNewMessagesForAllRooms(campInfo, api);
            Assert.AreEqual(1, campInfo.PendingNotifications.Count);
            CampfireState.PendingNotify pn = campInfo.PendingNotifications[0];
            Assert.IsTrue(pn.TriggerTime > DateTime.Now.AddMinutes(9));

            // now the target of the notication enters the room... which should cancel the notification
            api.MessagesInRoom[roomId1].Add(new CampfireAPI.Message(msgId++, CampfireAPI.Message.MType.EnterMessage, roomId1, fakeUserId2, string.Empty));
            MessageProcessor_Accessor.Work_ProcessNewMessagesForAllRooms(campInfo, api);
            Assert.AreEqual(0, campInfo.PendingNotifications.Count);
        }
        /// <summary>
        ///A test for a real sceanrio. A guest user trying to send a SmokeSignal.
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NotifierLibrary.dll")]
        public void Scenario_GuestUser_SendsSmokeSignal_Test()
        {
            CampfireState campInfo = CampfireState.Instance;
            MockCampfireAPI api = new MockCampfireAPI();

            Assert.AreEqual(0, campInfo.Users.Count);

            int msgId = 1;
            int realUserId = 12;
            int guestUserId = 123;
            api.FakeUsers.Add(new User("Peter Potrebic", "peter@mail.com", realUserId));

            int roomId1 = 24;
            api.FakeRooms.Add(new Room(roomId1, "room #24"));

            MessageProcessor_Accessor.Work_ScanForAddOrRemoveRooms(campInfo, api);
            Assert.AreEqual(1, campInfo.Rooms.Count);

            List<Message> msgs;
            msgs = new List<Message>();
            api.MessagesInRoom[roomId1] = msgs;

            // Create a Text Message for the real user
            api.MessagesInRoom[roomId1].Add(new CampfireAPI.Message(msgId++, CampfireAPI.Message.MType.TextMessage, roomId1, realUserId,
                "hello"));

            // The process step should now process that Text Message which gets the user into the system
            MessageProcessor_Accessor.Work_ProcessNewMessagesForAllRooms(campInfo, api);

            // Create a Text Message for the fakeUser
            api.MessagesInRoom[roomId1].Add(new CampfireAPI.Message(msgId++, CampfireAPI.Message.MType.TextMessage, roomId1, guestUserId,
                "!PeterP can you help with problem XYZ?"));

            MessageProcessor_Accessor.Work_ProcessNewMessagesForAllRooms(campInfo, api);
        }

        /// <summary>
        ///A test for a real sceanrio. A guest user trying to issue a SmokeSignal configuration command.
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NotifierLibrary.dll")]
        public void Scenario_SSCommand_GuestUser_Test()
        {
            CampfireState campInfo = CampfireState.Instance;
            MockCampfireAPI api = new MockCampfireAPI();

            Assert.AreEqual(0, campInfo.Users.Count);

            int msgId = 1;
            int fakeUserId = 12;
            int guestUserId = 123;
            api.FakeUsers.Add(new User("Peter Potrebic", "peter@mail.com", fakeUserId));

            int roomId1 = 24;
            api.FakeRooms.Add(new Room(roomId1, "room #24"));

            MessageProcessor_Accessor.Work_ScanForAddOrRemoveRooms(campInfo, api);
            Assert.AreEqual(1, campInfo.Rooms.Count);

            List<Message> msgs;
            msgs = new List<Message>();
            api.MessagesInRoom[roomId1] = msgs;

            // Create a Text Message for the fakeUser
            api.MessagesInRoom[roomId1].Add(new CampfireAPI.Message(msgId++, CampfireAPI.Message.MType.TextMessage, roomId1, guestUserId,
                "@SSignal:Altemail=guest@be.com please"));

            // This process step should just ignore the command because it was issued by a quest.
            MessageProcessor_Accessor.Work_ProcessNewMessagesForAllRooms(campInfo, api);
        }


        /// <summary>
        ///A test for a real sceanrio. Test various "Set AltEmail" cases, valid / invalid emails and clearing.
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NotifierLibrary.dll")]
        public void Scenario_SSCommand_SetAltEmail_Test()
        {
            CampfireState campInfo = CampfireState.Instance;
            MockCampfireAPI api = new MockCampfireAPI();

            Assert.AreEqual(0, campInfo.Users.Count);

            int msgId = 1;
            int fakeUserId = 12;
            api.FakeUsers.Add(new User("Peter Potrebic", "peter@mail.com", fakeUserId));

            int roomId1 = 24;
            api.FakeRooms.Add(new Room(roomId1, "room #24"));

            MessageProcessor_Accessor.Work_ScanForAddOrRemoveRooms(campInfo, api);
            Assert.AreEqual(1, campInfo.Rooms.Count);

            List<Message> msgs;
            msgs = new List<Message>();
            api.MessagesInRoom[roomId1] = msgs;

            // Create a Text Message for the fakeUser
            api.MessagesInRoom[roomId1].Add(new CampfireAPI.Message(msgId++, CampfireAPI.Message.MType.TextMessage, roomId1, fakeUserId, 
                "hello"));

            // The process step should now process that Text Message which gets the current user into the system
            MessageProcessor_Accessor.Work_ProcessNewMessagesForAllRooms(campInfo, api);

            Assert.AreEqual("", campInfo.Users[0].AltEmailAddress);

            // Create a Text Message for the fakeUser
            api.MessagesInRoom[roomId1].Add(new CampfireAPI.Message(msgId++, CampfireAPI.Message.MType.TextMessage, roomId1, fakeUserId,
                "@SSignal:Altemail=peter_john@be.com please"));
            // The process step should now process that Text Message which gets the current user into the system
            MessageProcessor_Accessor.Work_ProcessNewMessagesForAllRooms(campInfo, api);

            Assert.AreEqual("peter_john@be.com", campInfo.Users[0].AltEmailAddress);

            // Create a Text Message for the fakeUser
            api.MessagesInRoom[roomId1].Add(new CampfireAPI.Message(msgId++, CampfireAPI.Message.MType.TextMessage, roomId1, fakeUserId,
                "@SSignal:AltEmail=bogus_email please"));
            // The process step should now process that Text Message which gets the current user into the system
            MessageProcessor_Accessor.Work_ProcessNewMessagesForAllRooms(campInfo, api);

            Assert.AreEqual("peter_john@be.com", campInfo.Users[0].AltEmailAddress);

            // Create a Text Message for the fakeUser
            api.MessagesInRoom[roomId1].Add(new CampfireAPI.Message(msgId++, CampfireAPI.Message.MType.TextMessage, roomId1, fakeUserId,
                "@SSignal:ALTEMAIL= please"));
            // The process step should now process that Text Message which gets the current user into the system
            MessageProcessor_Accessor.Work_ProcessNewMessagesForAllRooms(campInfo, api);

            Assert.AreEqual("", campInfo.Users[0].AltEmailAddress);
        }

        /// <summary>
        ///A test for a real sceanrio. Adding a pending notification.And then having it canlced because person enters the room
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NotifierLibrary.dll")]
        public void Scenario_SSCommand_SetDelay_Test()
        {
            CampfireState campInfo = CampfireState.Instance;
            MockCampfireAPI api = new MockCampfireAPI();

            Assert.AreEqual(0, campInfo.Users.Count);

            int msgId = 1;
            int fakeUserId = 12;
            api.FakeUsers.Add(new User("Peter Potrebic", "peter@mail.com", fakeUserId));

            int roomId1 = 24;
            api.FakeRooms.Add(new Room(roomId1, "room #24"));

            MessageProcessor_Accessor.Work_ScanForAddOrRemoveRooms(campInfo, api);
            Assert.AreEqual(1, campInfo.Rooms.Count);

            List<Message> msgs;
            msgs = new List<Message>();
            api.MessagesInRoom[roomId1] = msgs;

            //// Create a Entered Message for the fakeUser
            //api.MessagesInRoom[roomId1].Add(new CampfireAPI.Message(msgId++, CampfireAPI.Message.MType.EnterMessage, roomId1, fakeUserId, string.Empty));
            //api.MessagesInRoom[roomId2].Add(new CampfireAPI.Message(msgId++, CampfireAPI.Message.MType.EnterMessage, roomId2, fakeUserId2, string.Empty));

            //MessageProcessor_Accessor.Work_ProcessNewMessagesForAllRooms(campInfo, api);
            //Assert.AreEqual(2, campInfo.Users.Count);
            //Assert.IsTrue(campInfo.Users.Any(u => u.Id == fakeUserId && u.Name == "Peter Potrebic"));
            //Assert.IsTrue(campInfo.Users.Any(u => u.Id == fakeUserId2 && u.Name == "Joe Cabral"));
            //campInfo.UpdateUser(fakeUserId2, 10);

            // Create a Text Message for the fakeUser
            api.MessagesInRoom[roomId1].Add(new CampfireAPI.Message(msgId++, CampfireAPI.Message.MType.TextMessage, roomId1, fakeUserId, "@SSignal:Delay=6 please"));

            // The process step should now process that Text Message
            MessageProcessor_Accessor.Work_ProcessNewMessagesForAllRooms(campInfo, api);

            Assert.AreEqual(6, campInfo.Users[0].DelayInMinutes);
        }


        /// <summary>
        ///A test for Work_ScanForUserChanges
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NotifierLibrary.dll")]
        public void Work_ScanForUserChangesTest()
        {
            CampfireState campInfo = CampfireState.Instance;
            MockCampfireAPI api = new MockCampfireAPI();

            // initialize known state with 1 user
            int changedUserId = 12;
            campInfo.AddUser(changedUserId, "Peter", "peter@mail.com", "1234");

            // Now change the data assoc with that user (at the API level)
            api.FakeUsers.Add(new User("Jake", "jake@mail.com", changedUserId));

            // The process step should now pick up the change and incorporate into campfileState
            MessageProcessor_Accessor.Work_ScanForUserChanges(campInfo, api);
            Assert.AreEqual(1, campInfo.Users.Count);
            Assert.IsTrue(campInfo.Users.Any(u => u.Id == changedUserId && u.Name == "Jake" && u.EmailAddress == "jake@mail.com"));
        }

        /// <summary>
        ///A test for Work_ScanForAddOrRemoveRooms
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NotifierLibrary.dll")]
        public void Work_ScanForAddOrRemoveRoomsTest()
        {
            CampfireState campInfo = CampfireState.Instance;
            MockCampfireAPI api = new MockCampfireAPI();

            // Create a new room (at the API level)
            int roomId = 123;
            api.FakeRooms.Add(new Room(roomId, "talking room"));

            // The process step should now pick up the change and incorporate into campfileState
            MessageProcessor_Accessor.Work_ScanForAddOrRemoveRooms(campInfo, api);
            Assert.AreEqual(1, campInfo.Rooms.Count);
            Assert.IsTrue(campInfo.Rooms.Any(u => u.Id == roomId && u.Name == "talking room"));
        }

        /// <summary>
        ///A test for Work_FetchNewMessagesForAllRooms
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NotifierLibrary.dll")]
        public void Work_FetchNewMessagesForAllRoomsTest()
        {
            CampfireState campInfo = CampfireState.Instance;
            MockCampfireAPI api = new MockCampfireAPI();

            List<Message> msgs;

            // Create a new room (at the API level)
            int roomId1 = 123;
            int roomId2 = 1001;
            int userId1 = 100;
            int userId2 = 200;
            api.FakeUsers.Add(new User("Peter", "peter@mail.com", userId1));
            api.FakeUsers.Add(new User("Casey", "casey@mail.com", userId2));
            
            Room rm1 = new Room(roomId1, "room #1");
            
            // The room must exist in the API and in cmapfireState
            api.FakeRooms.Add(rm1);
            campInfo.AddRoom(rm1.Id, rm1.Name, 0);

            // add a couple fake messages to room1
            msgs = new List<Message>();
            msgs.Add(new Message(1, Message.MType.EnterMessage, roomId1, userId1, ""));
            msgs.Add(new Message(2, Message.MType.TextMessage, roomId1, userId1, "Hello everyone"));
            api.MessagesInRoom.Add(roomId1, msgs);

            // add room 2
            Room rm2 = new Room(roomId2, "room #2");
            api.FakeRooms.Add(rm2);
            campInfo.AddRoom(rm2.Id, rm2.Name, 0);

            // add a couple fake messages to room1
            msgs = new List<Message>();
            msgs.Add(new Message(3, Message.MType.EnterMessage, roomId2, userId2, ""));
            msgs.Add(new Message(4, Message.MType.TextMessage, roomId2, userId2, "message from user 2"));
            msgs.Add(new Message(5, Message.MType.TextMessage, roomId2, userId1, "message from user 1"));
            api.MessagesInRoom.Add(roomId2, msgs);

            // The process step should now pick up the change and incorporate into campfileState
            MessageProcessor_Accessor.Work_ProcessNewMessagesForAllRooms(campInfo, api);

            // In the "Entered" queue there should be 2 entries
            MessageProcessor_Accessor.ProcessMessages_ProcessQueuedMessages(campInfo, api);
            Assert.AreEqual(2, campInfo.Users.Count);
            Assert.IsTrue(campInfo.Users.Any(u => u.Id == userId1));
            Assert.IsTrue(campInfo.Users.Any(u => u.Id == userId2));

            // In the Text Message queue there should be 3 entries
            MessageProcessor_Accessor.ProcessMessages_ProcessQueuedMessages(campInfo, api);
        }
    }
}
