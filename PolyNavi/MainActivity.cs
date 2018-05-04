using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
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
		private bool tapped = false;

		private static string DatabaseFilename = "schedule.sqlite";
		public static PolyNaviLib.BL.PolyManager PolyManager { get; private set; }

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			Initialize();
			
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

			var ft = FragmentManager.BeginTransaction();
			ft.AddToBackStack(null);
			ft.Add(Resource.Id.contentframe_main, new MainBuildingFragment());
			ft.Commit();
		}

		private void Initialize()
		{
			Task.Run(async () =>
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
				fragment = (Fragment)Activator.CreateInstance(fragmentClass);
				FragmentManager.BeginTransaction().Replace(Resource.Id.contentframe_main, fragment).Commit();
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
					fragmentClass = typeof(MainBuildingFragment);
					break;
				case (Resource.Id.nav_buildings_menu):
					Toast.MakeText(this, GetString(Resource.String.buildings_nav), ToastLength.Short).Show();
					break;
				case (Resource.Id.nav_rasp_menu):
					Toast.MakeText(this, GetString(Resource.String.schedule_nav), ToastLength.Short).Show();
					fragmentClass = typeof(ScheduleFragment);
					break;
				case (Resource.Id.nav_settings_menu):
					Toast.MakeText(this, GetString(Resource.String.settings_nav), ToastLength.Short).Show();
					break;
			}
			Title = e.MenuItem.TitleFormatted.ToString();
			e.MenuItem.SetChecked(true);
			tapped = true;
			drawerLayout.CloseDrawers();
		}
	}
}

