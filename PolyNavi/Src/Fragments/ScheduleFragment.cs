using System;
using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Views;
using PolyNavi.Adapters;

namespace PolyNavi.Fragments
{
    [Activity(Label = "ScheduleSwipeActivity")]
	public class ScheduleFragment : Android.Support.V4.App.Fragment
	{
        private View view;
        private TabLayout tabLayout;
        private ViewPager viewPager;
        private ScheduleFragmentAdapter adapter;
        private DateTime? lastDate;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			view = inflater.Inflate(Resource.Layout.fragment_schedule, container, false);

            HasOptionsMenu = true;

            tabLayout = view.FindViewById<TabLayout>(Resource.Id.tablayout_schedule);
			tabLayout.AddTab(tabLayout.NewTab().SetText(GetString(Resource.String.currentweek_tab)));
			tabLayout.AddTab(tabLayout.NewTab().SetText(GetString(Resource.String.nextweek_tab)));
			tabLayout.SetForegroundGravity(TabLayout.GravityFill);

			adapter = new ScheduleFragmentAdapter(((AppCompatActivity)Activity).SupportFragmentManager, tabLayout.TabCount, DateTime.Today);

			viewPager = view.FindViewById<ViewPager>(Resource.Id.viewpager_schedule);
			viewPager.Adapter = adapter;
			viewPager.AddOnPageChangeListener(new TabLayout.TabLayoutOnPageChangeListener(tabLayout));

			tabLayout.TabSelected += (sender, e) =>
			{
				viewPager.CurrentItem = e.Tab.Position;
			};
            
			return view;
		}

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.menu_schedule, menu);
            base.OnCreateOptionsMenu(menu, inflater);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_schedule_datetimepicker:
                    var frag = DateTimePickerFragment.NewInstance(delegate (DateTime time)
                    {
                        viewPager.Adapter = null;
                        adapter = new ScheduleFragmentAdapter(((AppCompatActivity)Activity).SupportFragmentManager, tabLayout.TabCount, time, time.DayOfYear);
                        viewPager.Adapter = adapter;
                        lastDate = time;
                    }, lastDate);
                    frag.Show(Activity.FragmentManager, DateTimePickerFragment.DateTimePickerTag);
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}