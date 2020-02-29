using AndroidX.Fragment.App;
using PolyNavi.Fragments;
using System;

namespace PolyNavi.Adapters
{
    internal class ScheduleFragmentAdapter : FragmentStatePagerAdapter
    {
        private readonly ScheduleWeekFragment thisWeekTab;
        private readonly ScheduleWeekFragment nextWeekTab;

        private const int CurrentWeekTag = 0;
        private const int NextWeekTag = 1;

        public override int Count { get; }

        public ScheduleFragmentAdapter(FragmentManager fragmentManager, int numberOfTabs, DateTime date, int dayOfYear = -1) : base(fragmentManager)
        {
            this.Count = numberOfTabs;

            thisWeekTab = new ScheduleWeekFragment(date, CurrentWeekTag, dayOfYear);
            nextWeekTab = new ScheduleWeekFragment(GetNextWeekDate(date), NextWeekTag, dayOfYear);
        }

        private static DateTime GetNextWeekDate(DateTime date) => date.AddDays(7);

        public override Fragment GetItem(int position)
        {
            return position switch
            {
                0 => thisWeekTab,
                1 => nextWeekTab,
                _ => null
            };
        }
    }
}