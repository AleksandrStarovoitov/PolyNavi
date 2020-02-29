using Android.Content;
using Android.OS;
using AndroidX.Preference;
using PolyNavi.Preferences;

namespace PolyNavi.Fragments
{
    public class MyPreferenceFragment : PreferenceFragmentCompat, ISharedPreferencesOnSharedPreferenceChangeListener
    {
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
                dialogFragment.Show(FragmentManager, "android.support.v7.preference.PreferenceFragment.DIALOG"); //TODO AndroidX?
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
        }

        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            AddPreferencesFromResource(Resource.Xml.preferences);

            MainApp.Instance.SharedPreferences.RegisterOnSharedPreferenceChangeListener(this);
        }
    }
}