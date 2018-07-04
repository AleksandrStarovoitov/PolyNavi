using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolyNaviLib.BL
{
    public class Group : BusinessEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public string Type { get; set; }
        public int Kind { get; set; }
        public string Spec { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public Faculty Faculty { get; set; }

        //[ManyToOne]
        //public GroupRoot GroupRoot { get; set; }
        //[ForeignKey(typeof(GroupRoot))]
        //public int GroupRootId { get; set; }

        //[OneToOne]
        //public WeekRoot WeekRoot { get; set; }
        //[ForeignKey(typeof(WeekRoot))]
        //public int WeekRootID { get; set; }

        public Group()
        {
            Faculty = new Faculty();
        }

        [ManyToOne]
        public Lesson Lesson { get; set; }

        [ForeignKey(typeof(Lesson))]
        public int LessonID { get; set; }
    }

    public class GroupRoot : BusinessEntity
    {
        //[OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Group> Groups { get; set; }

        public string DummyStringToWorkaroundSQliteNetBug { get; set; }

        public GroupRoot()
        {
            Groups = new List<Group>();
        }
    }
}
