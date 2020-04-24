using System.Collections.Generic;
using Android.Content;
using Polynavi.Common.Constants;
using Polynavi.Common.Exceptions;
using Polynavi.Common.Services;

namespace Polynavi.Droid.Services
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

                if (key == PreferenceConstants.GroupIdPreferenceKey)
                {
                    throw new GroupNumberException();
                }

                //throw new KeyNotFoundException();
                return null; //TODO
            }
            set
            {
                if (preferences != null)
                {
                    preferences[key] = value;
                }
            }
        }
    }
}
