using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace PolyNavi.Activities
{
    [Activity(
        Label = "PolyNavi",
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize,
        MainLauncher = true)]
    public class MainEmptyActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Intent intent;
            if (MainApp.Instance.SharedPreferences.GetBoolean("auth", false))
            {
                intent = new Intent(this, typeof(MainActivity));
            }
            else
                if (MainApp.Instance.SharedPreferences.GetBoolean("welcome", false))
            {
                intent = new Intent(this, typeof(AuthorizationActivity));
            }
            else
            {
                intent = new Intent(this, typeof(WelcomeActivity));
            }

            StartActivity(intent);
            Finish();
        }
    }
}