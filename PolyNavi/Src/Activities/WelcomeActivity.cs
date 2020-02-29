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
using System;

namespace PolyNavi.Activities
{
    [Activity(Label = "WelcomeActivity", ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class WelcomeActivity : AppCompatActivity
    {
        private WelcomeSectionsPagerAdapter sectionsPagerAdapter;
        private static ViewPager viewPager;
        private static ImageButton nextButton;
        private static Button skipButton, finishButton;
        private ImageView firstIndicator, secondIndicator, thirdIndicator;
        private static ImageView[] indicators;
        private static int pageNumber;
        private static int[] colorList;
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

            sectionsPagerAdapter = new WelcomeSectionsPagerAdapter(SupportFragmentManager);

            nextButton = FindViewById<ImageButton>(Resource.Id.button_welcome_next);
            skipButton = FindViewById<Button>(Resource.Id.button_welcome_skip);
            finishButton = FindViewById<Button>(Resource.Id.button_welcome_finish);

            firstIndicator = FindViewById<ImageView>(Resource.Id.imageview_welcome_indicator_1);
            secondIndicator = FindViewById<ImageView>(Resource.Id.imageview_welcome_indicator_2);
            thirdIndicator = FindViewById<ImageView>(Resource.Id.imageview_welcome_indicator_3);

            indicators = new[] { firstIndicator, secondIndicator, thirdIndicator };

            viewPager = FindViewById<ViewPager>(Resource.Id.viewpager_welcome);
            viewPager.Adapter = sectionsPagerAdapter;
            viewPager.CurrentItem = pageNumber;
            UpdateIndicators(pageNumber);

            var color1 = ContextCompat.GetColor(this, Resource.Color.color_cyan);
            var color2 = ContextCompat.GetColor(this, Resource.Color.color_orange);
            var color3 = ContextCompat.GetColor(this, Resource.Color.color_green);

            colorList = new[] { color1, color2, color3 };
            colors = new[] { new Color(color1), new Color(color2), new Color(3) };

            argbEvaluator = new ArgbEvaluator();

            viewPager.AddOnPageChangeListener(new ViewPagerPageChangeListener());

            nextButton.Click += NextButtonClick;
            skipButton.Click += SkipButtonClick;
            finishButton.Click += FinishButtonClick;

            preferencesEditor = MainApp.Instance.SharedPreferences.Edit();
        }

        private void NextButtonClick(object sender, EventArgs e)
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
            preferencesEditor.PutBoolean("welcome", true).Apply();
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


        private class ViewPagerPageChangeListener : Java.Lang.Object, ViewPager.IOnPageChangeListener
        {
            public void OnPageScrollStateChanged(int state)
            {

            }

            public void OnPageSelected(int position)
            {
                pageNumber = position;
                UpdateIndicators(pageNumber);

                switch (position)
                {
                    case 0:
                        viewPager.SetBackgroundColor(colors[0]);
                        break;
                    case 1:
                        viewPager.SetBackgroundColor(colors[1]);
                        break;
                    case 2:
                        viewPager.SetBackgroundColor(colors[2]);
                        break;
                }

                nextButton.Visibility = position == 2 ? ViewStates.Gone : ViewStates.Visible;
                finishButton.Visibility = position == 2 ? ViewStates.Visible : ViewStates.Gone;
            }

            public new void Dispose()
            {

            }

            public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
            {
                var colorUpdate = (int)argbEvaluator.Evaluate(positionOffset, colorList[position], colorList[position == 2 ? position : position + 1]);
                var color = new Color(colorUpdate);
                viewPager.SetBackgroundColor(color);
            }
        }
    }
}