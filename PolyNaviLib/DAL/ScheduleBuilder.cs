using System;
using System.Collections.Generic;
using System.Text;
using PolyNaviLib.BL;
using HtmlAgilityPack;

namespace PolyNaviLib.DAL
{
    public class ScheduleBuilder
    {
		//Парсинг и построение расписания
		public static Schedule BuildSchedule(HtmlDocument htmlDoc)
		{
			Schedule schedule = new Schedule();
			Week w = new Week();
			Day d;
			Lesson l;

			var node = htmlDoc.DocumentNode.SelectSingleNode("//body/div/div/div/ul");
			var days = node.ChildNodes; //Список дней

			//bool first = true; //Первая пара или нет (возможно нужно будет для отображения даты в Layout)

			//Проход по дням
			foreach (var day in days)
			{
				d = new Day();
				d.Date = day.FirstChild.InnerText; //Добавляем дату

				var lessons = day.LastChild.ChildNodes; //Список пар

				//Проход по парам
				foreach (var lesson in lessons)
				{
					l = new Lesson();

					l.Building = lesson.LastChild.LastChild.FirstChild.FirstChild.FirstChild.InnerText;      //Корпус
																											 //l.Groups = lesson; //Группы пока не уверен как правильно сделать
					l.Room = lesson.LastChild.LastChild.FirstChild.FirstChild.LastChild.LastChild.InnerText; //Аудитория
					l.Subject = lesson.FirstChild.LastChild.InnerText;                                       //Название пары
					l.Time = lesson.FirstChild.FirstChild.InnerText;                                         //Время пары

					d.Lessons.Add(l); //Добавление пары в день
				}
				w.Days.Add(d); //Добавление дня в неделю
			}
			schedule.Weeks.Add(w);

			return schedule;
		}

	}
}
