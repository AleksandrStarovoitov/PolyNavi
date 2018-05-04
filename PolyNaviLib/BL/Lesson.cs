using System;
using System.Collections.Generic;
using System.Text;
using SQLiteNetExtensions.Attributes;

namespace PolyNaviLib.BL
{
	public class Lesson : BusinessEntity
	{
		public string Subject { get; set; }  //Название пары
		public string Timestr { get; set; }     //Время пары
		public string Building { get; set; } //Корпус
		public string Room { get; set; }     //Номер аудитории
		public string Groups { get; set; }   //Номера групп
		public string Type { get; set;  }      //Лекция/практика/лабораторные
		public bool Last { get; set; }		//Последний в списке

		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }

		[ManyToOne]
		public Day Day { get; set; }

		[ForeignKey(typeof(Day))]
		public int DayID { get; set; }

		public Lesson()
		{

		}
	}
}
