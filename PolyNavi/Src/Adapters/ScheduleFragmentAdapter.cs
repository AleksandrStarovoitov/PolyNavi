using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;

namespace PolyNavi
{
	class ScheduleFragmentAdapter : FragmentStatePagerAdapter
	{
		private int numOfTabs;
		private ScheduleWeekFragment tab1;
		private ScheduleWeekFragment tab2;

        const int CurrentWeekTag = 0;
        const int NextWeekTag = 1;
        DateTime date;

        public ScheduleFragmentAdapter(FragmentManager fragmentManager, int numOfTabs, DateTime date, int dayOfYear = -1) : base(fragmentManager)
		{
			this.numOfTabs = numOfTabs;
			
			tab1 = new ScheduleWeekFragment(date, CurrentWeekTag, dayOfYear);
			tab2 = new ScheduleWeekFragment(date + TimeSpan.FromDays(7), NextWeekTag, dayOfYear);
		}

		public override Fragment GetItem(int position)
		{
			switch (position)
			{
				case 0:
					return tab1;
				case 1:
					return tab2;
				default:
					return null;
			}
		}

		public override int Count
		{
			get
			{
				return numOfTabs;
			}
		}

	}

}