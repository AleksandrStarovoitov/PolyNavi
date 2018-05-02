using System;
using System.Collections.Generic;
using System.Text;

namespace PolyNaviLib.BL
{
	public class Lesson
	{
		public string Subject { get; set; }  //Название пары
		public string Timestr { get; set; }     //Время пары
		public string Building { get; set; } //Корпус
		public string Room { get; set; }     //Номер аудитории
		public string Groups { get; set; }   //Номера групп
		public bool Last { get; set; }		//Последний в списке

		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }

		public Lesson()
		{

		}
	}
}
