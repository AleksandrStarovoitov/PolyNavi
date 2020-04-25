using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Preference;
using Polynavi.Bll.Settings;
using Polynavi.Droid.Preferences;

namespace Polynavi.Droid.Fragments
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

            PreferenceManager.GetDefaultSharedPreferences(Application.Context)
                .UnregisterOnSharedPreferenceChangeListener(this); 
        }

        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            if (key.Equals(SettingsStorage.IsUserTeacherKey))
            {
                TogglePreferences();
            }
        }

        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            AddPreferencesFromResource(Resource.Xml.preferences);

            groupNumberPreference = FindPreference(SettingsStorage.GroupNumberKey);
            teacherNamePreference = FindPreference(SettingsStorage.TeacherNameKey);

            TogglePreferences();

            PreferenceManager.GetDefaultSharedPreferences(Application.Context)
                .RegisterOnSharedPreferenceChangeListener(this);
        }

        private void TogglePreferences()
        {
            var isTeacher = AndroidDependencyContainer.Instance.ScheduleSettings.IsUserTeacher;

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
