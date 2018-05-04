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
		private List<Day> days;
		private RecyclerView recyclerViewSchedule;
		private ScheduleCardFragmentAdapter adapter;
		private string groupNumber;
		private Weeks week;

		public ScheduleWeekFragment(Weeks week, string groupNumber)
		{
			this.groupNumber = groupNumber;
			this.week = week;
		}

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			view = inflater.Inflate(Resource.Layout.fragment_week_schedule, container, false);

			mSwipeRefreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipetorefresh_week_schedule);
			mSwipeRefreshLayout.SetOnRefreshListener(this);

			recyclerViewSchedule = view.FindViewById<RecyclerView>(Resource.Id.recyclerview_week_schedule);

			LoadSheduleAndUpdateUIWithPorgressBar(week);

			return view;
		}

		public void OnRefresh()
		{
			recyclerViewSchedule.SetAdapter(null);
			mSwipeRefreshLayout.Refreshing = false;
			LoadSheduleAndUpdateUIWithPorgressBar(week);
		}

		private void LoadSheduleAndUpdateUIWithPorgressBar(Weeks week)
		{
			var progress = view.FindViewById<ProgressBar>(Resource.Id.progressbar_week_schedule);
			progress.Visibility = ViewStates.Visible;
			Task.Run(async () =>
			{
				days = await MainActivity.PolyManager.GetScheduleByWeekAsync(week, groupNumber); //Next
				Activity.RunOnUiThread(() =>
				{
					progress.Visibility = ViewStates.Invisible;
					adapter = new ScheduleCardFragmentAdapter(days);
					recyclerViewSchedule.HasFixedSize = true;
					recyclerViewSchedule.SetAdapter(adapter);
					recyclerViewSchedule.SetLayoutManager(new LinearLayoutManager(Activity.BaseContext));
				});
			});
		}
	}
}