using System.Collections.Generic;
using SQLiteNetExtensions.Attributes;

namespace Polynavi.Common.Models
{
    public class Group : Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public string Type { get; set; }
        public int Kind { get; set; }
        public string Spec { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public Faculty Faculty { get; set; }

        [OneToOne]
        public WeekRoot WeekRoot { get; set; }

        [ForeignKey(typeof(WeekRoot))]
        public int WeekRootID { get; set; }

        [ManyToOne]
        public Lesson Lesson { get; set; }

        [ForeignKey(typeof(Lesson))]
        public int LessonID { get; set; }

        public Group()
        {
            Faculty = new Faculty();
        }
    }

    public class GroupRoot : Entity
    {
        public List<Group> Groups { get; set; }

        public string DummyStringToWorkaroundSQliteNetBug { get; set; }

        public GroupRoot()
        {
            Groups = new List<Group>();
        }
    }
}
