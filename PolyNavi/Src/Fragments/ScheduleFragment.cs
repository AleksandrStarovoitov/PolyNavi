﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;

using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Support.V4.View;
using Android.Support.Design.Widget;

namespace PolyNavi
{
	[Activity(Label = "ScheduleSwipeActivity")]
	public class ScheduleFragment : Android.Support.V4.App.Fragment
	{
		private View view;
		private TabLayout tabLayout;
		private ViewPager viewPager;
		private ScheduleFragmentAdapter adapter;
        DateTime? lastDate = null;

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
		}

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

			tabLayout.TabSelected += (object sender, TabLayout.TabSelectedEventArgs e) =>
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
                    DateTimePickerFragment frag = DateTimePickerFragment.NewInstance(delegate (DateTime time)
                    {
                        viewPager.Adapter = null;
                        adapter = new ScheduleFragmentAdapter(((AppCompatActivity)Activity).SupportFragmentManager, tabLayout.TabCount, time, time.DayOfYear);
                        viewPager.Adapter = adapter;
                        lastDate = time;
                    }, lastDate);
                    frag.Show(Activity.FragmentManager, DateTimePickerFragment.TAG);
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}