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
	public class MainBuildingMapFragment : Fragment
	{
		View view;
		public override void OnCreate(Bundle savedInstanceState)
		{

			base.OnCreate(savedInstanceState);

			// Create your fragment here
			view = new MainBuildingView(Activity.BaseContext);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);
			return view;
			//return base.OnCreateView(inflater, container, savedInstanceState);
		}
	}
}