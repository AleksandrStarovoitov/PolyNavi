using SQLiteNetExtensions.Attributes;

namespace Polynavi.Common.Models
{
    public class TypeObj : Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Abbr { get; set; }

        [OneToOne]
        public Lesson Lesson { get; set; }

        [ForeignKey(typeof(Lesson))]
        public int LessonID { get; set; }
    }
}
