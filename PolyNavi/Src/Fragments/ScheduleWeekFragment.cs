using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using PolyNavi.Adapters;
using PolyNavi.Extensions;
using PolyNaviLib.BL;
using PolyNaviLib.DAL;
using PolyNaviLib.Exceptions;
using static AndroidX.SwipeRefreshLayout.Widget.SwipeRefreshLayout;

namespace PolyNavi.Fragments
{
    public class ScheduleWeekFragment : Fragment, IOnRefreshListener
    {
        private SwipeRefreshLayout swipeRefreshLayout;
        private RecyclerView recyclerViewSchedule;
        private ProgressBar progressBar;
        private View view;
        private List<Day> days;
        private readonly DateTime weekDate;
        private readonly int weekTag;
        private readonly int dayOfYear;

        public ScheduleWeekFragment(DateTime weekDate, int weekTag, int dayOfYear)
        {
            this.weekDate = weekDate;
            this.weekTag = weekTag;
            this.dayOfYear = dayOfYear;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_week_schedule, container, false);
            ShowScheduleLayout();

            progressBar = view.FindViewById<ProgressBar>(Resource.Id.progressbar_week_schedule);
            swipeRefreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipetorefresh_week_schedule);
            swipeRefreshLayout.SetOnRefreshListener(this);

            recyclerViewSchedule = view.FindViewById<RecyclerView>(Resource.Id.recyclerview_week_schedule);

            LoadScheduleAndToggleProgressBar();

            return view;
        }

        public void OnRefresh()
        {
            if (!days.Any())
            {
                ShowScheduleLayout();
            }

            recyclerViewSchedule.SetAdapter(null);

            progressBar.Show();
            HideSwipeRefreshLayout();

            Task.Run(ForceLoadLatestSchedule);
        }

        private void ShowScheduleLayout()
        {
            ChangeViewContentInContainer(Resource.Id.relativelayout_week_schedule,
                Resource.Layout.layout_week_schedule);
        }

        private void LoadScheduleAndToggleProgressBar()
        {
            ToggleProgressBarVisibility();

            Task.Run(LoadCachedOrLatestSchedule);
        }

        private async Task ForceLoadLatestSchedule()
        {
            var manager = await MainApp.Instance.PolyManager;
            var weekRoot = await manager.GetLatestSchedule(weekDate);

            LoadSchedule(weekRoot);
        }

        private async Task LoadCachedOrLatestSchedule()
        {
            var manager = await MainApp.Instance.PolyManager;
            var weekRoot = await manager.GetSchedule(weekDate);

            LoadSchedule(weekRoot);
        }

        private void LoadSchedule(WeekRoot weekRoot)
        {
            try
            {
                days = weekRoot.Days;

                Activity.RunOnUiThread(() =>
                {
                    ToggleProgressBarVisibility();

                    if (days.Any())
                    {
                        ShowSchedule();
                    }
                    else
                    {
                        ShowEmptyScheduleError(weekRoot);
                    }
                });
            }
            catch (NetworkException)
            {
                Activity.RunOnUiThread(ShowNetworkError);
            }
            catch (GroupNumberException)
            {
                Activity.RunOnUiThread(ShowGroupNumberError);
            }
        }


        private void ShowSchedule()
        {
            var adapter = new ScheduleCardFragmentAdapter(days);

            recyclerViewSchedule.HasFixedSize = true;
            recyclerViewSchedule.SetAdapter(adapter);
            recyclerViewSchedule.SetLayoutManager(new LinearLayoutManager(Activity.BaseContext));

            var currentDayPosition = days.FindIndex(day =>
                day.Date.DayOfYear == (dayOfYear == -1 ? DateTime.Now.DayOfYear : dayOfYear));

            if (currentDayPosition != -1) //TODO -1
            {
                recyclerViewSchedule.ScrollToPosition(currentDayPosition);
            }
        }

        private void ShowEmptyScheduleError(WeekRoot weekRoot) //TODO Remove weekRoot?
        {
            ChangeViewContentInContainer(Resource.Id.relativelayout_week_schedule,
                Resource.Layout.layout_empty_schedule_error);

            var scheduleErrorTitleTextView =
                view.FindViewById<TextView>(Resource.Id.textview_empty_schedule_error_title);

            scheduleErrorTitleTextView.Text =
                GetString(Resource.String.empty_schedule_error_title_start) + " " +
                (weekTag == 0
                    ? GetString(Resource.String.empty_schedule_error_title_current)
                    : GetString(Resource.String.empty_schedule_error_title_next)) + " " +
                GetString(Resource.String.empty_schedule_error_title_end);

            var emptyScheduleErrorTextView = view.FindViewById<TextView>(Resource.Id.textview_empty_schedule_error);
            var wasUpdatedOn = GetString(Resource.String.updated) + " " +
                               weekRoot.LastUpdated.ToString(CultureInfo.CurrentCulture);
            emptyScheduleErrorTextView.Text = wasUpdatedOn;

            var swipeToRefreshLayout =
                view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipetorefresh_empty_schedule_error);
            swipeToRefreshLayout.SetOnRefreshListener(this);
        }

        private void ShowNetworkError()
        {
            ChangeViewContentInContainer(Resource.Id.relativelayout_week_schedule,
                Resource.Layout.layout_no_connection);

            var noConnectionLayout = view.FindViewById<RelativeLayout>(Resource.Id.layout_no_connection_clickable_zone);
            noConnectionLayout.Click += (sender, args) => ReattachFragment();
        }

        private void ReattachFragment()
        {
            FragmentManager.BeginTransaction().Detach(this).Attach(this).Commit();
        }

        private void ShowGroupNumberError()
        {
            ChangeViewContentInContainer(Resource.Id.relativelayout_week_schedule,
                Resource.Layout.layout_empty_schedule_error);

            var errorTextView = view.FindViewById<TextView>(Resource.Id.textview_empty_schedule_error_title);
            errorTextView.Text = GetString(Resource.String.wrong_group_error);
        }

        private void ToggleProgressBarVisibility()
        {
            if (swipeRefreshLayout.Refreshing)
            {
                progressBar.Hide();
                ShowSwipeRefreshLayout();
            }
            else
            {
                HideSwipeRefreshLayout();
                progressBar.Show();
            }
        }
        
        private void ShowSwipeRefreshLayout()
        {
            swipeRefreshLayout.Visibility = ViewStates.Visible;
            swipeRefreshLayout.Refreshing = false;
        }

        private void HideSwipeRefreshLayout()
        {
            swipeRefreshLayout.Refreshing = true;
            swipeRefreshLayout.Visibility = ViewStates.Invisible;
        }

        private void ChangeViewContentInContainer(int rootContainerId, int newLayoutId)
        {
            var viewGroup = view.FindViewById<ViewGroup>(rootContainerId);

            ClearViewGroup(viewGroup);
            
            AddViewToViewGroup(viewGroup, newLayoutId);
        }

        private static void ClearViewGroup(ViewGroup viewGroup)
        {
            viewGroup.RemoveAllViews();
        }

        private void AddViewToViewGroup(ViewGroup viewGroup, int newLayoutId)
        {
            viewGroup.AddView(Inflate(newLayoutId),
                0,
                new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.MatchParent));
        }

        private View Inflate(int layoutId)
        {
            return ((LayoutInflater) Activity.GetSystemService(Context.LayoutInflaterService)).Inflate(layoutId, null);
        }
    }
}