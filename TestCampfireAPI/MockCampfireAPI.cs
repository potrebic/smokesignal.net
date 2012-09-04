// Copyright 2012 Peter Potrebic.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestCampfireAPI
{
    class MockCampfireAPI : CampfireAPI.ICampfireAPI
    {
        public List<CampfireAPI.Room> FakeRooms;
        public List<CampfireAPI.User> FakeUsers;
        public Dictionary<int, List<CampfireAPI.Message>> MessagesInRoom;

        public MockCampfireAPI()
        {
            FakeRooms = new List<CampfireAPI.Room>();
            FakeUsers = new List<CampfireAPI.User>();
            MessagesInRoom = new Dictionary<int, List<CampfireAPI.Message>>();
        }

        #region ICampfileAPI Members

        public List<CampfireAPI.Room> Rooms()
        {
            return this.FakeRooms;
        }

        public List<CampfireAPI.Message> RecentMessages(int roomId, CampfireAPI.Message.MType messageTypes)
        {
            return RecentMessages(roomId, 0, messageTypes);
        }

        public List<CampfireAPI.Message> RecentMessages(int roomId, int sinceMessageId, CampfireAPI.Message.MType messageTypes)
        {
            List<CampfireAPI.Message> filtered = new List<CampfireAPI.Message>();
            List<CampfireAPI.Message> all = MessagesInRoom[roomId];
            List<CampfireAPI.Message> toRemove = new List<CampfireAPI.Message>();

            if (all == null)
            {
                return filtered;
            }

            foreach (CampfireAPI.Message m in all)
            {
                if ((m.Id > sinceMessageId) && ((m.Type | messageTypes) != 0))
                {
                    filtered.Add(m);
                    toRemove.Add(m);
                }
            }

            foreach (CampfireAPI.Message m in toRemove)
            {
                all.Remove(m);
            }

            return filtered;
        }

        public CampfireAPI.User GetUser(int userId)
        {
            return this.FakeUsers.Find(u => u.Id == userId);
        }

        public CampfireAPI.Room GetRoom(int roomId)
        {
            return this.FakeRooms.Find(r => r.Id == roomId);
        }

        public bool EnterRoom(int roomId)
        {
            return this.FakeRooms.Exists(r => r.Id == roomId);
        }

        public List<CampfireAPI.User> UsersInRoom(int roomId)
        {
            throw new NotImplementedException();
        }

        public bool UpdateTopic(int roomId, string topic)
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}
