using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolyNaviLib.BL
{
    public class WeekRoot : BusinessEntity
    {
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public Week Week { get; set; }
        
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Day> Days { get; set; }

        public string DummyStringToWorkaroundSQliteNetBug { get; set; }

        //[OneToOne]
        //public Group Group { get; set; }

        public WeekRoot()
        {
            Days = new List<Day>();
            Week = new Week();
        }
    }
}
