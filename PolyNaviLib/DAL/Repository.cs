using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using PolyNaviLib.BL;
using PolyNaviLib.SL;
using PolyNaviLib.DL;
using System.Globalization;
using System.Linq;

namespace PolyNaviLib.DAL
{
	public class Repository
	{
		//Номер группы....
		private string baseLink = @"http://ruz.spbstu.ru/search/groups?q=";
		private string groupLink;

		SQLiteDatabase database;
		INetworkChecker checker;
		ISettingsProvider settings;

		const int CacheWeeks = 2;

		private Repository()
		{
		}

		private async Task<Repository> InitializeAsync(string dbPath, INetworkChecker checker, ISettingsProvider settings)
		{
			database = new SQLiteDatabase(dbPath);
			await database.CreateTableAsync<Week>();
			await database.CreateTableAsync<Day>();
			await database.CreateTableAsync<Lesson>();
			this.checker = checker;
			this.settings = settings;
			await RemoveExpiredWeeksAsync();
			return this;
		}

		public static Task<Repository> CreateAsync(string dbPath, INetworkChecker networkChecker, ISettingsProvider settings)
		{
			var repo = new Repository();
			return repo.InitializeAsync(dbPath, networkChecker, settings);
		}

		public async Task<Week> GetWeekAsync(DateTime weekDate)
		{
			if (await database.IsEmptyAsync<Week>())
			{
				var week = (await LoadScheduleFromWebAsync()).Where(w => w.DateEqual(weekDate)).Single();
				await database.SaveItemAsync(week);
				return week;
			}
			else
			{
				var weekFromDb = (await database.GetItemsAsync<Week>()).Where(w => w.DateEqual(weekDate)).SingleOrDefault();
				if (weekFromDb == null)
				{
					var newWeek = (await LoadScheduleFromWebAsync()).Where(w => w.DateEqual(weekDate)).Single();
					await database.SaveItemAsync(newWeek);
					return newWeek;
				}
				else
				{
					return weekFromDb;
				}
			}
		}

		private async Task RemoveExpiredWeeksAsync()
		{
			await database.DeleteItemsAsync<Week>(w => w.IsExpired());
		}

		//public async Task<List<Week>> GetScheduleAsync(string groupNumber)
		//{
		//	List<Week> weeks = new List<Week>();
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

		private async Task<List<Week>> LoadScheduleFromWebAsync()
		{
			HtmlDocument htmlDoc;
			var weeks = new List<Week>();
			DateTime weekDate = DateTime.Now;
			string weekDateStr;
			string groupNumber = settings["groupnumber"].ToString();

			//Поиск группы
			HtmlDocument htmlDocSearch = await HtmlLoader.LoadHtmlDocumentAsync(baseLink + groupNumber);
			groupLink = WeekBuilder.GetScheduleLink(htmlDocSearch); //Получили ссылку

			for (int i = 0; i < CacheWeeks; ++i)
			{
				var week = new Week();

				weekDateStr = weekDate.ToString("yyyy-M-d", new CultureInfo("ru-RU"));

				htmlDoc = await HtmlLoader.LoadHtmlDocumentAsync(groupLink + "?date=" + weekDateStr);
				week = WeekBuilder.BuildWeek(htmlDoc);
				weeks.Add(week);

				weekDate = weekDate.AddDays(1);
				while (weekDate.DayOfWeek != DayOfWeek.Monday)
					weekDate = weekDate.AddDays(1);
			}
			return weeks;
		}

		//private async Task<List<Week>> LoadExpiredWeeksFromWebAsync(DateTime weekDate, int expired)
		//{
		//	var weeks = new List<Week>();
		//	HtmlDocument htmlDocNextWeek;
		//	string nextWeekDate;

		//	int weeksStored = CacheWeeks - expired;
		//	weekDate.AddDays(weeksStored * 7); //Переходим на ту неделю, с которой нужно начать загрузку

		//	for (int i = 0; i < expired; ++i)
		//	{
		//		var week = new Week();

		//		nextWeekDate = weekDate.ToString("yyyy-M-d", new CultureInfo("ru-RU"));

		//		htmlDocNextWeek = await HtmlLoader.LoadHtmlDocumentAsync(groupLink + "?date=" + nextWeekDate);
		//		week = WeekBuilder.BuildWeek(htmlDocNextWeek);
		//		weeks.Add(week);

		//		weekDate = weekDate.AddDays(1);
		//		while (weekDate.DayOfWeek != DayOfWeek.Monday)
		//			weekDate = weekDate.AddDays(1);
		//	}
		//	return weeks;
		//}
	}
}
