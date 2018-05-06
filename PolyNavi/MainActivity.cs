using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

namespace PolyNavi
{
	[Activity(Label = "PolyNavi", MainLauncher = true)]
	public class MainActivity : AppCompatActivity, IDrawerListener
	{
		private DrawerLayout drawerLayout;
		private ActionBarDrawerToggle drawerToggle;
		private Type fragmentClass = null;
		private Fragment fragment;
		private Android.Support.V7.Widget.Toolbar toolbar;
		private NavigationView navigationView;
		private string startActivity;
		private int startMenuItem;
		private bool tapped = false;

		public static int RequestCode = 1;
		public static ISharedPreferences sharedPreferences;

		private const string DatabaseFilename = "schedule.sqlite";
		public static PolyManager PolyManager { get; private set; }

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this);
			startActivity = sharedPreferences.GetString("startactivity_preference", null);

			var initTask = Initialize();
			
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
			//Thread.Sleep(100);
			if (!initTask.IsCompleted)
			{
				initTask.Wait();
			}
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
				default:
					fragmentClass = typeof(MainBuildingFragment);
					startMenuItem = 0;
					break;
			}
			fragment = (Fragment)Activator.CreateInstance(fragmentClass);
			navigationView.Menu.GetItem(startMenuItem).SetChecked(true);
			Title = navigationView.Menu.GetItem(startMenuItem).TitleFormatted.ToString();
			FragmentManager.BeginTransaction().Replace(Resource.Id.contentframe_main, fragment).Commit();
		}


		private Task Initialize()
		{
			return Task.Run(async () =>
			{
				string dirPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
				string path = System.IO.Path.Combine(dirPath, DatabaseFilename);
				PolyManager = await PolyManager.CreateAsync(path, new NetworkChecker());
			});
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
			}
			Title = e.MenuItem.TitleFormatted.ToString();
			e.MenuItem.SetChecked(true);
			tapped = true;
			drawerLayout.CloseDrawers();
		}
	}
}

