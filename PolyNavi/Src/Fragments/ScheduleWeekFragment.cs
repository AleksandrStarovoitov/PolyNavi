using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

using static Android.Support.V4.Widget.SwipeRefreshLayout;

using PolyNaviLib.BL;
using PolyNaviLib.DAL;

namespace PolyNavi
{
    public class ScheduleWeekFragment : Fragment, IOnRefreshListener
	{
		SwipeRefreshLayout mSwipeRefreshLayout;
		View view;
		List<Day> days;
		RecyclerView recyclerViewSchedule;
		ScheduleCardFragmentAdapter adapter;
	    readonly DateTime weekDate;
	    readonly int weekTag;
	    readonly int dayOfYear;

        public ScheduleWeekFragment()
        {

        }

		public ScheduleWeekFragment(DateTime weekDate, int weekTag, int dayOfYear)
		{
			this.weekDate = weekDate;
            this.weekTag = weekTag;
            this.dayOfYear = dayOfYear;
        }

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			view = inflater.Inflate(Resource.Layout.fragment_week_schedule, container, false);
			DrawContent(Resource.Id.relativelayout_week_schedule, Resource.Layout.layout_week_schedule);

			mSwipeRefreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipetorefresh_week_schedule);
			mSwipeRefreshLayout.SetOnRefreshListener(this);

			recyclerViewSchedule = view.FindViewById<RecyclerView>(Resource.Id.recyclerview_week_schedule);
            
			LoadSheduleAndUpdateUIWithPorgressBar(weekDate, false);

			return view;
		}

		public void OnRefresh()
		{
			if (days.Count == 0)
			{
				DrawContent(Resource.Id.relativelayout_week_schedule, Resource.Layout.layout_week_schedule);
			}
			LoadSheduleAndUpdateUIWithPorgressBar(weekDate, true);
		}

		private void LoadSheduleAndUpdateUIWithPorgressBar(DateTime weekDate, bool forceUpdate)
		{
			if (forceUpdate)
			{
				recyclerViewSchedule.SetAdapter(null);
			}
			var progress = view.FindViewById<ProgressBar>(Resource.Id.progressbar_week_schedule);
            mSwipeRefreshLayout.Refreshing = true;
            mSwipeRefreshLayout.Visibility = ViewStates.Invisible;
            progress.Visibility = ViewStates.Visible;            
            Task.Run(async () =>
			{
				var manager = await MainApp.Instance.PolyManager;
				try
				{
                    var weekRoot = await manager.GetWeekRootAsync(weekDate, forceUpdate: forceUpdate);

					days = weekRoot.Days;
					Activity.RunOnUiThread(() =>
					{
						progress.Visibility = ViewStates.Invisible;
                        mSwipeRefreshLayout.Visibility = ViewStates.Visible;
                        mSwipeRefreshLayout.Refreshing = false;
                        if (days.Count == 0)
                        {
                            DrawContent(Resource.Id.relativelayout_week_schedule, Resource.Layout.layout_empty_schedule_error);
                            
                            view.FindViewById<TextView>(Resource.Id.textview_empty_schedule_error_title).Text =
                                GetString(Resource.String.empty_schedule_error_title_start) + " " +
                                (weekTag == 0 ?
                                GetString(Resource.String.empty_schedule_error_title_current) :
                                GetString(Resource.String.empty_schedule_error_title_next)) + " " +
                                GetString(Resource.String.empty_schedule_error_title_end);
							view.FindViewById<TextView>(Resource.Id.textview_empty_schedule_error).Text = GetString(Resource.String.updated) + " " + weekRoot.LastUpdated.ToString(CultureInfo.CurrentCulture);
							view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipetorefresh_empty_schedule_error).SetOnRefreshListener(this);
						}
                        else
                        {
                            adapter = new ScheduleCardFragmentAdapter(days);
                            recyclerViewSchedule.HasFixedSize = true;
                            recyclerViewSchedule.SetAdapter(adapter);
                            recyclerViewSchedule.SetLayoutManager(new LinearLayoutManager(Activity.BaseContext));
                            var pos = days.FindIndex(day => day.Date.DayOfYear == (dayOfYear == -1 ? DateTime.Now.DayOfYear : dayOfYear));
                            if (pos != -1)
                            {
                                recyclerViewSchedule.ScrollToPosition(pos);
                            }
						}
					});
				}
				catch (NetworkException)
				{
					Activity.RunOnUiThread(() =>
					{
						DrawContent(Resource.Id.relativelayout_week_schedule, Resource.Layout.layout_no_connection);
						var v = view.FindViewById<RelativeLayout>(Resource.Id.layout_no_connection_clickable_zone);

                        v.Click += (sender, e) =>
						{
							FragmentManager.BeginTransaction().Detach(this).Attach(this).Commit();
						};
					});
				}
                catch (GroupNumberException)
                {
                    Activity.RunOnUiThread(() =>
                    {
                        DrawContent(Resource.Id.relativelayout_week_schedule, Resource.Layout.layout_empty_schedule_error);

                        view.FindViewById<TextView>(Resource.Id.textview_empty_schedule_error_title).Text =
                            GetString(Resource.String.wrong_group_error);
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