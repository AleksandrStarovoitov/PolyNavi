using SQLiteNetExtensions.Attributes;

namespace PolyNaviLib.BL
{
    public class TypeObj : BusinessEntity
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
