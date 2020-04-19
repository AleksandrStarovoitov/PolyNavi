using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.View;
using AndroidX.DrawerLayout.Widget;
using Google.Android.Material.Navigation;
using PolyNavi.Fragments;
using PolyNaviLib.Constants;
using static AndroidX.DrawerLayout.Widget.DrawerLayout;
using Fragment = AndroidX.Fragment.App.Fragment;

namespace PolyNavi.Activities
{
    [Activity(
        Label = "PolyNavi",
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class MainActivity : AppCompatActivity, IDrawerListener
    {
        private DrawerLayout drawerLayout;
        private ActionBarDrawerToggle drawerToggle;
        private Type fragmentClass;
        private NavigationView navigationView;
        private string startActivity;
        private bool isTapped;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            MainApp.ChangeLanguage(this);

            SetTheme(Resource.Style.MyAppTheme);
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_main);

            Setup();

            InstantiateFragment();
        }

        private void Setup()
        {
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawerlayout_main);
            drawerLayout.AddDrawerListener(this);
            drawerLayout.SetStatusBarBackground(Resource.Color.color_status_bar);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar_main);

            drawerToggle = new ActionBarDrawerToggle(this, drawerLayout, toolbar,
                Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawerLayout.AddDrawerListener(drawerToggle);

            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.baseline_menu_black);

            navigationView = FindViewById<NavigationView>(Resource.Id.navview_main);
            navigationView.NavigationItemSelected += NavViewItemSelected;
            navigationView.Alpha = 0.99f;

            startActivity =
                MainApp.Instance.SharedPreferences.GetString(PreferenceConstants.StartActivityPreferenceKey, null);
        }

        private void InstantiateFragment()
        {
            int startMenuItem;

            switch (startActivity)
            {
                case "mainbuilding":
                    fragmentClass = typeof(MainBuildingFragment);
                    startMenuItem = 0;
                    break;
                case "buildings":
                    fragmentClass = typeof(MapBuildingsFragment);
                    startMenuItem = 1;
                    break;
                case "schedule":
                    fragmentClass = typeof(ScheduleFragment);
                    startMenuItem = 2;
                    break;
                case "settings":
                    fragmentClass = typeof(MyPreferenceFragment);
                    startMenuItem = 3;
                    break;
                case "about":
                    fragmentClass = typeof(AboutFragment);
                    startMenuItem = 4;
                    break;
                default:
                    fragmentClass = typeof(MainBuildingFragment);
                    startMenuItem = 0;
                    break;
            }

            var fragment = (Fragment)Activator.CreateInstance(fragmentClass);
            navigationView.Menu.GetItem(startMenuItem).SetChecked(true);
            Title = startMenuItem == 4
                ? navigationView.Menu.FindItem(Resource.Id.nav_about_menu).TitleFormatted.ToString() //TODO ?
                : navigationView.Menu.GetItem(startMenuItem).TitleFormatted.ToString();
            SupportFragmentManager.BeginTransaction().Replace(Resource.Id.contentframe_main, fragment).Commit();
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            drawerToggle.SyncState();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    drawerLayout.OpenDrawer(GravityCompat.Start);
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        public void OnDrawerSlide(View drawerView, float slideOffset)
        {
            Utils.Utils.HideKeyboard(drawerView, this);
        }

        public void OnDrawerOpened(View drawerView)
        {
        }

        public void OnDrawerStateChanged(int newState)
        {
        }

        public void OnDrawerClosed(View drawerView)
        {
            if (fragmentClass == null || !isTapped)
            {
                return;
            }

            InstantiateFragment();
            isTapped = false;
        }

        private void NavViewItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            var menuItemId = e.MenuItem.ItemId;

            startActivity = menuItemId switch
            {
                Resource.Id.nav_gz_menu => "mainbuilding",
                Resource.Id.nav_buildings_menu => "buildings",
                Resource.Id.nav_rasp_menu => "schedule",
                Resource.Id.nav_settings_menu => "settings",
                Resource.Id.nav_about_menu => "about",
                _ => startActivity
            };

            Title = e.MenuItem.TitleFormatted.ToString();
            e.MenuItem.SetChecked(true);
            isTapped = true;
            drawerLayout.CloseDrawers();
        }
    }
}
