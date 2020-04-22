using System;
using System.Collections.Generic;
using SQLiteNetExtensions.Attributes;

namespace Polynavi.Common.Models
{
    public class Day : Entity
    {
        public int Weekday { get; set; }
        public DateTime Date { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Lesson> Lessons { get; set; }

        [ManyToOne]
        public WeekRoot WeekRoot { get; set; }

        [ForeignKey(typeof(WeekRoot))]
        public int WeekRootID { get; set; }

        public Day()
        {
            Lessons = new List<Lesson>();
        }
    }
}
