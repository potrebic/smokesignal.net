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
    public class SmokeSignalConfig : IXmlSerializable
    {
        #region static API

        private static string BackingStoreDir;

        public static string BackingStoreFile
        {
            get
            {
                return Path.Combine(BackingStoreDir, "SmokeSignalConfig.xml");
            }
        }

        public static SmokeSignalConfig Instance
        {
            get;
            private set;
        }

        public static bool Initialize(string storePath)
        {
            bool success = true;
            if (Instance == null)
            {
                Instance = Restore(storePath);

                if (Instance == null)
                {
                    // Data in the xml file was corrupt/unreadable
                    success = false;
                    Instance = new SmokeSignalConfig();
                }
            }
            return success;
        }

        private static SmokeSignalConfig Restore(string dirPath)
        {
            BackingStoreDir = dirPath;
            string fullPath = BackingStoreFile;

            SmokeSignalConfig smokeSignalInfo = null;
            bool error = false;

            if (!System.IO.File.Exists(fullPath))
            {
                // create & save a new file
                smokeSignalInfo = new SmokeSignalConfig();
                smokeSignalInfo.Save();
                return smokeSignalInfo;
            }

            Exception caughtException = null;
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(fullPath);

                XmlSerializer ser = new XmlSerializer(typeof(SmokeSignalConfig));
                using (StringReader reader = new StringReader(xmlDoc.OuterXml))
                {
                    smokeSignalInfo = (SmokeSignalConfig)ser.Deserialize(reader);
                    reader.Close();
                }
            }
            catch (XmlException ex) { caughtException = ex; }
            catch (IOException ex) { caughtException = ex; }
            catch (UnauthorizedAccessException ex) { caughtException = ex; }
            catch (System.Security.SecurityException ex) { caughtException = ex; }
            catch (InvalidOperationException ex) { caughtException = ex; }
            catch (ApplicationException ex) { caughtException = ex; }

            if (caughtException != null)
            {
                // xml file was corrupt. Make a backup and delete original
                Utils.TraceException(TraceLevel.Warning, caughtException, string.Format("SmokeSignalConfig.xml was corrupt. Making a backup and starting with a new file."));
                error = Utils.SafeBackupAndDelete(fullPath);
            }

            return smokeSignalInfo;
        }

        private static void SaveToStore(SmokeSignalConfig ss)
        {
            SmokeSignalConfig.SaveToStore(ss, BackingStoreDir);
        }

        public static void SaveToStore(SmokeSignalConfig ss, string dirPath)
        {
            string fullPath = Path.Combine(dirPath, "SmokeSignalConfig.xml");

            if (!System.IO.File.Exists(fullPath))
            {
                if (!System.IO.Directory.Exists(dirPath))
                {
                    System.IO.Directory.CreateDirectory(dirPath);
                }
            }

            XmlSerializer ser = new XmlSerializer(typeof(SmokeSignalConfig));

            System.IO.StreamWriter file = new System.IO.StreamWriter(fullPath);

            ser.Serialize(file, ss);
            file.Close();
        }

        #endregion

        #region persisted data

        public string CampfireName { get; private set; }
        public string CampfireToken { get; private set; }
        public int DelayBeforeSmokeSignalInMinutes { get; private set; }
        public bool SendNewUserEmail { get; private set; }
        public string ExtraEmailMessage { get; private set; }

        #endregion

        #region public API

        private SmokeSignalConfig()
        {
            this.CampfireName = string.Empty;
            this.CampfireToken = string.Empty;
            this.DelayBeforeSmokeSignalInMinutes = 2;
            this.SendNewUserEmail = false;
            this.ExtraEmailMessage = string.Empty;
        }

        public bool IsSmokeSignalConfiged()
        {
            return !string.IsNullOrEmpty(CampfireName) && !string.IsNullOrEmpty(CampfireToken);
        }

        private object saveLock = new object();
        /// <summary>
        /// (Thread Safe) Save state to persistant store
        /// </summary>
        public void Save()
        {
            lock (this.saveLock)
            {
                SmokeSignalConfig.SaveToStore(this);
            }
        }

        private object nameTokenLock = new object();


        /// <summary>
        /// (Thread Safe)
        /// </summary>
        /// <param name="name"></param>
        public void SetName(string name)
        {
            lock (this.nameTokenLock)
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentNullException("Campfire 'name' cannot be null or empty");
                }

                this.CampfireName = name;
            }
        }

        /// <summary>
        /// (Thread Safe)
        /// </summary>
        /// <param name="token"></param>
        public void SetToken(string token)
        {
            lock (this.nameTokenLock)
            {
                if (string.IsNullOrEmpty(token) || token.Trim() == "")
                {
                    throw new ArgumentNullException("Campfire 'token' cannot be null or empty");
                }

                this.CampfireToken = token.Trim();
            }
        }
        /// <summary>
        /// (Thread Safe)
        /// </summary>
        /// <param name="token"></param>
        public void SetDelay(int delay)
        {
            lock (this.nameTokenLock)
            {
                if (delay < 1 || delay > 10)
                {
                    throw new ArgumentNullException("Campfire 'delay' must be between 1 and 10, inclusive");
                }

                this.DelayBeforeSmokeSignalInMinutes = delay;
            }
        }
        /// <summary>
        /// (Thread Safe)
        /// </summary>
        /// <param name="token"></param>
        public void SetSendWelcomeEmail(bool sendEmail)
        {
            lock (this.nameTokenLock)
            {
                this.SendNewUserEmail = sendEmail;
            }
        }

        /// <summary>
        /// (Thread Safe)
        /// </summary>
        /// <param name="token"></param>
        public void SetExtraEmailMessage(string message)
        {
            lock (this.nameTokenLock)
            {
                this.ExtraEmailMessage = message;
            }
        }

        /// <summary>
        /// (Thread Safe)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="token"></param>
        public void SetAndSaveNameAndToken(string name, string token)
        {
            lock (this.nameTokenLock)
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentNullException("Campfire 'name' cannot be null or empty");
                }
                if (string.IsNullOrEmpty(token))
                {
                    throw new ArgumentNullException("Campfire 'token' cannot be null or empty");
                }

                this.CampfireName = name;
                this.CampfireToken = token;
                this.Save();
            }
        }

        #endregion

        #region IXmlSerializable

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();

            while (true)
            {
                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }
                switch (reader.LocalName)
                {
                    case "CampfireName":
                        this.CampfireName = reader.ReadElementContentAsString("CampfireName", "");
                        break;
                    case "CampfireToken":
                        this.CampfireToken = reader.ReadElementContentAsString("CampfireToken", "");
                        break;
                    case "DelayBeforeSmokeSignalInMinutes":
                        this.DelayBeforeSmokeSignalInMinutes = reader.ReadElementContentAsInt("DelayBeforeSmokeSignalInMinutes", "");
                        break;
                    case "SendNewUserEmail":
                        this.SendNewUserEmail = reader.ReadElementContentAsBoolean("SendNewUserEmail", "");
                        break;
                    case "ExtraEmailMessage":
                        this.ExtraEmailMessage = reader.ReadElementContentAsString("ExtraEmailMessage", "");
                        break;
                    
                    default:
                        if (reader.NodeType == XmlNodeType.Comment)
                        {
                            reader.Read();
                        }
                        else
                        {
                            reader.ReadElementContentAsString(reader.LocalName, "");
                        }
                        break;
                }
            }

            if (this.DelayBeforeSmokeSignalInMinutes < 1)
            {
                this.DelayBeforeSmokeSignalInMinutes = 1;
            }
            else if (this.DelayBeforeSmokeSignalInMinutes > 10)
            {
                this.DelayBeforeSmokeSignalInMinutes = 10;
            }

            //if (this.SmtpPort < 1)
            //{
            //    this.SmtpPort = 25;
            //}

            //if (string.IsNullOrEmpty(this.CampfireName.Trim()) || string.IsNullOrEmpty(this.CampfireToken.Trim()))
            //{
            //    throw new ApplicationException("Invalid SmokeSignalConfig.xml, missing CampfireName and/or CampfireToken fields");
            //}

            //if (string.IsNullOrEmpty(this.SmtpHost.Trim()))
            //{
            //    throw new ApplicationException("Invalid SmokeSignalConfig.xml, missing SmtpHost and/or SmtpUserName fields");
            //}

            reader.ReadEndElement();
        }
       
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("CampfireName", this.CampfireName);
            writer.WriteElementString("CampfireToken", this.CampfireToken);
            writer.WriteElementString("DelayBeforeSmokeSignalInMinutes", this.DelayBeforeSmokeSignalInMinutes.ToString());
            writer.WriteElementString("SendNewUserEmail", this.SendNewUserEmail.ToString().ToLower());
            writer.WriteElementString("ExtraEmailMessage", this.ExtraEmailMessage);
        }

        #endregion
    }
}
