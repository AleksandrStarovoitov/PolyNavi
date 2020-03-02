using System;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.Content;
using AndroidX.ViewPager.Widget;
using PolyNavi.Adapters;
using PolyNaviLib.Constants;
using Object = Java.Lang.Object;

namespace PolyNavi.Activities
{
    [Activity(Label = "WelcomeActivity", ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class WelcomeActivity : AppCompatActivity
    {
        private static ViewPager viewPager;
        private static ImageButton nextButton;
        private static Button finishButton;
        private static ImageView[] indicators;
        private static int pageNumber;
        private static int[] colorCodesList;
        private static ArgbEvaluator argbEvaluator;
        private static Color[] colors;
        private ISharedPreferencesEditor preferencesEditor;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Window.DecorView.SystemUiVisibility = StatusBarVisibility.Hidden;
            var window = Window;
            window.SetStatusBarColor(Color.Transparent);

            SetContentView(Resource.Layout.activity_welcome);

            Setup();
        }

        private void Setup()
        {
            var sectionsPagerAdapter = new WelcomeSectionsPagerAdapter(SupportFragmentManager);

            nextButton = FindViewById<ImageButton>(Resource.Id.button_welcome_next);
            var skipButton = FindViewById<Button>(Resource.Id.button_welcome_skip);
            finishButton = FindViewById<Button>(Resource.Id.button_welcome_finish);

            var firstIndicator = FindViewById<ImageView>(Resource.Id.imageview_welcome_indicator_1);
            var secondIndicator = FindViewById<ImageView>(Resource.Id.imageview_welcome_indicator_2);
            var thirdIndicator = FindViewById<ImageView>(Resource.Id.imageview_welcome_indicator_3);

            indicators = new[] { firstIndicator, secondIndicator, thirdIndicator };

            viewPager = FindViewById<ViewPager>(Resource.Id.viewpager_welcome);
            viewPager.Adapter = sectionsPagerAdapter;
            viewPager.CurrentItem = pageNumber;
            UpdateIndicators(pageNumber);

            var color1 = ContextCompat.GetColor(this, Resource.Color.color_cyan);
            var color2 = ContextCompat.GetColor(this, Resource.Color.color_orange);
            var color3 = ContextCompat.GetColor(this, Resource.Color.color_green);

            colorCodesList = new[] { color1, color2, color3 };
            colors = new[] { new Color(color1), new Color(color2), new Color(3) };

            argbEvaluator = new ArgbEvaluator();

            viewPager.AddOnPageChangeListener(new ViewPagerPageChangeListener());

            nextButton.Click += NextButtonClick;
            skipButton.Click += SkipButtonClick;
            finishButton.Click += FinishButtonClick;

            preferencesEditor = MainApp.Instance.SharedPreferences.Edit();
        }

        private static void NextButtonClick(object sender, EventArgs e)
        {
            pageNumber += 1;
            viewPager.CurrentItem = pageNumber;
        }

        private void SkipButtonClick(object sender, EventArgs e)
        {
            ProceedToAuthActivity();
        }

        private void FinishButtonClick(object sender, EventArgs e)
        {
            ProceedToAuthActivity();
        }

        private void ProceedToAuthActivity()
        {
            preferencesEditor.PutBoolean(PreferencesConstants.WelcomeCompletedPreferenceKey, true).Apply();

            var intent = new Intent(this, typeof(AuthorizationActivity));
            intent.SetFlags(ActivityFlags.ClearTop);

            StartActivity(intent);
            Finish();
        }

        private static void UpdateIndicators(int position)
        {
            for (var i = 0; i < indicators.Length; i++)
            {
                indicators[i].SetBackgroundResource(i == position ? Resource.Drawable.indicator_selected : Resource.Drawable.indicator_unselected);
            }
        }

        private class ViewPagerPageChangeListener : Object, ViewPager.IOnPageChangeListener
        {
            public void OnPageScrollStateChanged(int state)
            {

            }

            public void OnPageSelected(int position)
            {
                pageNumber = position;
                UpdateIndicators(pageNumber);

                viewPager.SetBackgroundColor(colors[position]);

                nextButton.Visibility = position == 2 ? ViewStates.Gone : ViewStates.Visible;
                finishButton.Visibility = position == 2 ? ViewStates.Visible : ViewStates.Gone;
            }

            public new void Dispose() //TODO ?
            {

            }

            public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
            {
                var colorUpdate = (int)argbEvaluator.Evaluate(positionOffset,
                    colorCodesList[position],
                    colorCodesList[position == 2 ? position : position + 1]);

                var color = new Color(colorUpdate);

                viewPager.SetBackgroundColor(color);
            }
        }
    }
}