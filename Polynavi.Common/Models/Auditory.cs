using SQLiteNetExtensions.Attributes;

namespace Polynavi.Common.Models
{
    public class Auditory : Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public Building Building { get; set; }

        [ManyToOne]
        public Lesson Lesson { get; set; }

        [ForeignKey(typeof(Lesson))]
        public int LessonID { get; set; }

        public Auditory()
        {
            Building = new Building();
        }
    }
}
