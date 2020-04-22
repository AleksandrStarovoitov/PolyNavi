using SQLiteNetExtensions.Attributes;

namespace Polynavi.Common.Models
{
    public class Building : Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Abbr { get; set; }
        public string Address { get; set; }

        [OneToOne]
        public Auditory Auditory { get; set; }

        [ForeignKey(typeof(Auditory))]
        public int AuditoryID { get; set; }
    }
}
