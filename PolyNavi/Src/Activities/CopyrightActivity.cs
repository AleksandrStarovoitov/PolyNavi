using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Webkit;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.Content;

namespace PolyNavi.Activities
{
    [Activity(
        Label = "PolyNavi",
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class CopyrightActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            MainApp.ChangeLanguage(this);

            SetTheme(Resource.Style.MyAppTheme);
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_copyright);

            Setup();
        }

        private void Setup()
        {
            Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
            Window.SetStatusBarColor(new Color(ContextCompat.GetColor(this, Resource.Color.color_primary_dark)));

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar_copyright);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            Title = GetString(Resource.String.title_copyright_activity);

            var webView = FindViewById<WebView>(Resource.Id.webview_copyright);
            webView.LoadUrl("file:///android_asset/copyright.html");
        }

        public override bool OnSupportNavigateUp()
        {
            Finish();
            return true;
        }
    }
}