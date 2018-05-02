using System;
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
//using static Android.Support.Design.Widget.TabLayout;


namespace PolyNavi
{
	[Activity(Label = "ScheduleSwipeActivity")]
	public class ScheduleFragment : Fragment
	{
		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View v = inflater.Inflate(Resource.Layout.fragment_schedule, container, false);

			var tabLayout = v.FindViewById<TabLayout>(Resource.Id.tablayout_schedule);
			tabLayout.AddTab(tabLayout.NewTab().SetText("Текущая неделя"));
			tabLayout.AddTab(tabLayout.NewTab().SetText("Следующая неделя"));
			tabLayout.SetForegroundGravity(TabLayout.GravityFill);

			var viewPager = v.FindViewById<ViewPager>(Resource.Id.viewpager_schedule);
			var adapter = new ScheduleFragmentAdapter(((AppCompatActivity)Activity).SupportFragmentManager, tabLayout.TabCount);
			viewPager.Adapter = adapter;
			viewPager.AddOnPageChangeListener(new TabLayout.TabLayoutOnPageChangeListener(tabLayout));

			tabLayout.TabSelected += (object sender, TabLayout.TabSelectedEventArgs e) =>
			{
				viewPager.CurrentItem = e.Tab.Position;
			};
			return v;
		}

	}
}