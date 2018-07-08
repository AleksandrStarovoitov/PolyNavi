using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
//using Android.Preferences;
using Android.Runtime;
using Android.Support.V7.Preferences;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace PolyNavi
{
	public class MyPreferenceFragment : PreferenceFragmentCompat, ISharedPreferencesOnSharedPreferenceChangeListener
	{
        ISharedPreferences sharedPreferences;

        //public override void OnCreate(Bundle savedInstanceState)
        //{
        //	base.OnCreate(savedInstanceState);
        //	AddPreferencesFromResource(Resource.Xml.preferences);

        //          MainApp.Instance.SharedPreferences.RegisterOnSharedPreferenceChangeListener(this);
        //}

        public override void OnDisplayPreferenceDialog(Preference preference)
        {
            //base.OnDisplayPreferenceDialog(preference);

            PreferenceDialogFragmentCompat dialogFragment = null;
            if (preference is AutoCompleteTextViewPreference) {
                // Create a new instance of TimePreferenceDialogFragment with the key of the related
                // Preference
                dialogFragment = AutoCompleteTextViewPreferenceDialogFragmentCompat
                        .NewInstance(preference.Key);
            }

            // If it was one of our cutom Preferences, show its dialog
            if (dialogFragment != null)
            {
                dialogFragment.SetTargetFragment(this, 0);
                dialogFragment.Show(FragmentManager, "android.support.v7.preference.PreferenceFragment.DIALOG");
            }
            // Could not be handled here. Try with the super method.
            else
            {
                base.OnDisplayPreferenceDialog(preference);
            }


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

        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            //base.OnCreatePreferences(savedInstanceState, rootKey);

            AddPreferencesFromResource(Resource.Xml.preferences);

            MainApp.Instance.SharedPreferences.RegisterOnSharedPreferenceChangeListener(this);
        }
    }
}