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

namespace PolyNavi
{
	public class MainBuildingMapFragment : Android.Support.V4.App.Fragment
	{
		public MainBuildingView MapView { get; set; }
		public int DrawawbleId { get; set; }

		public MainBuildingMapFragment(int id)
		{
			DrawawbleId = id;
		}

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			MapView = new MainBuildingView(Activity.BaseContext, DrawawbleId);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			return MapView;
		}
	}
}