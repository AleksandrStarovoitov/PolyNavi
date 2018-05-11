using System;
using System.Collections.Generic;
using System.Text;
using PolyNaviLib.BL;
using HtmlAgilityPack;
using System.Globalization;

namespace PolyNaviLib.SL
{
	public static class WeekBuilder
	{
		public static string[] months = { "янв.", "февр.", "мар.",
		                                  "апр.", "мая", "июн.",
		                                  "июл.", "авг.", "сент.",
		                                  "окт.", "нояб.", "дек.", ""};
		//Парсинг и построение расписания
		public static Week BuildWeek(HtmlDocument htmlDoc)
		{
			Week w = new Week();
			Day d;
			Lesson l = new Lesson();

			CultureInfo ci = CultureInfo.CreateSpecificCulture("ru-RU");
			DateTimeFormatInfo dtfi = ci.DateTimeFormat;
			dtfi.AbbreviatedMonthNames = months;
			dtfi.AbbreviatedMonthGenitiveNames = dtfi.AbbreviatedMonthNames;

			var node = htmlDoc.DocumentNode.SelectSingleNode("//body/div/div/div/ul");
			if (node == null)
			{
				throw new ScheduleParseException("Error when parse //body/div/div/div/ul of html document");
			}

			var days = node.ChildNodes; //Список дней

			foreach (var day in days)
			{
				d = new Day
				{
					Datestr = day.FirstChild.InnerText, //Добавляем дату
					Date = DateTime.ParseExact(day.FirstChild.InnerText, "d MMM, ddd", ci),
				};

				var lessons = day.LastChild.ChildNodes; //Список пар
														//Проход по парам
				foreach (var lesson in lessons)
				{
					l = new Lesson
					{
						Building = lesson.LastChild.LastChild.FirstChild.FirstChild.FirstChild.InnerText,
						Room = lesson.LastChild.LastChild.FirstChild.FirstChild.LastChild.LastChild.InnerText,
						Subject = lesson.FirstChild.LastChild.InnerText.Replace("&quot;", "\""),
						Timestr = lesson.FirstChild.FirstChild.InnerText,
						Type = lesson.LastChild.FirstChild.InnerText,
						StartTime = DateTime.Parse(lesson.FirstChild.FirstChild.InnerText.Substring(0, 5), ci),
						EndTime = DateTime.Parse(lesson.FirstChild.FirstChild.InnerText.Substring(6), ci),
					};
					d.Lessons.Add(l); //Добавление пары в день
				}
				w.Days.Add(d); //Добавление дня в неделю
			}
			return w;
		}

		public static string GetScheduleLink(HtmlDocument htmlDocSearch)
		{
			var searchRes = htmlDocSearch.DocumentNode.SelectSingleNode("//body/div/div/div/ul");
			var h = @"http://ruz.spbstu.ru";
			if (searchRes == null)
			{
				throw new ScheduleParseException("Error when parse //body/div/div/div/ul of html document");
			}
			if (searchRes.ChildNodes.Count == 1)
			{
				h += searchRes.FirstChild.FirstChild.Attributes["href"].Value;
			}
			//else searchRes.ChildNodes.Count > 1 //уточнить номер группы (слишком много групп в результате поиска)
			return h;
		}

	}
}