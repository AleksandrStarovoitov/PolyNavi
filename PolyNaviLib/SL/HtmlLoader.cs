using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PolyNaviLib.SL
{
	public static class HtmlLoader
	{
		// TODO сделать отмену загрузки страницы, при выходе с экрана расписания
		public static async Task<HtmlDocument> LoadHtmlDocumentAsync(string addr)
		{
			var web = new HtmlWeb();
			return await web.LoadFromWebAsync(addr);
		}

		public static HtmlDocument LoadHtmlDocument(string addr)
		{
			var web = new HtmlWeb();
			return web.Load(addr);
		}
	}
}
