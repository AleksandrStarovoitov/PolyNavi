using System.Collections.Generic;

using Android.Content;

using PolyNaviLib.BL;
using PolyNaviLib.DAL;

namespace PolyNavi
{
    public class SettingsProvider : ISettingsProvider
	{
        private readonly IDictionary<string, object> preferences;

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
			    if (preferences != null) preferences[key] = value;
			}
		}
	}
}