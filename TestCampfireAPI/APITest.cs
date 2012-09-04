// Copyright 2012 Peter Potrebic.  All rights reserved.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using CampfireAPI;
using System.Xml;
using System.Net;
using System.Xml.Linq;
using System.IO;

namespace TestCampfireAPI
{
    /// <summary>
    ///This is a test class for CampfireAPI.API. This class is about making the HTTP requests to the campire API and turning
    ///the results into the enity objects in this project - CampfireAPI.Message, CampfireAPI.Room, CampfireAPI.User
    ///To Test CampfireAPI.API the code below will stub in a mock ICampfireToXML implementation which will just return 
    ///hard coded Xml Documents.
    ///</summary>
    [TestClass()]
    public class APITest
    {

        // If one want to test against real campfire API (e.g. to validate that scheme is still OK) define the following 2 values. And
        // switch tests to use the other CampfireAPI.API constructor
        string Test_CampfireName = null;
        string Test_AuthToken = null;

        private class MockCampfireToXml : ICampfireToXml
        {
            public XDocument fakeDocument;
            public HttpStatusCode fakeStatus;

            public MockCampfireToXml(XDocument doc)
            {
                fakeDocument = doc;
            }

            #region ICampfireToXml Members

            public XDocument Doit(string httpGetRequestURI)
            {
                return Doit(httpGetRequestURI, "GET");
            }

            public XDocument Doit(string httpRequestURI, string requestMethod)
            {
                HttpStatusCode status;
                return Doit(httpRequestURI, "GET", out status);
            }

            public XDocument Doit(string httpRequestURI, string requestMethod, out HttpStatusCode httpStatus)
            {
                httpStatus = fakeStatus;
                return fakeDocument;
            }

            #endregion
        }

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
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for GetUser
        ///</summary>
        [TestMethod()]
        public void GetUserTest()
        {
            string rawXml = @"
                <user>
                  <created-at>2010-10-12T02:51:34Z</created-at>
                  <id >56897</id>
                  <type>Member</type>
                  <name>Peter Potrebic</name>
                  <email-address>potrebic@gmail.com</email-address>
                  <admin >true</admin>
                  <avatar-url>http://asset0.37img.com/global/missing/avatar.gif?r=3</avatar-url>
                </user>
            ";
            
            XDocument fakeDoc = XDocument.Load(new StringReader(rawXml));
            CampfireAPI.API target = new CampfireAPI.API(new MockCampfireToXml(fakeDoc));
            //CampfireAPI.API target = new CampfireAPI.API(Test_CampfireName, Test_AuthToken);

            int userId = 56897;
            User actual;
            actual = target.GetUser(userId);
            Assert.IsNotNull(actual);
            Assert.AreEqual(userId, actual.Id);
            Assert.AreEqual("Peter Potrebic", actual.Name);
            Assert.AreEqual("potrebic@gmail.com", actual.Email);
        }

        /// <summary>
        ///A test for Rooms
        ///</summary>
        [TestMethod()]
        public void RoomsTest()
        {
            string rawXml = @"
              <rooms >
                  <room>
                    <created-at >2010-10-14T19:10:24Z</created-at>
                    <id >340137</id>
                    <locked >false</locked>
                    <membership-limit >4</membership-limit>
                    <name>General Tech. Q&amp;A</name>
                    <topic>General question about Coupons Inc, technologies, coding, ops, etc. Ask, discuss, answer here.</topic>
                    <updated-at >2010-10-14T19:20:03Z</updated-at>
                  </room>
                  <room>
                    <created-at >2010-10-14T19:19:37Z</created-at>
                    <id >340141</id>
                    <locked >false</locked>
                    <membership-limit >4</membership-limit>
                    <name>Non-business chatter</name>
                    <topic>A place to discuss non business related topics. Water cooler stuff.</topic>
                    <updated-at >2010-10-14T19:19:37Z</updated-at>
                  </room>
              </rooms>
            ";

            XDocument fakeDoc = XDocument.Load(new StringReader(rawXml));
            CampfireAPI.API target = new CampfireAPI.API(new MockCampfireToXml(fakeDoc));
            //CampfireAPI.API target = new CampfireAPI.API(Test_CampfireName, Test_AuthToken);
            List<Room> actual;
            actual = target.Rooms();
            Assert.IsTrue(actual.Count == 2);

            Assert.IsTrue(actual.Any(r => string.Equals(r.Name.ToLower(), "General Tech. Q&A".ToLower())));
            Assert.IsTrue(actual.Any(r => string.Equals(r.Name.ToLower(), "Non-business chatter".ToLower())));

            Assert.IsTrue(actual.Any(r => string.Equals(r.Id, 340137)));
            Assert.IsTrue(actual.Any(r => string.Equals(r.Id, 340141)));
        }

