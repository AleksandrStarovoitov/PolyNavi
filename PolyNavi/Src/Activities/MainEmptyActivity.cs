using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using PolyNaviLib.Constants;

namespace PolyNavi.Activities
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
            var preferences = MainApp.Instance.SharedPreferences;

            var isAuthCompleted = preferences.GetBoolean(PreferenceConstants.AuthCompletedPreferenceKey, false);
            var isWelcomeCompleted = preferences.GetBoolean(PreferenceConstants.WelcomeCompletedPreferenceKey, false);

            if (isAuthCompleted)
            {
                return new Intent(this, typeof(MainActivity));
            }

            if (isWelcomeCompleted)
            {
                return new Intent(this, typeof(UserTypeSelectActivity));
            }

            return new Intent(this, typeof(WelcomeActivity));
        }
    }
}