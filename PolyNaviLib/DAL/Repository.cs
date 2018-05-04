using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using PolyNaviLib.BL;
using PolyNaviLib.SL;
using PolyNaviLib.DL;

namespace PolyNaviLib.DAL
{
	public class Repository
	{
		//Номер группы....
		private string group = @"http://ruz.spbstu.ru/search/groups?q=";
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

		public async Task<Schedule> GetScheduleAsync()
		{
			if (false) //Есть в БД
			{

				return new Schedule();
			}
			else //Нет в БД
			{
				//Поиск группы
				//HtmlDocument htmlDocSearch = HtmlLoader.LoadHtmlDocument(group + "23537/1"); //Где брать адрес?
				HtmlDocument htmlDocSearch = await HtmlLoader.LoadHtmlDocumentAsync(group + "23537/1");
				groupLink = ScheduleBuilder.GetScheduleLink(htmlDocSearch); //Получили ссылку

				//HtmlDocument htmlDoc = HtmlLoader.LoadHtmlDocument(groupLink);
				HtmlDocument htmlDoc = await HtmlLoader.LoadHtmlDocumentAsync(groupLink);
				Schedule schedule = ScheduleBuilder.BuildSchedule(htmlDoc);

				return schedule;
			}
		}

	}
}
