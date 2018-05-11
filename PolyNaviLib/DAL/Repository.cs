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
	}
}
