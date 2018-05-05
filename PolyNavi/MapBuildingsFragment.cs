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

namespace PolyNavi
{
	public class MapBuildingsFragment : Fragment
	{
		View view;

		MapControl mapControl;
		Map map;
		
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
			map.Layers.Add(OpenStreetMap.CreateTileLayer());

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
			map.Widgets.Add(new Mapsui.Widgets.Zoom.ZoomInOutWidget(map) { MarginX = 20, MarginY = 40 });

			return view;
		}
	}
}