// Copyright 2012 Peter Potrebic.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CampfireAPI
{
    public interface ICampfireAPI
    {
        List<Room> Rooms();
        List<Message> RecentMessages(int roomId, Message.MType messageTypes);
        List<Message> RecentMessages(int roomId, int sinceMessageId, Message.MType messageTypes);
        User GetUser(int userId);
        Room GetRoom(int roomId);
        bool EnterRoom(int roomId);
        bool UpdateTopic(int roomId, string topic);
        List<User> UsersInRoom(int roomId);
    }
}
