using System;
using System.Collections.Generic;
using System.Text;

using PolyNaviLib.DAL;

namespace PolyNaviLib.BL
{
	public class PolyManager
	{
		Repository repository;
		
		public enum Weeks
		{
			Current, //Текущая неделя
			Next     //Следующая неделя
		}

		public enum Days
		{
			Monday,
			Tuesday,
			Wednesday,
			Thursday,
			Friday,
			Saturday,
			Sunday	   //,
			//Current, //Может понадобится
			//Next	   //Может понадобится
		}

		public PolyManager()
		{
			repository = new Repository();
		}

		public PolyManager(string dbPath, INetworkChecker checker)
		{
			//TODO...
		}

		//Получить неделю
		public Week GetWeek(Weeks w)
		{
			Schedule schedule = repository.GetSchedule();

			return schedule.Weeks[(int)w];
		}

		//Получить день
		public Day GetDay(Weeks w, Days d)
		{
			Schedule schedule = repository.GetSchedule();

			return schedule.Weeks[(int)w].Days[(int)d];
		}


		//Получить расписание на неделю
		public List<Lesson> GetScheduleByWeek(Weeks w)
		{
			Schedule schedule = repository.GetSchedule();

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
		public List<Lesson> GetScheduleByDay(Weeks w, Days d)
		{
			Schedule schedule = repository.GetSchedule();

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
