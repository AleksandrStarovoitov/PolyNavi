using Android.Content;
using Polynavi.Common.Services;

namespace Polynavi.Droid.Services
{
    public class SharedPreferencesStorage : ISettingsStorage
    {
        private readonly ISharedPreferences sharedPreferences;

        public SharedPreferencesStorage(ISharedPreferences sharedPreferences)
        {
            this.sharedPreferences = sharedPreferences;
        }

        public bool GetBoolean(string key, bool defaultValue)
            => sharedPreferences.GetBoolean(key, defaultValue);

        public string GetString(string key, string defaultValue)
            => sharedPreferences.GetString(key, defaultValue);

        public int GetInt(string key, int defaultValue)
            => sharedPreferences.GetInt(key, defaultValue);

        public long GetLong(string key, long defaultValue)
            => sharedPreferences.GetLong(key, defaultValue);

        public void PutBoolean(string key, bool value)
        {
            var editor = sharedPreferences.Edit();
            editor.PutBoolean(key, value);
            editor.Commit();
        }

        public void PutString(string key, string value)
        {
            var editor = sharedPreferences.Edit();
            editor.PutString(key, value);
            editor.Commit();
        }

        public void PutInt(string key, int value)
        {
            var editor = sharedPreferences.Edit();
            editor.PutInt(key, value);
            editor.Commit();
        }

        public void PutLong(string key, long value)
        {
            var editor = sharedPreferences.Edit();
            editor.PutLong(key, value);
            editor.Commit();
        }

        public void Remove(string key)
        {
            var editor = sharedPreferences.Edit();
            editor.Remove(key);
            editor.Commit();
        }

        public bool Contains(string key) =>
            sharedPreferences.Contains(key);

        public void AddOnChangeListener(object listener)
        {
            sharedPreferences.RegisterOnSharedPreferenceChangeListener(listener 
                as ISharedPreferencesOnSharedPreferenceChangeListener);
        }

        public void RemoveOnChangeListener(object listener)
        {
            sharedPreferences.UnregisterOnSharedPreferenceChangeListener(listener
                as ISharedPreferencesOnSharedPreferenceChangeListener);
        }
    }
}
