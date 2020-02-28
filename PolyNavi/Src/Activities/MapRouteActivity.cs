using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using PolyNavi.Adapters;

namespace PolyNavi.Activities
{
    [Activity(
		Label = "PolyNavi",
		ScreenOrientation = ScreenOrientation.Portrait,
		ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
	public class MapRouteActivity : AppCompatActivity
	{
        private BuildingsAdapter adapterBuildings;
        private List<object> buildings;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			MainApp.ChangeLanguage(this);
			SetTheme(Resource.Style.MyAppTheme);
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.activity_map_routing);
            Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
            Window.SetStatusBarColor(new Color(ContextCompat.GetColor(this, Resource.Color.color_primary_dark)));

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar_route);
			SetSupportActionBar(toolbar);
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);
			
			Title = GetString(Resource.String.title_route_activity);

			buildings = new List<object>(MainApp.Instance.BuildingsDictionary.Keys);
			buildings[0] = new MainBuildingTag() { MainBuildingString = buildings[0].ToString() };

			var listView = FindViewById<ListView>(Resource.Id.listview_buildingslist);



			adapterBuildings = new BuildingsAdapter(this, buildings);
			listView.Adapter = adapterBuildings;
			listView.ItemClick += ListView_ItemClick;
		}

		private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
		{
			var obj = buildings[e.Position];

			string route = obj.ToString();
			var intent = new Intent();
			intent.PutExtra("route", route);
			SetResult(Result.Ok, intent);
			Finish();
		}

		public override bool OnSupportNavigateUp()
		{
			Finish();
			return true;
		}
	}
}