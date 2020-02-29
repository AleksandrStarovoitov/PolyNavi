using Android.Content;
using PolyNaviLib.BL;
using PolyNaviLib.DAL;
using System.Collections.Generic;

namespace PolyNavi.Services
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
                if (preferences.TryGetValue(key, out var value))
                {
                    return value;
                }
                else
                {
                    if (key == "groupid") //TODO
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