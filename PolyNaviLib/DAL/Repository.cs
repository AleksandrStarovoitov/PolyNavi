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
				var week = (await LoadScheduleFromWebAsync(weekDate));
				await database.SaveItemAsync(week);
				return week;
			}
			else
			{
				var weekFromDb = (await database.GetItemsAsync<Week>()).Where(w => w.DateEqual(weekDate)).SingleOrDefault();
				if (weekFromDb == null)
				{
					var newWeek = (await LoadScheduleFromWebAsync(weekDate));
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

		private async Task<Week> LoadScheduleFromWebAsync(DateTime weekDate)
		{
			if (checker.Check() == false)
			{
				throw new NetworkException("No internet connection");
			}

			HtmlDocument htmlDoc;
			string groupNumber = settings["groupnumber"].ToString();

			//Поиск группы
			HtmlDocument htmlDocSearch = await HtmlLoader.LoadHtmlDocumentAsync(baseLink + groupNumber);
			groupLink = WeekBuilder.GetScheduleLink(htmlDocSearch); //Получили ссылку
			
			var week = new Week();
			string weekDateStr = weekDate.ToString("yyyy-M-d", new CultureInfo("ru-RU"));
			htmlDoc = await HtmlLoader.LoadHtmlDocumentAsync(groupLink + "?date=" + weekDateStr);
			week = WeekBuilder.BuildWeek(htmlDoc);

			return week;
		}
	}
}
