using System;
using System.Collections.Generic;
using System.Text;

namespace PolyNaviLib.BL
{
	public class Day
	{
		public List<Lesson> Lessons { get; set; } //Список пар
		public string Datestr { get; set; }          //Дата, день недели
		public DateTime Date { get; set; }

		public Day()
		{
			Lessons = new List<Lesson>();
		}
	}
}