using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using HtmlAgilityPack;
using Android.Support.V4.Widget;
using static Android.Support.V4.Widget.SwipeRefreshLayout;
using PolyNaviLib.BL;
using PolyNaviLib.DAL;

namespace PolyNavi
{
	public class ScheduleWeekFragment : Fragment, IOnRefreshListener
	{

		private SwipeRefreshLayout mSwipeRefreshLayout;
		private View view;
		//private List<Lesson> lessons;
		private List<Day> days;
		private RecyclerView recyclerViewSchedule;
		private ScheduleCardFragmentAdapter adapter;
		private PolyManager polyManager;

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			polyManager = new PolyManager();
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			view = inflater.Inflate(Resource.Layout.fragment_week_schedule, container, false);

			mSwipeRefreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipetorefresh_week_schedule);
			mSwipeRefreshLayout.SetOnRefreshListener(this);

			//var p = view.FindViewById<ProgressBar>(Resource.Id.progressbar_week_schedule);
			//lessons = null;
			//p.Visibility = ViewStates.Visible;

			days = polyManager.GetScheduleByWeek(PolyManager.Weeks.Current);
			recyclerViewSchedule = (RecyclerView)view.FindViewById(Resource.Id.recyclerview_week_schedule);
			recyclerViewSchedule.HasFixedSize = true;
			adapter = new ScheduleCardFragmentAdapter(days);
			recyclerViewSchedule.SetAdapter(adapter);
			recyclerViewSchedule.SetLayoutManager(new LinearLayoutManager(Activity.BaseContext));

			return view;
			//Task.Run(async () =>
			//{
			//	lessons = await LoadAsync("13631/2"); //вызов полиманагера
			//	Activity.RunOnUiThread(() =>
			//	{
			//		p.Visibility = ViewStates.Invisible;
			//		// Lookup the recyclerview in activity layout
			//		recyclerViewSchedule = (RecyclerView)view.FindViewById(Resource.Id.recyclerview_week_schedule);
			//		recyclerViewSchedule.HasFixedSize = true;
			//		adapter = new ScheduleWeekAdapter(lessons);
			//		// Attach the adapter to the recyclerview to populate items
			//		recyclerViewSchedule.SetAdapter(adapter);
			//		// Set layout manager to position the items
			//		recyclerViewSchedule.SetLayoutManager(new LinearLayoutManager(Activity.BaseContext));
			//	});
			//});
			//return view;
		}

		public void OnRefresh()
		{
			days = null;
			recyclerViewSchedule.SetAdapter(null);
			days = polyManager.GetScheduleByWeek(PolyManager.Weeks.Current); //Next
			adapter = new ScheduleCardFragmentAdapter(days);
			recyclerViewSchedule.SetAdapter(adapter);
			recyclerViewSchedule.SetLayoutManager(new LinearLayoutManager(Activity.BaseContext));
			mSwipeRefreshLayout.Refreshing = false;

			//lessons = null;
			//var p = view.FindViewById<ProgressBar>(Resource.Id.progressbar_week_schedule);
			//p.Visibility = ViewStates.Visible;
			//Task.Run(async () =>
			//{
			//	recyclerViewSchedule.SetAdapter(null);
			//	lessons = await LoadAsync("23537/1"); //вызов полиманагера
			//	Activity.RunOnUiThread(() =>
			//	{
			//		p.Visibility = ViewStates.Invisible;
			//		// Lookup the recyclerview in activity layout
			//		//RecyclerView recyclerViewSchedule = (RecyclerView)view.FindViewById(Resource.Id.timetableView);
			//		//recyclerViewSchedule.HasFixedSize = true;

			//		adapter = new ScheduleWeekAdapter(lessons);
			//		// Attach the adapter to the recyclerview to populate items
			//		recyclerViewSchedule.SetAdapter(adapter);
			//		// Set layout manager to position the items
			//		recyclerViewSchedule.SetLayoutManager(new LinearLayoutManager(Activity.BaseContext));
			//	});
			//});
			//mSwipeRefreshLayout.Refreshing = false;
		}

		//	private async Task<List<Lesson>> LoadAsync(string str)
		//	{
		//		string temp22 = str;

		//		string search = @"http://ruz.spbstu.ru/search/groups?q=";
		//		search += temp22;
		//		var htmlSearch = search;
		//		HtmlWeb webSearch = new HtmlWeb();

		//		//var htmlDocSearch = webSearch.Load(htmlSearch);
		//		var htmlDocSearch = await webSearch.LoadFromWebAsync(htmlSearch);
		//		var searchRes = htmlDocSearch.DocumentNode.SelectSingleNode("//body/div/div/div/ul/li");
		//		var h = @"http://ruz.spbstu.ru";
		//		h += searchRes.FirstChild.Attributes["href"].Value;

		//		List<Lesson> tmtbl = new List<Lesson>();
		//		//var html = @"http://ruz.spbstu.ru/faculty/99/groups/24729";

		//		HtmlWeb web = new HtmlWeb();

		//		//var htmlDoc = web.Load(h);
		//		var htmlDoc = await web.LoadFromWebAsync(h);
		//		var sss2 = htmlDoc.DocumentNode.SelectSingleNode("//body/div/div/div/ul");

		//		var d = sss2.ChildNodes;
		//		string temp;
		//		bool first = true, last = false;
		//		List<Lesson> lessons = new List<Lesson>();
		//		foreach (var dd in d)
		//		{
		//			var ddd = dd.LastChild.ChildNodes;
		//			if (last)
		//			{
		//				tmtbl._head = true;
		//				last = false;
		//			}
		//			first = true;
		//			foreach (var g in ddd)
		//			{
		//				tmtbl = new Lesson();
		//				if (first)
		//				{
		//					tmtbl._date = dd.FirstChild.InnerText; //Дата	
		//				}
		//				tmtbl._time = g.FirstChild.FirstChild.InnerText; //Время
		//				tmtbl._subject = g.FirstChild.LastChild.InnerText; //Название предметов
		//																   //tmtbl._ g.LastChild.FirstChild.InnerText); //Практика/лекции
		//																   //Console.WriteLine("Группы: " + g.LastChild.ChildNodes[1].LastChild.LastChild.InnerText); //Группы
		//																   //temp = g.LastChild.ChildNodes[2].FirstChild.FirstChild.LastChild.InnerText;
		//																   //if (!temp.Contains("ауд."))
		//																   //	Console.WriteLine("Преподаватель: " + g.LastChild.ChildNodes[2].FirstChild.FirstChild.LastChild.InnerText); //Преподаватель
		//				tmtbl._building = g.LastChild.LastChild.FirstChild.FirstChild.FirstChild.InnerText; //Корпус
		//				tmtbl._room = g.LastChild.LastChild.FirstChild.FirstChild.LastChild.LastChild.InnerText; //Аудитория
		//				lessons.Add(tmtbl);
		//				first = false;
		//			}
		//			last = true;
		//		}
		//		return lessons;
		//	}
	}
}