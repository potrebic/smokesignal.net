// Copyright 2012 Peter Potrebic.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace CampfireAPI
{
    public class User
    {
        public User(string name, string emailAddr, int id)
        {
            this.Name = name;
            this.Email = emailAddr;
            this.Id = id;
            this.Type = UserType.Member;
        }

        public User(XElement element)
        {
            this.Email = element.Element("email-address").Value;
            this.Name = element.Element("name").Value;

            int value;

            Int32.TryParse(element.Element("id").Value, out value);
            this.Id = value;

            this.Type = element.Element("type").Value.ToLower() == "member" ? UserType.Member : UserType.Guest;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", this.Name, this.Id);
        }

        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }

        public enum UserType { Member, Guest };
        public UserType Type { get; set; }
    }
}
