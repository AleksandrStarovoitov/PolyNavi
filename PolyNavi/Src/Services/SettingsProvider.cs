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
using PolyNaviLib.DAL;

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
                if (preferences.TryGetValue(key, out object value))
                {
                    return value;
                }
                else
                {

                    if (key == "groupid")
                        throw new GroupNumberException();
                    else
                        throw new KeyNotFoundException();
                }
            }	
			set
			{
				preferences[key] = value;
			}
		}
	}
}