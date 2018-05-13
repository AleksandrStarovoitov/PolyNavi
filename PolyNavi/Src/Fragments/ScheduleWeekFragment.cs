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
		private DateTime weekDate;

		public ScheduleWeekFragment(DateTime weekDate)
		{
			this.weekDate = weekDate;
		}

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			view = inflater.Inflate(Resource.Layout.fragment_week_schedule, container, false);
			DrawContent(Resource.Id.relativelayout_week_schedule, Resource.Layout.layout_week_schedule);

			mSwipeRefreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipetorefresh_week_schedule);
			mSwipeRefreshLayout.SetOnRefreshListener(this);

			recyclerViewSchedule = view.FindViewById<RecyclerView>(Resource.Id.recyclerview_week_schedule);

			LoadSheduleAndUpdateUIWithPorgressBar(weekDate);

			return view;
		}

		public void OnRefresh()
		{
			FragmentManager.BeginTransaction().Detach(this).Attach(this).Commit();
			mSwipeRefreshLayout.Refreshing = false;
		}

		private void LoadSheduleAndUpdateUIWithPorgressBar(DateTime weekDate)
		{
			var progress = view.FindViewById<ProgressBar>(Resource.Id.progressbar_week_schedule);
			progress.Visibility = ViewStates.Visible;
			Task.Run(async () =>
			{
				var manager = await MainApp.Instance.PolyManager;
				try
				{
					var week = await manager.GetWeekAsync(weekDate);
					days = week.Days;
					Activity.RunOnUiThread(() =>
					{
						progress.Visibility = ViewStates.Invisible;
						adapter = new ScheduleCardFragmentAdapter(days);
						recyclerViewSchedule.HasFixedSize = true;
						recyclerViewSchedule.SetAdapter(adapter);
						recyclerViewSchedule.SetLayoutManager(new LinearLayoutManager(Activity.BaseContext));
					});
				}
				catch (NetworkException)
				{
					Activity.RunOnUiThread(() =>
					{
						DrawContent(Resource.Id.relativelayout_week_schedule, Resource.Layout.layout_no_connection);
						var v = Activity.FindViewById<RelativeLayout>(Resource.Id.layout_no_connection_clickable_zone);
						v.Click += (sender, e) =>
						{
							FragmentManager.BeginTransaction().Detach(this).Attach(this).Commit();
						};
					});
				}
			});
		}

		private void DrawContent(int rootContainerId, int newLayoutId)
		{
			ViewGroup vg = view.FindViewById<ViewGroup>(rootContainerId);
			vg.RemoveAllViews();
			vg.AddView(Inflate(newLayoutId),
			           0,
			           new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent,
			                                      ViewGroup.LayoutParams.MatchParent));
		}

		private View Inflate(int layoutId)
		{
			return ((LayoutInflater)Activity.GetSystemService(Context.LayoutInflaterService)).Inflate(layoutId, null);
		}
	}
}