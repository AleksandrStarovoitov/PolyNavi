using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

using Mapsui.Geometries;
using Mapsui.UI;
using Mapsui.UI.Android;
using Mapsui.Utilities;
using Mapsui.Projection;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui;
using Android.Support.Design.Widget;
using BruTile.Predefined;

namespace PolyNavi
{
	public class MapBuildingsFragment : Fragment
	{
		private View view;

		private MapControl mapControl;
		private Map map;
		private FloatingActionButton fab;
		public static readonly Dictionary<string, Point> BuildingsDictionary = new Dictionary<string, Point>()
		{
			{ "Главный учебный корпус", new Point(60.00718, 30.37281)},
			{ "Химический корпус", new Point(60.00648, 30.37630)},
			{ "Механический корпус", new Point(60.00768, 30.37628)},
			{ "Гидрокорпус-1", new Point(60.00565, 30.38176)},
			{ "Гидрокорпус-2", new Point(60.00670, 30.38266)},
			{ "НИК", new Point(59.99828, 30.37347)},
			{ "1-й учебный корпус", new Point(60.00885, 30.37270)},
			{ "2-й учебный корпус", new Point(60.00846, 30.37492)},
			{ "3-й учебный корпус", new Point(60.00711, 30.38149)},
			{ "4-й учебный корпус", new Point(60.00750, 30.37694)},
			{ "5-й учебный корпус", new Point(59.99984, 30.37438)},
			{ "6-й учебный корпус", new Point(60.00048, 30.36805)},
			{ "9-й учебный корпус", new Point(60.00081, 30.36619)},
			{ "10-й учебный корпус", new Point(60.00066, 30.36902)},
			{ "11-й учебный корпус", new Point(60.00900, 30.37744)},
			{ "15-й учебный корпус (ИМОП)", new Point(60.00714, 30.39050	)},
			{ "16-й учебный корпус", new Point(60.00790, 30.39041)},
			{ "Спортивный комплекс", new Point(60.00295, 30.36801)},

			{ "Лабораторный корпус", new Point(60.00734, 30.37954)},
			{ "Гидробашня", new Point(60.00583, 30.37428)},
			{ "НОЦ РАН", new Point(60.00317, 30.37468)},
			{ "1-й профессорский корпус", new Point(60.00481, 30.37071)},
			{ "2-й профессорский корпус", new Point(60.00475, 30.37796)},
			{ "Дом ученых в Лесном", new Point(60.00448, 30.37908)},
			{ "Секретариат приемной комиссии", new Point(60.00048, 30.36805)}
		};
		
		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			// Create your fragment here
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			view = inflater.Inflate(Resource.Layout.fragment_map_buildings, container, false);

			mapControl = view.FindViewById<MapControl>(Resource.Id.mapControl);
			mapControl.RotationLock = false;
			map = mapControl.Map;
		
			//map.BackColor = Mapsui.Styles.Color.Black;
			map.CRS = "EPSG:3857";
			//map.Layers.Add(OpenStreetMap.CreateTileLayer());

			map.Layers.Add(new TileLayer(KnownTileSources.Create(KnownTileSource.EsriWorldTopo)));
			//EsriWorldTopo

			Point centerOfPolytech = new Point(30.371144, 60.003675).FromLonLat();
			map.NavigateTo(centerOfPolytech);
			map.NavigateTo(7);
			map.Transformation = new MinimalTransformation();

			Point leftBot = new Point(30.365751, 59.999560).FromLonLat();
			Point rightTop = new Point(30.391848, 60.008916).FromLonLat();
			map.PanLimits = new BoundingBox(leftBot, rightTop);
			map.PanMode = PanMode.KeepCenterWithinExtents;
			

			map.ZoomLimits = new MinMax(1, 7);

			map.Widgets.Add(new Mapsui.Widgets.ScaleBar.ScaleBarWidget(map) { TextAlignment = Mapsui.Widgets.Alignment.Center, HorizontalAlignment = Mapsui.Widgets.HorizontalAlignment.Center, VerticalAlignment = Mapsui.Widgets.VerticalAlignment.Top });

			fab = view.FindViewById<FloatingActionButton>(Resource.Id.new_fab_buildings);
			fab.Click += Fab_Click;

			return view;
		}

		private void Fab_Click(object sender, EventArgs e)
		{
			var searchActivity = new Intent(Activity, typeof(MapRouteActivity));
			StartActivityForResult(searchActivity, MainActivity.RequestCode);
		}

		public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			if (requestCode == MainActivity.RequestCode)
			{
				if (resultCode == Result.Ok)
				{
					string[] route = data.GetStringArrayExtra("route");
					//string start = route[0];
					//string end = route[1];
				}
			}
		}
	}
}