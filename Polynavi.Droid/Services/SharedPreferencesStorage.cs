using Android.Content;
using Polynavi.Common.Services;

namespace Polynavi.Droid.Services
{
    public class SharedPreferencesStorage : IKeyValueStorage
    {
        private readonly ISharedPreferences sharedPreferences;

        public SharedPreferencesStorage(ISharedPreferences sharedPreferences)
        {
            this.sharedPreferences = sharedPreferences;
        }

        public bool GetBoolean(string key, bool defaultValue) =>
            sharedPreferences.GetBoolean(key, defaultValue);

        public string GetString(string key, string defaultValue) =>
            sharedPreferences.GetString(key, defaultValue);

        public int GetInt(string key, int defaultValue) =>
            sharedPreferences.GetInt(key, defaultValue);

        public long GetLong(string key, long defaultValue) =>
            sharedPreferences.GetLong(key, defaultValue);

        public void PutBoolean(string key, bool value) =>
            sharedPreferences.Edit()
                .PutBoolean(key, value)
                .Apply();

        public void PutString(string key, string value) =>
            sharedPreferences.Edit()
                .PutString(key, value)
                .Apply();

        public void PutInt(string key, int value) =>
            sharedPreferences.Edit()
                .PutInt(key, value)
                .Apply();

        public void PutLong(string key, long value) =>
            sharedPreferences.Edit()
                .PutLong(key, value)
                .Apply();

        public void Remove(string key) =>
            sharedPreferences.Edit()
                .Remove(key)
                .Apply();

        public bool Contains(string key) =>
            sharedPreferences.Contains(key);

        public void AddOnChangeListener(object listener) =>
            sharedPreferences.RegisterOnSharedPreferenceChangeListener(listener 
                as ISharedPreferencesOnSharedPreferenceChangeListener);

        public void RemoveOnChangeListener(object listener) =>
            sharedPreferences.UnregisterOnSharedPreferenceChangeListener(listener
                as ISharedPreferencesOnSharedPreferenceChangeListener);
    }
}
