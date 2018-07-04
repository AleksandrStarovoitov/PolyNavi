using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolyNaviLib.BL
{
    public class Building : BusinessEntity
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
