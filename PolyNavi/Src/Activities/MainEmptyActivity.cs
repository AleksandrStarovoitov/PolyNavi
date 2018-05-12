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

namespace PolyNavi
{
	[Activity(Label = "PolyNavi", MainLauncher = true)]
	public class MainEmptyActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your application here
			Intent intent;
			if (MainApp.Instance.SharedPreferences.GetBoolean("auth", false))
			{
				intent = new Intent(this, typeof(MainActivity));
			}
			else
			{
				intent = new Intent(this, typeof(AuthorizationActivity));
			}
			StartActivity(intent);
		}
	}
}