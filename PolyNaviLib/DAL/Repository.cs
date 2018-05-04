using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using PolyNaviLib.BL;
using PolyNaviLib.SL;
using PolyNaviLib.DL;
using System.Globalization;

namespace PolyNaviLib.DAL
{
	public class Repository
	{
		//Номер группы....
		private string baseLink = @"http://ruz.spbstu.ru/search/groups?q=";
		private string groupLink;

		SQLiteDatabase database;
		INetworkChecker checker;

		private Repository()
		{
		}

		private async Task<Repository> InitializeAsync(string dbPath, INetworkChecker networkChecker)
		{
			database = new SQLiteDatabase(dbPath);
			await database.CreateTableAsync<Week>();
			await database.CreateTableAsync<Day>();
			await database.CreateTableAsync<Lesson>();
			checker = networkChecker;
			return this;
		}

		public static Task<Repository> CreateAsync(string dbPath, INetworkChecker networkChecker)
		{
			var repo = new Repository();
			return repo.InitializeAsync(dbPath, networkChecker);
		}

		public async Task<Schedule> GetScheduleAsync(string groupNumber)
		{
			if (false/*schedule != null*/) //Есть в БД
			{
				//return schedule;
			}
			else //Нет в БД
			{
				//Поиск группы
				HtmlDocument htmlDocSearch = await HtmlLoader.LoadHtmlDocumentAsync(baseLink + groupNumber);
				groupLink = ScheduleBuilder.GetScheduleLink(htmlDocSearch); //Получили ссылку

				//Загрузка текущей недели
				HtmlDocument htmlDoc = await HtmlLoader.LoadHtmlDocumentAsync(groupLink);
				Schedule schedule = ScheduleBuilder.BuildSchedule(htmlDoc);

				//Дата следующей недели
				DateTime nextMondayDate = DateTime.Now.AddDays(1);
				while (nextMondayDate.DayOfWeek != DayOfWeek.Monday)
					nextMondayDate = nextMondayDate.AddDays(1);
				string nextWeekDate = nextMondayDate.ToString("yyyy-M-d", new CultureInfo("ru-RU"));

				//Загрузка следующей недели
				HtmlDocument htmlDocNextWeek = await HtmlLoader.LoadHtmlDocumentAsync(groupLink + "?date=" + nextWeekDate);
				Schedule scheduleNextWeek = ScheduleBuilder.BuildSchedule(htmlDocNextWeek);
				schedule.Weeks.Add(scheduleNextWeek.Weeks[0]);

				return schedule;
			}
		}

	}
}
