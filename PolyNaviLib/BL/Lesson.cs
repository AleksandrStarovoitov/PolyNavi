using System;
using System.Collections.Generic;
using SQLiteNetExtensions.Attributes;

namespace PolyNaviLib.BL
{
    public class Lesson : BusinessEntity
    {
        public string Subject { get; set; }
        public string Subject_Short { get; set; }
        public int Type { get; set; }
        public string Additional_Info { get; set; }
        public DateTime Time_Start { get; set; }
        public DateTime Time_End { get; set; }
        public int Parity { get; set; }
        public string Lms_Url { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public TypeObj TypeObj { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Group> Groups { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Teacher> Teachers { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Auditory> Auditories { get; set; }

        [ManyToOne]
        public Day Day { get; set; }

        [ForeignKey(typeof(Day))]
        public int DayID { get; set; }

        public Lesson()
        {
            TypeObj = new TypeObj();
            Groups = new List<Group>();
            Teachers = new List<Teacher>();
            Auditories = new List<Auditory>();
        }
    }
}