        /// <summary>
        ///A test for Room
        ///</summary>
        [TestMethod()]
        public void RoomTest()
        {
            string rawXml = @"
                <room>
                  <created-at >2010-10-14T19:19:37Z</created-at>
                  <id >340141</id>
                  <locked >false</locked>
                  <membership-limit >4</membership-limit>
                  <name>Non-business chatter</name>
                  <topic>A place to discuss non business related topics. Water cooler stuff.</topic>
                  <updated-at >2010-10-14T19:19:37Z</updated-at>
                  <open-to-guests >false</open-to-guests>
                  <full >false</full>
                  <users />
                </room>";

            XDocument fakeDoc = XDocument.Load(new StringReader(rawXml));
            CampfireAPI.API target = new CampfireAPI.API(new MockCampfireToXml(fakeDoc));
            //CampfireAPI.API target = new CampfireAPI.API(Test_CampfireName, Test_AuthToken);
            Room actual = target.GetRoom(340141);
            Assert.AreEqual("Non-business chatter", actual.Name);
        }
        /// <summary>
        ///A test for Room
        ///</summary>
        
        [TestMethod()]
        public void RoomBadIdTest()
        {
            XDocument fakeDoc = new XDocument();
            CampfireAPI.API target = new CampfireAPI.API(new MockCampfireToXml(fakeDoc));
            //CampfireAPI.API target = new CampfireAPI.API(Test_CampfireName, Test_AuthToken);
            Room actual = target.GetRoom(9999);
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Test showing that filtering works when retreiving 'messages'
        /// </summary>
        [TestMethod()]
        public void TextOrPasteMessageTest()
        {
            string rawXml = @"
            <messages>
              <message>
                <created-at >2012-08-22T20:00:00Z</created-at>
                <id >650978084</id>
                <room-id >340141</room-id>
                <user-id  ></user-id>
                <body ></body>
                <type>TimestampMessage</type>
              </message>
              <message>
                <created-at >2012-08-22T20:03:05Z</created-at>
                <id >650978085</id>
                <room-id >340141</room-id>
                <user-id >56897</user-id>
                <body ></body>
                <type>EnterMessage</type>
              </message>
              <message>
                <created-at >2012-08-22T20:03:17Z</created-at>
                <id >650978309</id>
                <room-id >340141</room-id>
                <user-id >56897</user-id>
                <body>hello</body>
                <type>TextMessage</type>
              </message>
              <message>
                <created-at >2012-08-22T20:12:06Z</created-at>
                <id >650988431</id>
                <room-id >340141</room-id>
                <user-id >56897</user-id>
                <body> &lt;!-- To modify your build process, add your task inside one of the targets below and uncomment it. 
                   Other similar extension points exist, see Microsoft.Common.targets.</body>
                <type>PasteMessage</type>
              </message>
            </messages>  ";

            XDocument fakeDoc = XDocument.Load(new StringReader(rawXml));
            CampfireAPI.API target = new CampfireAPI.API(new MockCampfireToXml(fakeDoc));
            //CampfireAPI.API target = new CampfireAPI.API(Test_CampfireName, Test_AuthToken);

            List<Message> msgs = target.RecentMessages(340141, 1, Message.MType.TextMessage | Message.MType.EnterMessage);
            Assert.IsTrue(msgs.All(m => m.Type == Message.MType.TextMessage || m.Type == Message.MType.EnterMessage));
            Assert.AreEqual(3, msgs.Count);

            Assert.AreEqual(2, msgs.Count(m => m.Type == Message.MType.TextMessage));       // should have 2 Text messages. "Pastes" are turned into 'Text'
        }


        /// <summary>
        ///A test for UsersInRoom
        ///</summary>
        [TestMethod()]
        public void UsersInRoomTest()
        {
            string rawXml = @"
                <room>
                  <created-at >2010-10-14T19:19:37Z</created-at>
                  <id >340141</id>
                  <locked >false</locked>
                  <membership-limit >4</membership-limit>
                  <name>Non-business chatter</name>
                  <topic>A place to discuss non business related topics. Water cooler stuff.</topic>
                  <updated-at >2010-10-14T19:19:37Z</updated-at>
                  <open-to-guests >false</open-to-guests>
                  <full >false</full>
                  <users >
                    <user type=""Member"">
                      <created-at >2010-10-12T02:51:34Z</created-at>
                      <id >56897</id>
                      <type>Member</type>
                      <name>Peter Potrebic</name>
                      <email-address>potrebic@gmail.com</email-address>
                      <admin >true</admin>
                      <avatar-url>http://asset0.37img.com/global/missing/avatar.gif?r=3</avatar-url>
                    </user>
                    <user type=""Guest"">
                      <created-at >2010-10-12T02:51:34Z</created-at>
                      <id >0</id>
                      <type>Guest</type>
                      <name>Unknown Joe</name>
                      <email-address>xxx@yyy.com</email-address>
                      <admin >false</admin>
                      <avatar-url>http://asset0.37img.com/global/missing/avatar.gif?r=3</avatar-url>
                    </user>
                  </users>
                </room>";

            XDocument fakeDoc = XDocument.Load(new StringReader(rawXml));
            CampfireAPI.API target = new CampfireAPI.API(new MockCampfireToXml(fakeDoc));
            //CampfireAPI.API target = new CampfireAPI.API(Test_CampfireName, Test_AuthToken);
            List<User> actual = target.UsersInRoom(340141);
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("Peter Potrebic", actual[0].Name);
        }
    }
}
