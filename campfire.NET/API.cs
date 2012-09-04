// Copyright 2012 Peter Potrebic.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using CampfireAPI;
using System.Xml.Linq;
using System.Xml;

namespace CampfireAPI
{
 
    public class API : ICampfireAPI
    {
        private ICampfireToXml requestor;

        public API(string campfireName, string authToken)
        {
            requestor = new CampfireToXml(campfireName, authToken);
        }

        public API(ICampfireToXml lowLevel)
        {
            requestor = lowLevel;
        }

        public List<Room> Rooms()
        {
            string cmd = "/rooms.xml";
            
            XDocument xdoc = requestor.Doit(cmd);
            var rooms = from elem in xdoc.Descendants() where elem.Name == "room" select new Room(elem);

            return rooms.ToList();
        }

        /// <summary>
        /// Only returns the 'Members', not any guests.
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        public List<User> UsersInRoom(int roomId)
        {
            // /room/#{id}.xml
            string cmd = string.Format("/room/{0}.xml", roomId);
            XDocument xdoc = requestor.Doit(cmd);

            var users = from elem in xdoc.Descendants() where elem.Name == "user" && elem.Element("type").Value == "Member" select new User(elem);

            return users.ToList();
        }

        public Room GetRoom(int roomId)
        {
            // /room/#{id}.xml
            string cmd = string.Format("/room/{0}.xml", roomId);
            XDocument xdoc = requestor.Doit(cmd);

            var rooms = from elem in xdoc.Descendants() where elem.Name == "room" select new Room(elem);

            return rooms.FirstOrDefault();
        }

        public List<Message> RecentMessages(int roomId, Message.MType messageTypes)
        {
            return RecentMessages(roomId, 0, messageTypes);
        }

        public List<Message> RecentMessages(int roomId, int sinceMessageId, Message.MType messageTypes)
        {
            // GET /room/#{id}/recent.xml
            string cmd = string.Format("/room/{0}/recent.xml", roomId);
            if (sinceMessageId > 0)
            {
                cmd = string.Format("{0}?since_message_id={1}", cmd, sinceMessageId);
            }

            XDocument xdoc = requestor.Doit(cmd);

            // get all the messages
            var messages = from elem in xdoc.Descendants() where elem.Name == "message" select new Message(elem);

            // now filter out ones NOT matching 'messageTypes'
            messages = from msg in messages where ((msg.Type & messageTypes) != 0) select msg;

            List<Message> msgs = messages.ToList();

            return msgs;
        }

        public User GetUser(int userId)
        {
            string cmd = string.Format("/users/{0}.xml", userId);

            XDocument xdoc = requestor.Doit(cmd);

            var users = from elem in xdoc.Descendants() where elem.Name == "user" select new User(elem);

            return users.FirstOrDefault();
        }

        public bool UpdateTopic(int roomId, string topic)
        {
            try
            {
                HttpStatusCode status;
                XDocument xdoc = requestor.Doit(string.Format("/room/{0}.xml", roomId), "PUT", out status);
                return status == HttpStatusCode.OK;
            }
            catch (WebException ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Currently never used by SmokeSignal functionality
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        public bool EnterRoom(int roomId)
        {
            try
            {
                HttpStatusCode status;
                XDocument xdoc = requestor.Doit(string.Format("/room/{0}/join.xml", roomId), "POST", out status);
                return status == HttpStatusCode.OK;
            }
            catch (WebException ex)
            {
                return false;
            }
        }
    }
}
