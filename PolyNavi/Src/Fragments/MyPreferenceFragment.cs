using Android.Content;
using Android.OS;
using AndroidX.Preference;
using Polynavi.Common.Constants;
using PolyNavi.Preferences;

namespace PolyNavi.Fragments
{
    public class MyPreferenceFragment : PreferenceFragmentCompat, ISharedPreferencesOnSharedPreferenceChangeListener
    {
        private Preference groupNumberPreference;
        private Preference teacherNamePreference;

        public override void OnDisplayPreferenceDialog(Preference preference)
        {
            if (preference is AutoCompleteTextViewPreference)
            {
                var dialogFragment = AutoCompleteTextViewPreferenceDialogFragmentCompat
                    .NewInstance(preference.Key);

                dialogFragment.SetTargetFragment(this, 0);
                dialogFragment.Show(FragmentManager,
                    "android.support.v7.preference.PreferenceFragment.DIALOG"); //TODO AndroidX?

                return;
            }

            base.OnDisplayPreferenceDialog(preference);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            MainApp.Instance.SharedPreferences.UnregisterOnSharedPreferenceChangeListener(this);
        }

        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            if (key.Equals(PreferenceConstants.IsUserTeacherPreferenceKey))
            {
                TogglePreferences();
            }
        }

        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            AddPreferencesFromResource(Resource.Xml.preferences);

            groupNumberPreference = FindPreference(PreferenceConstants.GroupNumberPreferenceKey);
            teacherNamePreference = FindPreference(PreferenceConstants.TeacherNamePreferenceKey);

            TogglePreferences();

            MainApp.Instance.SharedPreferences.RegisterOnSharedPreferenceChangeListener(this);
        }

        private void TogglePreferences()
        {
            var isTeacher =
                PreferenceManager.SharedPreferences.GetBoolean(PreferenceConstants.IsUserTeacherPreferenceKey, false);

            if (isTeacher)
            {
                PreferenceScreen.RemovePreference(groupNumberPreference);
                PreferenceScreen.AddPreference(teacherNamePreference);
            }
            else
            {
                PreferenceScreen.RemovePreference(teacherNamePreference);
                PreferenceScreen.AddPreference(groupNumberPreference);
            }
        }
    }
}
