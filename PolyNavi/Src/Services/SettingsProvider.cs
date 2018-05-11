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

using PolyNaviLib.BL;

namespace PolyNavi
{
	public class SettingsProvider : ISettingsProvider
	{
		IDictionary<string, object> preferences;

		public SettingsProvider(ISharedPreferences preferences)
		{
			this.preferences = preferences.All;
		}

		public object this[string key]
		{
			get
			{
				return preferences[key];
			}
			set
			{
				preferences[key] = value;
			}
		}
	}
}