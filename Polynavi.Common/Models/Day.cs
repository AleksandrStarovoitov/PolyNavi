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
        public WeekSchedule WeekRoot { get; set; }

        [ForeignKey(typeof(WeekSchedule))]
        public int WeekRootID { get; set; }

        public Day()
        {
            Lessons = new List<Lesson>();
        }
    }
}
