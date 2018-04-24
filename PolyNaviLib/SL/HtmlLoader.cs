using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolyNaviLib.SL
{
	public class HtmlLoader
	{
		//Загрузка страницы расписания 
		public static HtmlDocument LoadHtmlDocument(string addr)
		{
			var web = new HtmlWeb();
			return web.Load(addr);
		}
	}
}
