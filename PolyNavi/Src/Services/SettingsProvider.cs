using System.Collections.Generic;
using Android.Content;
using PolyNaviLib.BL;
using PolyNaviLib.Constants;
using PolyNaviLib.DAL;

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

                if (key == PreferencesConstants.GroupIdPreferenceKey)
                    throw new GroupNumberException();
                throw new KeyNotFoundException();
            }
            set
            {
                if (preferences != null) preferences[key] = value;
            }
        }
    }
}