using System;
using System.Collections.Generic;
using System.Text;

using PolyNaviLib.DAL;

namespace PolyNaviLib.BL
{
	public class PolyManager
	{
		Repository repository;
		
		public enum weeks
		{
			current, //Текущая неделя
			next     //Следующая неделя
		}

		public enum days
		{
			monday,
			tuesday,
			wednesday,
			thursday,
			friday,
			saturday,
			sunday	   //,
			//current, //Может понадобится
			//next	   //Может понадобится
		}

		public PolyManager(string dbPath, INetworkChecker checker)
		{
			//TODO...
		}

		//Получить неделю
		public Week GetWeek(weeks w)
		{
			Schedule schedule = Repository.GetSchedule();

			return schedule.Weeks[(int)w];
		}

		//Получить день
		public Day GetDay(weeks w, days d)
		{
			Schedule schedule = Repository.GetSchedule();

			return schedule.Weeks[(int)w].Days[(int)d];
		}


		//Получить расписание на неделю
		public List<Lesson> GetScheduleByWeek(weeks w)
		{
			Schedule schedule = Repository.GetSchedule();

			var week = schedule.Weeks[(int)w];

			List<Lesson> lessons = new List<Lesson>();

			foreach (var d in week.Days)
			{
				foreach (var l in d.Lessons)
				{
					lessons.Add(l);
				}
			}

			return lessons;
		}

		//Получить расписание на день определенной недели
		public List<Lesson> GetScheduleByDay(weeks w, days d)
		{
			Schedule schedule = Repository.GetSchedule();

			var day = schedule.Weeks[(int)w].Days[(int)d];

			List<Lesson> lessons = new List<Lesson>();

			foreach (var l in lessons)
			{
				lessons.Add(l);
			}

			return lessons;
		}

	}
}
