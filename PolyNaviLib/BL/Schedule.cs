using System;
using System.Collections.Generic;
using System.Text;

namespace PolyNaviLib.BL
{
	public class Schedule
	{
		public List<Week> Weeks { get; set; } //Недели
			
		public Schedule()
		{
			Weeks = new List<Week>();
		}
	}
}
