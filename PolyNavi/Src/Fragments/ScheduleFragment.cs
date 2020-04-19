using System;
using Android.App;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.ViewPager.Widget;
using Google.Android.Material.Tabs;
using PolyNavi.Adapters;
using Fragment = AndroidX.Fragment.App.Fragment;

namespace PolyNavi.Fragments
{
    [Activity(Label = "ScheduleSwipeActivity")]
    public class ScheduleFragment : Fragment
    {
        private TabLayout tabLayout;
        private ViewPager viewPager;
        private ScheduleFragmentAdapter adapter;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_schedule, container, false);

            HasOptionsMenu = true;

            tabLayout = view.FindViewById<TabLayout>(Resource.Id.tablayout_schedule);
            tabLayout.AddTab(tabLayout.NewTab().SetText(GetString(Resource.String.currentweek_tab)));
            tabLayout.AddTab(tabLayout.NewTab().SetText(GetString(Resource.String.nextweek_tab)));
            tabLayout.SetForegroundGravity(TabLayout.GravityFill);

            adapter = new ScheduleFragmentAdapter(((AppCompatActivity)Activity).SupportFragmentManager,
                tabLayout.TabCount, DateTime.Today);

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
                    DateTime? lastDate = null; //TODO
                    var frag = DateTimePickerFragment.NewInstance(time =>
                    {
                        viewPager.Adapter = null;
                        adapter = new ScheduleFragmentAdapter(((AppCompatActivity)Activity).SupportFragmentManager,
                            tabLayout.TabCount, time, time.DayOfYear);
                        viewPager.Adapter = adapter;
                        lastDate = time; //TODO
                    }, lastDate);

                    frag.Show(Activity.SupportFragmentManager, DateTimePickerFragment.DateTimePickerTag); //TODO
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}
