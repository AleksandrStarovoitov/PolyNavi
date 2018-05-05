using System;
using System.Collections.Generic;
using System.Text;
using PolyNaviLib.BL;
using HtmlAgilityPack;
using System.Globalization;

namespace PolyNaviLib.DAL
{
	public static class ScheduleBuilder
	{
		public static string[] months = { "янв.", "февр.", "мар.",
								   "апр.", "мая", "июн.",
								   "июл.", "авг.", "сент.",
								   "окт.", "нояб.", "дек.", ""};
	//Парсинг и построение расписания
	public static Schedule BuildSchedule(HtmlDocument htmlDoc)
		{
			Schedule schedule = new Schedule();
			Week w = new Week();
			Day d;
			Lesson l = new Lesson();
			DateTime dateTime, err = new DateTime(2007, 1, 1);

			CultureInfo ci = CultureInfo.CreateSpecificCulture("ru-RU");
			DateTimeFormatInfo dtfi = ci.DateTimeFormat;
			dtfi.AbbreviatedMonthNames = months;
			dtfi.AbbreviatedMonthGenitiveNames = dtfi.AbbreviatedMonthNames;

			var node = htmlDoc.DocumentNode.SelectSingleNode("//body/div/div/div/ul");

			//if (node == null) //Что-то пошло не так
			//{					//Скорее всего не указан номер группы
			//	throw			//Надо заранее обработать
			//}

			var days = node.ChildNodes; //Список дней

			foreach (var day in days)
			{
				d = new Day
				{
					Datestr = day.FirstChild.InnerText, //Добавляем дату
				};
				if (DateTime.TryParseExact(day.FirstChild.InnerText, "d MMM, ddd", ci, DateTimeStyles.None, out dateTime))
				{
					d.Date = dateTime;
				}
				else
				{
					d.Date = err;
				}

				var lessons = day.LastChild.ChildNodes; //Список пар
				//Проход по парам
				foreach (var lesson in lessons)
				{
					l = new Lesson
					{
						Building = lesson.LastChild.LastChild.FirstChild.FirstChild.FirstChild.InnerText,      //Корпус
																											   //l.Groups = lesson; //Группы пока не уверен как правильно сделать
						Room = lesson.LastChild.LastChild.FirstChild.FirstChild.LastChild.LastChild.InnerText, //Аудитория
						Subject = lesson.FirstChild.LastChild.InnerText.Replace("&quot;", "\""),               //Название пары
						Timestr = lesson.FirstChild.FirstChild.InnerText,                                      //Время пары
						Type = lesson.LastChild.FirstChild.InnerText										   //леции/практика
					};
					if (DateTime.TryParse(lesson.FirstChild.FirstChild.InnerText.Substring(0, 5), ci, DateTimeStyles.None, out dateTime)) //+ " " + d.Datestr, "HH:mm d MMM., ddd", new CultureInfo("Ru-ru"));
					{
						l.StartTime = dateTime;
					}
					else
					{
						l.StartTime = err;
					}

					if (DateTime.TryParse(lesson.FirstChild.FirstChild.InnerText.Substring(6), ci, DateTimeStyles.None, out dateTime))//+ " " + d.Datestr, "HH:mm d MMM., ddd", new CultureInfo("Ru-ru"));
					{
						l.EndTime = dateTime;
					}
					else
					{
						l.EndTime = err;
					}
					d.Lessons.Add(l); //Добавление пары в день
				}
				w.Days.Add(d); //Добавление дня в неделю
			}
			schedule.Weeks.Add(w);

			return schedule;
		}

		public static string GetScheduleLink(HtmlDocument htmlDocSearch)
		{
			var searchRes = htmlDocSearch.DocumentNode.SelectSingleNode("//body/div/div/div/ul");
			var h = @"http://ruz.spbstu.ru";
			if (searchRes != null && searchRes.ChildNodes.Count == 1)
			{
				h += searchRes.FirstChild.FirstChild.Attributes["href"].Value;
			}
			//else searchRes.ChildNodes.Count != 1 //уточнить номер группы (слишком много групп в результате поиска)
			//else searchRes == null //нет результатов (либо неверно указан номер группы, либо сайт не работает)
			return h;
		}

	}
}