using System;
using System.Collections.Generic;
using System.Text;

using SQLiteNetExtensions.Attributes;

namespace PolyNaviLib.BL
{
    public class Week : BusinessEntity
    {
        public DateTime Date_Start { get; set; }
        public DateTime Date_End { get; set; }
        public bool Is_Odd { get; set; }

        [OneToOne]
        public WeekRoot WeekRoot { get; set; }

        [ForeignKey(typeof(WeekRoot))]
        public int WeekRootID { get; set; }

        public Week()
        {

        }

        public bool DateEqual(DateTime rhs)
        {
            return rhs.Date >= Date_Start &&
                   rhs.Date <= Date_End;
        }

        public bool IsExpired()
        {
            return DateTime.Now.Date > Date_End.Date;
        }
    }
}
