using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
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

        public override void OnDisplayPreferenceDialog(Preference preference)
        {
            PreferenceDialogFragmentCompat dialogFragment = null;
            if (preference is AutoCompleteTextViewPreference)
            {
                dialogFragment = AutoCompleteTextViewPreferenceDialogFragmentCompat
                        .NewInstance(preference.Key);
            }
            
            if (dialogFragment != null)
            {
                dialogFragment.SetTargetFragment(this, 0);
                dialogFragment.Show(FragmentManager, "android.support.v7.preference.PreferenceFragment.DIALOG");
            }
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

                if (dictionary.TryGetValue(groupName, out int id))
                {
                    sharedPreferences.Edit().PutInt("groupid", id).Apply();
                }          
            }
        }

        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            AddPreferencesFromResource(Resource.Xml.preferences);

            MainApp.Instance.SharedPreferences.RegisterOnSharedPreferenceChangeListener(this);
        }
    }
}