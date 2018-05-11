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

		public bool IsExpired()
		{
			//return DateTime.Now.Date > Days[6].Date;
			return DateTime.Now.Date > Days[Days.Count - 1].Date;
		}
	}
}
