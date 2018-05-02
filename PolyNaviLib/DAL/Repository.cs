using System;
using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;
using PolyNaviLib.BL;
using PolyNaviLib.SL;


namespace PolyNaviLib.DAL
{
	public class Repository
	{
		//Номер группы....
		private string group = @"http://ruz.spbstu.ru/search/groups?q=";
		private string groupLink;

		public Schedule GetSchedule()
		{
			if (false) //Есть в БД
			{

				return new Schedule();
			}
			else //Нет в БД
			{
				//Поиск группы
				HtmlDocument htmlDocSearch = HtmlLoader.LoadHtmlDocument(group + "23537/1"); //Где брать адрес?
				groupLink = ScheduleBuilder.GetScheduleLink(htmlDocSearch); //Получили ссылку

				HtmlDocument htmlDoc = HtmlLoader.LoadHtmlDocument(groupLink);
				Schedule schedule = ScheduleBuilder.BuildSchedule(htmlDoc);

				return schedule;
			}
		}

	}
}
