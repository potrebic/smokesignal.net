// Copyright 2012 Peter Potrebic.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;
using System.Xml.Linq;

namespace CampfireAPI
{
    public class Message
    {
        [Flags]
        public enum MType {
            TextMessage = 1,
            EnterMessage = 2,
            Unknown = 4,
        }

        public int UserId;
        public string Body;
        public int RoomId;
        public int Id;
        public MType Type;
        public DateTime PostedAt;

        /// <summary>
        /// Constructors used for test purposes.
        /// </summary>
        public Message(int id, MType type, int roomId, int userId, string msg)
        {
            this.Id = id;
            this.Type = type;
            this.RoomId = roomId;
            this.UserId = userId;
            this.Body = msg;
            this.PostedAt = DateTime.Now;
        }

        public Message(XElement element)
        {
            this.Body = element.Element("body").Value;

            switch (element.Element("type").Value)
            {
                case "TextMessage":
                case "PasteMessage":
                    this.Type = MType.TextMessage;
                    break;
                case "EnterMessage":
                    this.Type = MType.EnterMessage;
                    break;
                default:
                    this.Type = MType.Unknown;
                    break;
            }

            Int32.TryParse(element.Element("id").Value, out this.Id);
            Int32.TryParse(element.Element("user-id").Value, out this.UserId);
            Int32.TryParse(element.Element("room-id").Value, out this.RoomId);

            // <created-at type="datetime">2010-04-15T11:03:08Z</created-at>
            string dtString = element.Element("created-at").Value;
            DateTime dt;
            if (!DateTime.TryParse(dtString, out dt))
            {
                dt = System.Xml.XmlConvert.ToDateTime(dtString, "yyyy-MM-dd'T'HH:mm:ssZ");
            }
            this.PostedAt = dt;
        }

        public string TrimmedBody
        {
            get
            {
                string msg = this.Body;
                if (msg.Length > 20)
                {
                    msg = msg.Substring(0, 20) + "...";
                }
                return msg;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}: {1} (user:{2})", this.Type, this.TrimmedBody, this.UserId);
        }
    }
}
