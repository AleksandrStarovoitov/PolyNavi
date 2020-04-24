using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Polynavi.Common.Constants;

namespace Polynavi.Droid.Activities
{
    [Activity(
        Label = "PolyNavi",
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize,
        MainLauncher = true)] //TODO Check all activities attributes
    public class MainEmptyActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var intent = GetStartIntent();

            StartActivity(intent);
            Finish();
        }

        private Intent GetStartIntent()
        {
            var preferences = AndroidDependencyContainer.Instance.SettingsStorage;

            var isAuthCompleted = preferences.GetBoolean(PreferenceConstants.AuthCompletedPreferenceKey, false);
            var isWelcomeCompleted = preferences.GetBoolean(PreferenceConstants.WelcomeCompletedPreferenceKey, false);

            if (isAuthCompleted)
            {
                return new Intent(this, typeof(MainActivity));
            }

            return isWelcomeCompleted
                ? new Intent(this, typeof(UserTypeSelectActivity))
                : new Intent(this, typeof(WelcomeActivity));
        }
    }
}
