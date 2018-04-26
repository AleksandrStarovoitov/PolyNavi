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
		public Schedule GetSchedule()
		{
			if (false) //Есть в БД
			{

				return new Schedule();
			}
			else //Нет в БД
			{
				HtmlDocument htmlDoc = HtmlLoader.LoadHtmlDocument("addr"); //Где брать адрес?
				Schedule schedule = ScheduleBuilder.BuildSchedule(htmlDoc);

				return schedule;
			}
		}

	}
}
