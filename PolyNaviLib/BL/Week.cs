using SQLiteNetExtensions.Attributes;
using System;

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

        public bool ContainsDate(DateTime date)
        {
            return date.Date >= Date_Start &&
                   date.Date <= Date_End;
        }

        public bool IsExpired()
        {
            return DateTime.Now.Date > Date_End.Date;
        }
    }
}
