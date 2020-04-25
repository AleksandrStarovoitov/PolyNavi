using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

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
            var firstLoginStatusSettings = AndroidDependencyContainer.Instance.LoginStateSettings;

            if (firstLoginStatusSettings.IsAuthCompleted)
            {
                return new Intent(this, typeof(MainActivity));
            }

            return firstLoginStatusSettings.IsWelcomeCompleted
                ? new Intent(this, typeof(UserTypeSelectActivity))
                : new Intent(this, typeof(WelcomeActivity));
        }
    }
}
