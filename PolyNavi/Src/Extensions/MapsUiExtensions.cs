using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Mapsui.Geometries;
using Mapsui.Projection;

using Itinero.LocalGeo;

namespace PolyNavi
{
	public static class MapsUiExtensions
	{
		public static Point ToLonLat(this Point point)
		{
			return SphericalMercator.ToLonLat(point.X, point.Y);
		}

		public static Point FromLonLat(this Point point)
		{
			return SphericalMercator.FromLonLat(point.X, point.Y);
		}

		public static Point ToWorld(this Coordinate point)
		{
			return SphericalMercator.FromLonLat(point.Longitude, point.Latitude);
		}
	}
}