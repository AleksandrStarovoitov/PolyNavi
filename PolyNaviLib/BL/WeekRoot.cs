using System;
using System.Collections.Generic;
using SQLiteNetExtensions.Attributes;

namespace PolyNaviLib.BL
{
    public class WeekRoot : BusinessEntity
    {
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public Week Week { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Day> Days { get; set; }

        public DateTime LastUpdated { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public Group Group { get; set; }

        public WeekRoot()
        {
            Days = new List<Day>();
            Week = new Week();
            Group = new Group();
        }
    }
}
