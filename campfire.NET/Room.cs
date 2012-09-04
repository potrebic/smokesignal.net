// Copyright 2012 Peter Potrebic.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace CampfireAPI
{
    public class Room
    {
        public string Name;
        public int Id;

        /// <summary>
        /// Used for text purposes
        /// </summary>
        public Room(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public Room(XElement elem)
        {
            this.Name = elem.Element("name").Value;

            int id;
            if (Int32.TryParse(elem.Element("id").Value, out id))
            {
                this.Id = id;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", this.Name, this.Id);
        }
    }
}
