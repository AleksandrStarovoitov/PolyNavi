using System;
using System.Collections.Generic;
using System.Text;

namespace PolyNaviLib.BL
{
	public class Week : BusinessEntity
	{
		public List<Day> Days { get; set; } //Дни

		public Week()
		{
			Days = new List<Day>();
		}
	}
}
