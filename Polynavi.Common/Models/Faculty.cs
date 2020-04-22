using SQLiteNetExtensions.Attributes;

namespace Polynavi.Common.Models
{
    public class Faculty : Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Abbr { get; set; }

        [OneToOne]
        public Group Group { get; set; }

        [ForeignKey(typeof(Group))]
        public int GroupID { get; set; }
    }
}
