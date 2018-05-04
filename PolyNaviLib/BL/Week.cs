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

		public Week()
		{
			Days = new List<Day>();
		}
	}
}
