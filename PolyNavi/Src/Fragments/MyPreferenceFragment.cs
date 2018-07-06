using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace PolyNavi
{
	public class MyPreferenceFragment : PreferenceFragment, ISharedPreferencesOnSharedPreferenceChangeListener
	{
        ISharedPreferences sharedPreferences;

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			AddPreferencesFromResource(Resource.Xml.preferences);

            MainApp.Instance.SharedPreferences.RegisterOnSharedPreferenceChangeListener(this);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            MainApp.Instance.SharedPreferences.UnregisterOnSharedPreferenceChangeListener(this);
        }
        
        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            if (key.Equals("groupnumber"))
            {
                var groupName = sharedPreferences.GetString(key, "-1");
                var dictionary = MainApp.Instance.GroupsDictionary.Task.Result;
                if (dictionary.ContainsKey(groupName))
                {
                    var id = dictionary[groupName];

                    sharedPreferences.Edit().PutInt("groupid", id).Apply();

                    Toast.MakeText(Activity.BaseContext, id.ToString(), ToastLength.Short).Show();
                }
                else
                {
                    Toast.MakeText(Activity.BaseContext, "Wrong group", ToastLength.Short).Show();
                }
            }
        }
    }
}