using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        CancellationTokenSource cts;
        public override void OnDisplayPreferenceDialog(Preference preference)
        {
            PreferenceDialogFragmentCompat dialogFragment = null;
            if (preference is AutoCompleteTextViewPreference) {
                cts = new CancellationTokenSource();
                dialogFragment = AutoCompleteTextViewPreferenceDialogFragmentCompat
                        .NewInstance(preference.Key, cts);
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

                var status = MainApp.Instance.GroupsDictionary.Task.Status;
                var isStarted = MainApp.Instance.GroupsDictionary.IsStarted;

                if (isStarted && status != TaskStatus.RanToCompletion)
                {
                    cts.Cancel();

                    cts = new CancellationTokenSource();
                    MainApp.Instance.GroupsDictionary = new Nito.AsyncEx.AsyncLazy<Dictionary<string, int>>(async () => await MainApp.FillGroupsDictionary(false, cts.Token));
                }

                var dictionary = MainApp.Instance.GroupsDictionary.Task.Result;
                    
                if (dictionary.ContainsKey(groupName))
                {
                    var id = dictionary[groupName];

                    sharedPreferences.Edit().PutInt("groupid", id).Apply();                        
                }
                else
                {
                    Toast.MakeText(Activity.BaseContext, "Wrong group", ToastLength.Short).Show();
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