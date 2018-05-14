using System;
using System.Collections.Generic;
using System.Text;

using SQLiteNetExtensions.Attributes;

namespace PolyNaviLib.BL
{
	public class Week : BusinessEntity
	{
		[OneToMany(CascadeOperations = CascadeOperation.All)]
		public List<Day> Days { get; set; } //Дни

		public string DummyStringToWorkaroundSQliteNetBug { get; set; }

		public Week()
		{
			Days = new List<Day>();
		}

		public Week(DateTime weekDate)
		{
			while (weekDate.Date.DayOfWeek != DayOfWeek.Monday)
			{
				weekDate = weekDate.AddDays(-1);
			}

			Days = new List<Day>() { new Day() { Date = weekDate }, new Day() { Date = weekDate.AddDays(1) }, new Day() { Date = weekDate.AddDays(2) }, new Day() { Date = weekDate.AddDays(3) }, new Day() { Date = weekDate.AddDays(4) }, new Day() { Date = weekDate.AddDays(5) }, new Day() { Date = weekDate.AddDays(6) } };
		}

		public bool IsExpired()
		{
			return DateTime.Now.Date > Days[6].Date;
			//return DateTime.Now.Date > Days.Last().Date.Date;
		}

		public bool DateEqual(DateTime rhs)
		{
			return rhs.Date >= Days.First().Date.Date &&
				   rhs.Date <= Days.Last().Date.Date;
		}
	}
}
