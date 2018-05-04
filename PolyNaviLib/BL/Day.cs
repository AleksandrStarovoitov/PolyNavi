using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolyNaviLib.BL
{
	public class Day : BusinessEntity
	{
		[ForeignKey(typeof(Week))]
		public int WeekID { get; set; }

		[OneToMany(CascadeOperations = CascadeOperation.All)]
		public List<Lesson> Lessons { get; set; } //Список пар
		public string Datestr { get; set; }          //Дата, день недели
		public DateTime Date { get; set; }

		[ManyToOne]
		public Week Week { get; set; }

		public Day()
		{
			Lessons = new List<Lesson>();
		}
	}
}