using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Nito.AsyncEx;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Preferences;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

using PolyNaviLib.BL;

using static Android.Support.V4.Widget.DrawerLayout;
using Java.Util;
using Android.Content.Res;
using Android.Content.PM;

namespace PolyNavi
{
	[Activity(
		Label = "PolyNavi",
		ScreenOrientation = ScreenOrientation.Portrait,
		ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
	public class MainActivity : AppCompatActivity, IDrawerListener
	{
		private DrawerLayout drawerLayout;
		private ActionBarDrawerToggle drawerToggle;
		private Type fragmentClass = null;
		private Android.Support.V4.App.Fragment fragment;
		private Android.Support.V7.Widget.Toolbar toolbar;
		private NavigationView navigationView;
		private string startActivity;
		private int startMenuItem;
		private bool tapped = false;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			MainApp.ChangeLanguage(this);
			SetTheme(Resource.Style.MyAppTheme);
			base.OnCreate(savedInstanceState);

			startActivity = MainApp.Instance.SharedPreferences.GetString("startactivity", null);
			string str = MainApp.Instance.SharedPreferences.GetString("language", null);
			SetContentView(Resource.Layout.activity_main);

			drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawerlayout_main);
			drawerLayout.AddDrawerListener(this);
			drawerLayout.SetStatusBarBackground(Resource.Color.colorPrimaryDark);

			toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar_main);

			drawerToggle = new ActionBarDrawerToggle(this, drawerLayout, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
			drawerLayout.AddDrawerListener(drawerToggle);

			SetSupportActionBar(toolbar);
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);
			SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);
			//SupportActionBar.SetDisplayShowHomeEnabled(true);

			navigationView = FindViewById<NavigationView>(Resource.Id.navview_main);
			navigationView.NavigationItemSelected += NavViewItemSelected;
			navigationView.Alpha = 0.99f;
			InstantiateFragment();
		}

		private void InstantiateFragment()
		{
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
					//Title = navigationView.Menu.FindItem(Resource.Id.nav_about_menu).TitleFormatted.ToString();
					break;
				default:
					fragmentClass = typeof(MainBuildingFragment);
					startMenuItem = 0;
					break;
			}
			fragment = (Android.Support.V4.App.Fragment)Activator.CreateInstance(fragmentClass);
			navigationView.Menu.GetItem(startMenuItem).SetChecked(true);
			Title = startMenuItem == 4? navigationView.Menu.FindItem(Resource.Id.nav_about_menu).TitleFormatted.ToString() : navigationView.Menu.GetItem(startMenuItem).TitleFormatted.ToString();
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
				case (Android.Resource.Id.Home):
					drawerLayout.OpenDrawer(Android.Support.V4.View.GravityCompat.Start);
					return true;
				default:
					return base.OnOptionsItemSelected(item);
			}
		}

		public void OnDrawerSlide(View drawerView, float slideOffset)
		{
			InputMethodManager imm = (InputMethodManager)GetSystemService(InputMethodService);
			//InputMethodManager imm = (InputMethodManager)getSystemService(Context.INPUT_METHOD_SERVICE);
			imm.HideSoftInputFromWindow(drawerView.WindowToken, 0);
		}
		public void OnDrawerOpened(View drawerView)
		{
			
		}
		public void OnDrawerStateChanged(int newState)
		{

		}

		public void OnDrawerClosed(View drawerView)
		{
			if (fragmentClass != null && tapped)
			{
				InstantiateFragment();
				tapped = false;
			}
			MainBuildingView.drawerState = false;
		}

		void NavViewItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
		{
			var mItemId = e.MenuItem.ItemId;

			switch (mItemId)
			{
				case (Resource.Id.nav_gz_menu):
					Toast.MakeText(this, GetString(Resource.String.mainbuilding_nav), ToastLength.Short).Show();
					startActivity = "mainbuilding";
					break;
				case (Resource.Id.nav_buildings_menu):
					Toast.MakeText(this, GetString(Resource.String.buildings_nav), ToastLength.Short).Show();
					startActivity = "buildings";
					break;
				case (Resource.Id.nav_rasp_menu):
					Toast.MakeText(this, GetString(Resource.String.schedule_nav), ToastLength.Short).Show();
					startActivity = "schedule";
					break;
				case (Resource.Id.nav_settings_menu):
					Toast.MakeText(this, GetString(Resource.String.settings_nav), ToastLength.Short).Show();
					startActivity = "settings";
					break;
				case (Resource.Id.nav_about_menu):
					startActivity = "about";
					break;
			}
			Title = e.MenuItem.TitleFormatted.ToString();
			e.MenuItem.SetChecked(true);
			tapped = true;
			drawerLayout.CloseDrawers();
		}
	}
}

