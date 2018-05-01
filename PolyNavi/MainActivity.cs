using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
using Java.Lang;
using static Android.Support.V4.Widget.DrawerLayout;

namespace PolyNavi
{
	[Activity(Label = "PolyNavi", MainLauncher = true)]
	public class MainActivity : AppCompatActivity, IDrawerListener
	{
		private DrawerLayout drawerLayout;
		ActionBarDrawerToggle mDrawerToggle;
		Type fragmentClass = null;
		Fragment fr;
		bool tapped = false;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your application here

			SetContentView(Resource.Layout.activity_main);

			drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawerlayout_main);

			drawerLayout.AddDrawerListener(this);
			drawerLayout.SetStatusBarBackground(Resource.Color.mycolorprimarydark);
			//yourtextview.setText(Html.fromHtml(text));


			var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar_main);

			mDrawerToggle = new ActionBarDrawerToggle(this, drawerLayout, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
			drawerLayout.AddDrawerListener(mDrawerToggle);

			SetSupportActionBar(toolbar);
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);
			SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);
			//SupportActionBar.SetDisplayShowHomeEnabled(true);

			var navigationView = FindViewById<NavigationView>(Resource.Id.navview_main);

			navigationView.NavigationItemSelected += NavViewItemSelected;
			//navigationView.Menu.Add(Menu.None, Menu.None, Menu.None, "Gelll");
			navigationView.Alpha = 0.99f;

			
			var ft = FragmentManager.BeginTransaction();
			ft.AddToBackStack(null);
			//ft.Add(Resource.Id.contentframe_main, new MainFragment());
			ft.Add(Resource.Id.contentframe_main, new MainBuildingFragment());
			ft.Commit();
		}

		protected override void OnPostCreate(Bundle savedInstanceState)
		{
			base.OnPostCreate(savedInstanceState);
			mDrawerToggle.SyncState();
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
			//Toast.MakeText(this, "Slide", ToastLength.Short).Show();
		}
		public void OnDrawerOpened(View drawerView)
		{
			//Toast.MakeText(this, "OPEN", ToastLength.Short).Show();
		}
		public void OnDrawerStateChanged(int newState)
		{

		}

		public void OnDrawerClosed(View drawerView)
		{
			//Toast.MakeText(this, "closed", ToastLength.Short).Show();
			//Set your new fragment here
			if (fragmentClass != null && tapped)
			{
				fr = (Fragment)Activator.CreateInstance(fragmentClass);
				FragmentManager.BeginTransaction().Replace(Resource.Id.contentframe_main, fr).Commit();
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
					Toast.MakeText(this, "ГЗ", ToastLength.Short).Show();

					fragmentClass = typeof(MainBuildingFragment);

					break;
				case (Resource.Id.nav_buildings_menu):
					Toast.MakeText(this, "Корпуса", ToastLength.Short).Show();
					break;
				case (Resource.Id.nav_rasp_menu):
					Toast.MakeText(this, "Расписание", ToastLength.Short).Show();

					//fragmentClass = typeof(ScheduleSwipeFragment);

					break;
				case (Resource.Id.nav_settings_menu):
					Toast.MakeText(this, "Настройки", ToastLength.Short).Show();

					break;
			}

			Title = e.MenuItem.TitleFormatted.ToString();
			e.MenuItem.SetChecked(true);
			tapped = true;
			drawerLayout.CloseDrawers();
		}
	}
}

