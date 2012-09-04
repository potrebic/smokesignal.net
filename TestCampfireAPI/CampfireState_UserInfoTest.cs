// Copyright 2012 Peter Potrebic.  All rights reserved.

using NotifierLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace TestCampfireAPI
{
    
    
    /// <summary>
    ///This is a test class for CampfireState_UserInfoTest and is intended
    ///to contain all CampfireState_UserInfoTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CampfireState_UserInfoTest
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
        ///A test for NickName
        ///</summary>
        [TestMethod()]
        public void NickName_Basic_Test()
        {
            string name = string.Empty;
            CampfireState.UserInfo target = new CampfireState.UserInfo(0, "Jack Nicklaus", "x", "x");
            Assert.AreEqual("JackN", target.NickName);
        }
        /// <summary>
        ///A test for NickName
        ///</summary>
        [TestMethod()]
        public void NickName_NoLastName_Test()
        {
            string name = string.Empty;
            CampfireState.UserInfo target = new CampfireState.UserInfo(0, "Cher", "x", "x");
            Assert.AreEqual("Cher", target.NickName);
        }
        /// <summary>
        ///A test for NickName
        ///</summary>
        [TestMethod()]
        public void NickName_MiddleInitial_Test()
        {
            string name = string.Empty;
            CampfireState.UserInfo target = new CampfireState.UserInfo(0, "George W. Bush", "x", "x");
            Assert.AreEqual("GeorgeB", target.NickName);
        }
        /// <summary>
        ///A test for NickName
        ///</summary>
        [TestMethod()]
        public void NickName_TwoMiddleNames_Test()
        {
            string name = string.Empty;
            CampfireState.UserInfo target = new CampfireState.UserInfo(0, "Peter John Paul Frank", "x", "x");
            Assert.AreEqual("PeterF", target.NickName);
        }
    }
}
