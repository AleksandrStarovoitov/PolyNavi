using System;
using NUnit.Framework;
using System.IO;
using PolyNaviLib.DL;
using PolyNaviLib.BL;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SQLiteDatabaseTest
{
	[TestFixture]
	public class TestsSample
	{
		internal static string GetFileFullPath(string fname)
		{
			string dirPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
			return Path.Combine(dirPath, fname);
		}

		SQLiteDatabase db;

		[SetUp]
		public async void Setup()
		{
			db = new SQLiteDatabase(GetFileFullPath("test.sqlite"));

			//db.DropTableAsync<Week>().Wait();
			//db.DropTableAsync<Day>().Wait();
			//db.DropTableAsync<Lesson>().Wait();

			var t1 = db.CreateTableAsync<Week>();
			var t2 = db.CreateTableAsync<Day>();
			var t3 = db.CreateTableAsync<Lesson>();

			await Task.WhenAll(t1, t2, t3);
		}


		[TearDown]
		public void Tear()
		{
		}

		[Test]
		[Ignore]
		public void DeleteTest()
		{
			
		}


		[Test]
		public async void InsertTest()
		{
			var w = GetWeek();
			await db.SaveItemAsync(w);

			var weeks = await db.GetItemsAsync<Week>();

			Assert.True(weeks.Count == 1);

			var week = weeks[0];
			Assert.True(week.Days.Count == 3);
			foreach (var day in week.Days)
			{
				Assert.True(day.Lessons.Count == 4);
				for (int i = 0; i < day.Lessons.Count; ++i)
				{
					string str = $"day{i}";
					Assert.True(day.Lessons[i].Room == str);
					Assert.True(day.Lessons[i].Subject == str);
					Assert.True(day.Lessons[i].Groups == str);
				}
			}
		}

		Week GetWeek()
		{
			Week week = new Week()
			{
				Days = new List<Day>()
				{
					new Day()
					{
						Lessons = new List<Lesson>()
						{
							new Lesson()
							{
								Room = "Day1",
								Groups = "Day1",
								Subject = "Day1",
								StartTime = DateTime.Now,
								EndTime = DateTime.Now,
							},
							new Lesson()
							{
								Room = "Day1",
								Groups = "Day1",
								Subject = "Day1",
								StartTime = DateTime.Now,
								EndTime = DateTime.Now,
							},
							new Lesson()
							{
								Room = "Day1",
								Groups = "Day1",
								Subject = "Day1",
								StartTime = DateTime.Now,
								EndTime = DateTime.Now,
							},
							new Lesson()
							{
								Room = "Day1",
								Groups = "Day1",
								Subject = "Day1",
								StartTime = DateTime.Now,
								EndTime = DateTime.Now,
							},
						},
						Date = DateTime.Now,
					},
					new Day()
					{
						Lessons = new List<Lesson>()
						{
							new Lesson()
							{
								Room = "Day2",
								Groups = "Day2",
								Subject = "Day2",
								StartTime = DateTime.Now,
								EndTime = DateTime.Now,
							},
							new Lesson()
							{
								Room = "Day2",
								Groups = "Day2",
								Subject = "Day2",
								StartTime = DateTime.Now,
								EndTime = DateTime.Now,
							},
							new Lesson()
							{
								Room = "Day2",
								Groups = "Day2",
								Subject = "Day2",
								StartTime = DateTime.Now,
								EndTime = DateTime.Now,
							},
							new Lesson()
							{
								Room = "Day2",
								Groups = "Day2",
								Subject = "Day2",
								StartTime = DateTime.Now,
								EndTime = DateTime.Now,
							},
						},
						Date = DateTime.Now,
					},
					new Day()
					{
						Lessons = new List<Lesson>()
						{
							new Lesson()
							{
								Room = "Day3",
								Groups = "Day3",
								Subject = "Day3",
								StartTime = DateTime.Now,
								EndTime = DateTime.Now,
							},
							new Lesson()
							{
								Room = "Day3",
								Groups = "Day3",
								Subject = "Day3",
								StartTime = DateTime.Now,
								EndTime = DateTime.Now,
							},
							new Lesson()
							{
								Room = "Day3",
								Groups = "Day3",
								Subject = "Day3",
								StartTime = DateTime.Now,
								EndTime = DateTime.Now,
							},
							new Lesson()
							{
								Room = "Day3",
								Groups = "Day3",
								Subject = "Day3",
								StartTime = DateTime.Now,
								EndTime = DateTime.Now,
							},
						},
						Date = DateTime.Now,
					},
				},
			};
			return week;
		}
	}
}