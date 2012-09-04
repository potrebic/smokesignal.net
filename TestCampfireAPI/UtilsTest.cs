// Copyright 2012 Peter Potrebic.  All rights reserved.

using NotifierLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace TestCampfireAPI
{
    
    
    /// <summary>
    ///This is a test class for UtilsTest and is intended
    ///to contain all UtilsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class UtilsTest
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
        ///A test for WordsStartingWithPrefix
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NotifierLibrary.dll")]
        public void WordsStartingWithPrefixTest()
        {
            string text = "here we go @Jake someday,@Mary,@ joeseph.@";
            IList<string> actual;
            actual = Utils_Accessor.WordsStartingWithPrefix(text, "@", false);
            Assert.AreEqual(2, actual.Count);
            Assert.IsTrue(actual.Any(s => s == "Jake"));
            Assert.IsTrue(actual.Any(s => s == "Mary"));
        }

        /// <summary>
        ///A test for FindUserReferences
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NotifierLibrary.dll")]
        public void FindUserReferences_NoMatchTest()
        {
            List<CampfireState.UserInfo> users = new List<CampfireState.UserInfo>();
            users.Add(new CampfireState.UserInfo(1, "Peter Potrebic", "pp@be.com", ""));
            users.Add(new CampfireState.UserInfo(2, "Peter Johnson", "pj@be.com", ""));

            IList<CampfireState.UserReference> lazyReferences;
            IList<CampfireState.UserReference> immediateReferences;
            Utils.FindUserReferences("hey @george please answer the question", users, out lazyReferences, out immediateReferences);
            Assert.AreEqual(0, lazyReferences.Count);
            Assert.AreEqual(0, immediateReferences.Count);
        }

        /// <summary>
        ///A test for FindUserReferences
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NotifierLibrary.dll")]
        public void FindUserReferences_ImmediateTest()
        {
            List<CampfireState.UserInfo> users = new List<CampfireState.UserInfo>();
            users.Add(new CampfireState.UserInfo(1, "Peter Potrebic", "pp@be.com", ""));
            users.Add(new CampfireState.UserInfo(2, "Peter Johnson", "pj@be.com", ""));

            IList<CampfireState.UserReference> lazyReferences;
            IList<CampfireState.UserReference> immediateReferences;
            string msg = "hey !Johnson please answer the question! Please";
            Utils.FindUserReferences(msg, users, out lazyReferences, out immediateReferences);
            Assert.AreEqual(0, lazyReferences.Count);
            Assert.AreEqual(1, immediateReferences.Count);
            Assert.AreEqual(msg, immediateReferences[0].NearByText);
        }


        ///A test for FindUserReferences
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NotifierLibrary.dll")]
        public void FindUserReferences_NearByText()
        {
            List<CampfireState.UserInfo> users = new List<CampfireState.UserInfo>();
            users.Add(new CampfireState.UserInfo(1, "Peter Potrebic", "pp@be.com", ""));
            users.Add(new CampfireState.UserInfo(2, "Peter Johnson", "pj@be.com", ""));

            IList<CampfireState.UserReference> lazyReferences;
            IList<CampfireState.UserReference> immediateReferences;
            string msg = "A really long message to overflow the 100 char limit.  So that we only capture a portion!   hey @Johnson please answer the question!  When will you do so? We have to know asap!";
            string expected = "hey @Johnson please answer the question!";
            Utils.FindUserReferences(msg, users, out lazyReferences, out immediateReferences);
            Assert.AreEqual(0, immediateReferences.Count);
            Assert.AreEqual(1, lazyReferences.Count);
            Assert.AreEqual(expected, lazyReferences[0].NearByText);
        }

        /// <summary>
        ///A test for FindUserReferences
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NotifierLibrary.dll")]
        public void FindUserReferences_ImmediateAndLazyTest()
        {
            List<CampfireState.UserInfo> users = new List<CampfireState.UserInfo>();
            users.Add(new CampfireState.UserInfo(1, "Peter Potrebic", "pp@be.com", ""));
            users.Add(new CampfireState.UserInfo(2, "Peter Johnson", "pj@be.com", ""));

            IList<CampfireState.UserReference> lazyReferences;
            IList<CampfireState.UserReference> immediateReferences;
            Utils.FindUserReferences("hey !Johnson or @potrebic please answer the question", users, out lazyReferences, out immediateReferences);
            Assert.AreEqual(1, lazyReferences.Count);
            Assert.IsTrue(lazyReferences.Any(r => r.TargetUser.Name == "Peter Potrebic"));
            Assert.AreEqual(1, immediateReferences.Count);
            Assert.IsTrue(immediateReferences.Any(r => r.TargetUser.Name == "Peter Johnson"));
        }

        /// <summary>
        ///A test for FindUserReferences
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NotifierLibrary.dll")]
        public void FindUserReferences_MultipleMatchesTest()
        {
            List<CampfireState.UserInfo> users = new List<CampfireState.UserInfo>();
            users.Add(new CampfireState.UserInfo(1, "Peter Potrebic", "pp@be.com", ""));
            users.Add(new CampfireState.UserInfo(2, "Peter Johnson", "pj@be.com", ""));

            IList<CampfireState.UserReference> lazyReferences;
            IList<CampfireState.UserReference> immediateReferences;
            Utils.FindUserReferences("hey @peter please answer the question", users, out lazyReferences, out immediateReferences);
            Assert.AreEqual(0, lazyReferences.Count);
        }

        /// <summary>
        ///A test for FindUserReferences
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NotifierLibrary.dll")]
        public void FindUserReferences_MultipleRefsToSameNameTest()
        {
            List<CampfireState.UserInfo> users = new List<CampfireState.UserInfo>();
            users.Add(new CampfireState.UserInfo(1, "Peter Potrebic", "pp@be.com", ""));
            users.Add(new CampfireState.UserInfo(2, "Peter Johnson", "pj@be.com", ""));

            IList<CampfireState.UserReference> lazyReferences;
            IList<CampfireState.UserReference> immediateReferences;

            // notice that there are 2 references to the same person. This should only result in 1 total reference.
            Utils.FindUserReferences("hey @peterPotrebic please answer the question. Another ref to @potrebic", users, out lazyReferences, out immediateReferences);
            Assert.AreEqual(1, lazyReferences.Count);
        }

        /// <summary>
        ///A test for FindUserReferences
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NotifierLibrary.dll")]
        public void FindUserReferences_Test()
        {
            List<CampfireState.UserInfo> users = new List<CampfireState.UserInfo>();
            users.Add(new CampfireState.UserInfo(1, "Peter Potrebic", "pp@be.com", ""));
            users.Add(new CampfireState.UserInfo(2, "Steve Johnson", "pj@be.com", ""));
            users.Add(new CampfireState.UserInfo(3, "Lindy Johnson", "lj@be.com", ""));

            IList<CampfireState.UserReference> lazyReferences;
            IList<CampfireState.UserReference> immediateReferences;
            Utils.FindUserReferences("hey @peter or @stevej please answer the question", users, out lazyReferences, out immediateReferences);

            Assert.AreEqual(2, lazyReferences.Count);
            Assert.IsTrue(lazyReferences.Any(r => r.TargetUser.Name == "Peter Potrebic"));
            Assert.IsTrue(lazyReferences.Any(r => r.TargetUser.Name == "Steve Johnson"));
        }

        /// <summary>
        ///A test for FindUserReferences
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NotifierLibrary.dll")]
        public void FindUserReferences_StartOfTextTest()
        {
            List<CampfireState.UserInfo> users = new List<CampfireState.UserInfo>();
            users.Add(new CampfireState.UserInfo(1, "Peter Potrebic", "pp@be.com", ""));
            users.Add(new CampfireState.UserInfo(2, "Steve Johnson", "pj@be.com", ""));
            users.Add(new CampfireState.UserInfo(3, "Lindy Johnson", "lj@be.com", ""));

            IList<CampfireState.UserReference> lazyReferences;
            IList<CampfireState.UserReference> immediateReferences;
            Utils.FindUserReferences("Peter Potrebic: please answer the question", users, out lazyReferences, out immediateReferences);

            Assert.AreEqual(1, lazyReferences.Count);
            Assert.IsTrue(lazyReferences.Any(r => r.TargetUser.Name == "Peter Potrebic"));
        }

        /// <summary>
        ///A test for FindUserReferences
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NotifierLibrary.dll")]
        public void FindUserReferences_AtSignStartOfTextTest()
        {
            List<CampfireState.UserInfo> users = new List<CampfireState.UserInfo>();
            users.Add(new CampfireState.UserInfo(1, "Peter Potrebic", "pp@be.com", ""));
            users.Add(new CampfireState.UserInfo(2, "Steve Johnson", "pj@be.com", ""));
            users.Add(new CampfireState.UserInfo(3, "Lindy Johnson", "lj@be.com", ""));

            IList<CampfireState.UserReference> lazyReferences;
            IList<CampfireState.UserReference> immediateReferences;
            Utils.FindUserReferences("@PeterP: please answer the question", users, out lazyReferences, out immediateReferences);

            Assert.AreEqual(1, lazyReferences.Count);
            Assert.IsTrue(lazyReferences.Any(r => r.TargetUser.Name == "Peter Potrebic"));
        }

        /// <summary>
        ///A test for FindUserReferences
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NotifierLibrary.dll")]
        public void FindSmokeSignalReferences_Test()
        {
            List<CampfireState.UserInfo> users = new List<CampfireState.UserInfo>();
            users.Add(new CampfireState.UserInfo(1, "Peter Potrebic", "pp@be.com", ""));

            IList<CampfireState.UserReference> lazyReferences;
            IList<CampfireState.UserReference> immediateReferences;
            IList<CampfireState.UserReference> smokeSignalReferences;
            Utils.FindUserReferences("hey @SmokeSignal:settings or @stevej please answer the question", users,
                out lazyReferences, out immediateReferences, out smokeSignalReferences);

            Assert.AreEqual(0, lazyReferences.Count);
            Assert.AreEqual(0, immediateReferences.Count);
            Assert.AreEqual(1, smokeSignalReferences.Count);
            Assert.AreEqual("SmokeSignal:settings", smokeSignalReferences[0].NearByText);
        }

        /// <summary>
        ///A test for FindUserReferences
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NotifierLibrary.dll")]
        public void FindSmokeSignalReferencesWithEmail_Test()
        {
            List<CampfireState.UserInfo> users = new List<CampfireState.UserInfo>();
            users.Add(new CampfireState.UserInfo(1, "Peter Potrebic", "pp@be.com", ""));

            IList<CampfireState.UserReference> lazyReferences;
            IList<CampfireState.UserReference> immediateReferences;
            IList<CampfireState.UserReference> smokeSignalReferences;
            Utils.FindUserReferences("hey @SmokeSignal:altemail=peter.p@bingo.com or @peterp. please answer the question", users,
                out lazyReferences, out immediateReferences, out smokeSignalReferences);

            Assert.AreEqual(1, lazyReferences.Count);
            Assert.AreEqual(0, immediateReferences.Count);
            Assert.AreEqual(1, smokeSignalReferences.Count);
            Assert.AreEqual("SmokeSignal:altemail=peter.p@bingo.com", smokeSignalReferences[0].NearByText);
        }

        /// <summary>
        ///A test for FindUserReferences
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NotifierLibrary.dll")]
        public void FindSmokeSignalCommands_Test()
        {
            List<CampfireState.UserInfo> users = new List<CampfireState.UserInfo>();
            users.Add(new CampfireState.UserInfo(1, "Peter Potrebic", "pp@be.com", ""));
            users.Add(new CampfireState.UserInfo(2, "Steve Johnson", "pj@be.com", ""));
            users.Add(new CampfireState.UserInfo(3, "Lindy Johnson", "lj@be.com", ""));

            IList<CampfireState.UserReference> lazyReferences;
            IList<CampfireState.UserReference> immediateReferences;
            IList<CampfireState.UserReference> smokeSignalReferences;
            Utils.FindUserReferences("hey @peter or @stevej please answer the question. And @SmokeSignal:Delay=5",
                users, out lazyReferences, out immediateReferences, out smokeSignalReferences);

            Assert.AreEqual(1, smokeSignalReferences.Count);
        }

        /// <summary>
        ///A test for BuildAbbreviations
        ///</summary>
        [TestMethod()]
        public void BuildAbbreviationsOneWordTest()
        {
            string[] words = { "Peter" };
            IList<string> abbrevs = Utils.BuildAbbreviations(words);

            Assert.AreEqual(1, abbrevs.Count);
            Assert.IsTrue(abbrevs.Any(s => s == "Peter".ToLowerInvariant()));
        }

        /// <summary>
        ///A test for BuildAbbreviations
        ///</summary>
        [TestMethod()]
        public void BuildAbbreviationsSingleLetterTest()
        {
            string[] words = { "P", "John" };
            IList<string> abbrevs = Utils.BuildAbbreviations(words);

            Assert.AreEqual(2, abbrevs.Count);
            Assert.IsTrue(abbrevs.Any(s => s == "pjohn".ToLowerInvariant()));
            Assert.IsTrue(abbrevs.Any(s => s == "john".ToLowerInvariant()));
        }

        /// <summary>
        ///A test for BuildAbbreviations
        ///</summary>
        [TestMethod()]
        public void BuildAbbreviationsTwoWordTest()
        {
            string[] words = { "Peter", "Potrebic" };
            IList<string> abbrevs = Utils.BuildAbbreviations(words);
            Assert.AreEqual(5, abbrevs.Count);
            Assert.IsTrue(abbrevs.Any(s => s == "Peter".ToLowerInvariant()));
            Assert.IsTrue(abbrevs.Any(s => s == "Potrebic".ToLowerInvariant()));
            Assert.IsTrue(abbrevs.Any(s => s == "PPotrebic".ToLowerInvariant()));
            Assert.IsTrue(abbrevs.Any(s => s == "PeterP".ToLowerInvariant()));
            Assert.IsTrue(abbrevs.Any(s => s == "PeterPotrebic".ToLowerInvariant()));
        }

        /// <summary>
        ///A test for BuildAbbreviations
        ///</summary>
        [TestMethod()]
        public void BuildAbbreviationsThreeWordTest()
        {
            string[] words = { "Peter", "John", "Potrebic" };
            IList<string> abbrevs = Utils.BuildAbbreviations(words);
            Assert.AreEqual(20, abbrevs.Count);
            Assert.IsTrue(abbrevs.Any(s => s == "PJP".ToLowerInvariant()));
            Assert.IsTrue(abbrevs.Any(s => s == "PeterJohnPotrebic".ToLowerInvariant()));
            Assert.IsTrue(abbrevs.Any(s => s == "PeterJPotrebic".ToLowerInvariant()));
            Assert.IsTrue(abbrevs.Any(s => s == "PJPotrebic".ToLowerInvariant()));
            Assert.IsTrue(abbrevs.Any(s => s == "Peter".ToLowerInvariant()));
            Assert.IsTrue(abbrevs.Any(s => s == "PeterJ".ToLowerInvariant()));
            Assert.IsTrue(abbrevs.Any(s => s == "PeterJohn".ToLowerInvariant()));
            Assert.IsTrue(abbrevs.Any(s => s == "PeterJohnP".ToLowerInvariant()));
        }

        /// <summary>
        ///A test for FindSentenceContaining
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NotifierLibrary.dll")]
        public void FindSentenceContainingTestValid()
        {
            string[] text = new string[] {
                "This is a (target) test for message without any real sentences",
                "This is a (target) test for message.",
                "This is a (target) test. for message",
                "This is a (target. ) test for message",
                "A prelim sentence. This is a target message.",
                "A prelim sentence. This is a target message. And a final sentence.",
                "Many sentences. Heres another. A prelim sentence. This is a target message.",
                "Many sentences. Heres another.   This is a target message.   And a final sentence. More, more.",
                "target: some stuff. And more stuff.",
                "Let's go team! Hey @target, when you releasing doing 1.0? Please let us know asap",
            };
            string[] expected = new string[] {
                text[0],
                text[1],
                "This is a (target) test.",
                "This is a (target.",
                "This is a target message.",
                "This is a target message.",
                "This is a target message.",
                "This is a target message.",
                "target: some stuff.",
                "Hey @target, when you releasing doing 1.0?"
            };

            string target = "target";

            Assert.AreEqual(text.Length, expected.Length);
            for (int i = 0; i < text.Length; i++)
            {
                string actual = Utils_Accessor.FindSentenceContaining(text[i], target);
                Assert.AreEqual(expected[i], actual, i.ToString());
            }
        }
    }
}
