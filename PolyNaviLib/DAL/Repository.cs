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

		const int CacheWeeks = 2;

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

		//TODO проверка сети и вывод на экран сообщения, если сети нет
		public async Task<Schedule> GetScheduleAsync(string groupNumber)
		{
			return await LoadScheduleFromWebAsync(groupNumber);
			//Schedule schedule = null;
			//if (await database.IsEmptyAsync<Week>())
			//{
			//	schedule = await LoadScheduleFromWebAsync(groupNumber);
			//	foreach (var week in schedule.Weeks)
			//	{
			//		await database.SaveItemAsync(week);
			//	}
			//}
			//else
			//{
			//	schedule = new Schedule();
			//	var weeks = await database.GetOrderedItemsAsync<Week, DateTime>(x => x.Days[0].Date);
			//	int expired = 0;
			//	foreach (var week in weeks)
			//	{
			//		if (week.IsExpired())
			//		{
			//			await database.DeleteItemAsync(week);
			//			++expired;
			//		}
			//		else
			//		{
			//			schedule.Weeks.Add(week);
			//		}
			//	}
			//	if (expired > 0)
			//	{
			//		var newWeeks = await LoadWeeksFromWebAsync(DateTime.Now, expired);
			//		foreach (var week in newWeeks)
			//		{
			//			database.SaveItemAsync(week);
			//			schedule.Weeks.Add(week);
			//		}
			//	}
			//}
			//return schedule;
		}

		private async Task<Schedule> LoadScheduleFromWebAsync(string groupNumber)
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
