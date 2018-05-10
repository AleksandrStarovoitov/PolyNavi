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
			if (checker.Check() == false)
			{
				throw new NetworkException("No internet connection");
			}
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




























		////TODO проверка сети и вывод на экран сообщения, если сети нет
		//public async Task<List<Week>> GetScheduleAsync(string groupNumber)
		//{
		//	//return await LoadScheduleFromWebAsync(groupNumber);
		//	//Schedule schedule = null;
		//	List<Week> weeks = new List<Week>();
		//	var items = await database.GetItemsAsync<Week>();
		//	if (await database.IsEmptyAsync<Week>())
		//	{
		//		weeks = await LoadScheduleFromWebAsync(groupNumber);
		//		foreach (var week in weeks)
		//		{
		//			await database.SaveItemAsync(week);
		//		}
		//	}
		//	else
		//	{
		//		//schedule = new Schedule();
		//		var weeksFromDB = await database.GetOrderedItemsAsync<Week, DateTime>(x => x.Days[0].Date);
		//		int expired = 0;
		//		foreach (var week in weeksFromDB)
		//		{
		//			if (week.IsExpired())
		//			{
		//				await database.DeleteItemAsync(week);
		//				++expired;
		//			}
		//			else
		//			{
		//				weeks.Add(week);
		//			}
		//		}
		//		if (expired > 0)
		//		{
		//			var newWeeks = await LoadExpiredWeeksFromWebAsync(DateTime.Now, expired);
		//			foreach (var week in newWeeks)
		//			{
		//				await database.SaveItemAsync(week); 
		//				weeks.Add(week);
		//			}
		//		}
		//	}
		//	return weeks;
		//}

		//private async Task<List<Week>> LoadScheduleFromWebAsync(string groupNumber)
		//{
		//	HtmlDocument htmlDoc;
		//	var weeks = new List<Week>();
		//	DateTime weekDate = DateTime.Now;
		//	string weekDateStr;

		//	//Поиск группы
		//	HtmlDocument htmlDocSearch = await HtmlLoader.LoadHtmlDocumentAsync(baseLink + groupNumber);
		//	groupLink = ScheduleBuilder.GetScheduleLink(htmlDocSearch); //Получили ссылку

		//	for (int i = 0; i < CacheWeeks; ++i)
		//	{
		//		var week = new Week();

		//		weekDateStr = weekDate.ToString("yyyy-M-d", new CultureInfo("ru-RU"));

		//		htmlDoc = await HtmlLoader.LoadHtmlDocumentAsync(groupLink + "?date=" + weekDateStr);
		//		week = ScheduleBuilder.BuildSchedule(htmlDoc);
		//		weeks.Add(week);

		//		weekDate = weekDate.AddDays(1);
		//		while (weekDate.DayOfWeek != DayOfWeek.Monday)
		//			weekDate = weekDate.AddDays(1);
		//	}
		//	return weeks;

		//	////Поиск группы
		//	//HtmlDocument htmlDocSearch = await HtmlLoader.LoadHtmlDocumentAsync(baseLink + groupNumber);
		//	//groupLink = ScheduleBuilder.GetScheduleLink(htmlDocSearch); //Получили ссылку

		//	////Загрузка текущей недели
		//	//HtmlDocument htmlDoc = await HtmlLoader.LoadHtmlDocumentAsync(groupLink);
		//	//Schedule schedule = ScheduleBuilder.BuildSchedule(htmlDoc);

		//	////Дата следующей недели
		//	//DateTime nextMondayDate = DateTime.Now.AddDays(1);
		//	//while (nextMondayDate.DayOfWeek != DayOfWeek.Monday)
		//	//	nextMondayDate = nextMondayDate.AddDays(1);
		//	//string nextWeekDate = nextMondayDate.ToString("yyyy-M-d", new CultureInfo("ru-RU"));

		//	////Загрузка следующей недели
		//	//HtmlDocument htmlDocNextWeek = await HtmlLoader.LoadHtmlDocumentAsync(groupLink + "?date=" + nextWeekDate);
		//	//Schedule scheduleNextWeek = ScheduleBuilder.BuildSchedule(htmlDocNextWeek);
		//	//schedule.Weeks.Add(scheduleNextWeek.Weeks[0]);

		//	//return schedule;
		//}

		//private async Task<List<Week>> LoadExpiredWeeksFromWebAsync(DateTime weekDate, int expired)
		//{
		//	var weeks = new List<Week>();
		//	HtmlDocument htmlDocNextWeek;
		//	string nextWeekDate;

		//	int weeksStored = CacheWeeks - expired;
		//	weekDate.AddDays(weeksStored  * 7); //Переходим на ту неделю, с которой нужно начать загрузку

		//	for (int i = 0; i < expired; ++i)
		//	{
		//		var week = new Week();

		//		nextWeekDate = weekDate.ToString("yyyy-M-d", new CultureInfo("ru-RU"));

		//		htmlDocNextWeek = await HtmlLoader.LoadHtmlDocumentAsync(groupLink + "?date=" + nextWeekDate);
		//		week = ScheduleBuilder.BuildSchedule(htmlDocNextWeek);
		//		weeks.Add(week);

		//		weekDate = weekDate.AddDays(1);
		//		while (weekDate.DayOfWeek != DayOfWeek.Monday)
		//			weekDate = weekDate.AddDays(1);
		//	}
		//	return weeks;
		//}
	}
}
