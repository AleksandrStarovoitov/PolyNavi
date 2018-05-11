using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PolyNaviLib.DAL;

namespace PolyNaviLib.BL
{
	public enum Weeks
	{
		Current, //Текущая неделя
		Next,     //Следующая неделя
	}

	public enum Days
	{
		Monday,
		Tuesday,
		Wednesday,
		Thursday,
		Friday,
		Saturday,
		Sunday,
	}

	public class PolyManager
	{
		Repository repository;

		private PolyManager()
		{
		}

		private async Task<PolyManager> InitializeAsync(string dbPath, INetworkChecker checker)
		{
			repository = await Repository.CreateAsync(dbPath, checker);
			return this;
		}

		public static Task<PolyManager> CreateAsync(string dbPath, INetworkChecker checker)
		{
			var manager = new PolyManager();
			return manager.InitializeAsync(dbPath, checker);
		}

		//Получить неделю
		public async Task<Week> GetWeekAsync(Weeks w, string groupNumber)
		{
			List<Week> weeks = await repository.GetScheduleAsync(groupNumber);

			return weeks[(int)w];
		}

		//Получить день
		public async Task<Day> GetDayAsync(Weeks w, Days d, string groupNumber)
		{
			List<Week> weeks = await repository.GetScheduleAsync(groupNumber);

			return weeks[(int)w].Days[(int)d];
		}

		Nito.AsyncEx.AsyncLock mutex = new Nito.AsyncEx.AsyncLock();
		//Получить расписание на неделю
		public async Task<List<Day>> GetScheduleByWeekAsync(Weeks w, string groupNumber)
		{
			List<Week> weeks;
			using (await mutex.LockAsync())
			{
				weeks = await repository.GetScheduleAsync(groupNumber);
			}

			var week = weeks[(int)w];
			List<Day> days = new List<Day>();
			foreach (var d in week.Days)
			{
				days.Add(d);
			}
			return days;
		}

	}
}
