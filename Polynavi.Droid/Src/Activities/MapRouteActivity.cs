using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.Content;
using Polynavi.Droid.Adapters;
using Polynavi.Droid.Fragments;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace Polynavi.Droid.Activities
{
    [Activity(
        Label = "PolyNavi",
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class MapRouteActivity : AppCompatActivity
    {
        private List<object> buildingNames;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            MainApp.ChangeLanguage(this);

            SetTheme(Resource.Style.MyAppTheme);
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_map_routing);

            Setup();
        }

        private void Setup()
        {
            Window.ClearFlags(WindowManagerFlags.TranslucentStatus); //TODO Move?
            Window.SetStatusBarColor(new Color(ContextCompat.GetColor(this, Resource.Color.color_primary_dark)));

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar_route);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            Title = GetString(Resource.String.title_route_activity);

            buildingNames = new List<object>(MainApp.Instance.BuildingsDictionary.Keys);
            buildingNames[0] = new MainBuildingTag() { MainBuildingString = buildingNames[0].ToString() };

            var buildingsList = FindViewById<ListView>(Resource.Id.listview_buildingslist);

            var buildingsAdapter = new BuildingsAdapter(this, buildingNames);
            buildingsList.Adapter = buildingsAdapter;
            buildingsList.ItemClick += BuildingsList_ItemClick;
        }

        private void BuildingsList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var selectedBuildingName = buildingNames[e.Position].ToString();

            var intent = new Intent();
            intent.PutExtra(MapBuildingsFragment.MapActivityIntentResultName, selectedBuildingName);

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
