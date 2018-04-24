using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using PolyNaviLib.BL;

namespace PolyNavi
{

	//
	//Добавить разрешение в manifest
	//<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
	//

	public class NetworkChecker : INetworkChecker
	{
		public bool Check()
		{
			//ConnectivityManager cm = (ConnectivityManager)GetSystemService(ConnectivityService);
			//NetworkInfo info = cm.ActiveNetworkInfo;
			//if (info != null && info.IsConnected)
			//{
			//	return true;
			//}
			//else
			//{
			//	return false;
			//}

			return true;
		}
	}
}