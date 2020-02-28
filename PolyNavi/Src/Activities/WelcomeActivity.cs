using System;

using Android.Animation;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace PolyNavi
{
    [Activity(Label = "WelcomeActivity", ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class WelcomeActivity : AppCompatActivity
    {
        private WelcomeSectionsPagerAdapter mSectionsPagerAdapter;
        private static ViewPager mViewPager;
        private static ImageButton mNextBtn;
        private static Button mSkipBtn, mFinishBtn;

        private ImageView zero, one, two;
        private static ImageView[] indicators;

        private static int page;
        private static int[] colorList;
        private static ArgbEvaluator evaluator;
        private static Color[] colors;

        private ISharedPreferencesEditor prefEditor;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Window.DecorView.SystemUiVisibility = StatusBarVisibility.Hidden;
            var window = Window;
            window.SetStatusBarColor(Color.Transparent);

            SetContentView(Resource.Layout.activity_welcome);

            mSectionsPagerAdapter = new WelcomeSectionsPagerAdapter(SupportFragmentManager);

            mNextBtn = FindViewById<ImageButton>(Resource.Id.button_welcome_next);
            mSkipBtn = FindViewById<Button>(Resource.Id.button_welcome_skip);
            mFinishBtn = FindViewById<Button>(Resource.Id.button_welcome_finish);

            zero = FindViewById<ImageView>(Resource.Id.imageview_welcome_indicator_0);
            one = FindViewById<ImageView>(Resource.Id.imageview_welcome_indicator_1);
            two = FindViewById<ImageView>(Resource.Id.imageview_welcome_indicator_2);

            indicators = new[] { zero, one, two };

            mViewPager = FindViewById<ViewPager>(Resource.Id.viewpager_welcome);
            mViewPager.Adapter = mSectionsPagerAdapter;
            mViewPager.CurrentItem = page;
            UpdateIndicators(page);

            int color1 = ContextCompat.GetColor(this, Resource.Color.color_cyan);
            int color2 = ContextCompat.GetColor(this, Resource.Color.color_orange);
            int color3 = ContextCompat.GetColor(this, Resource.Color.color_green);

            colorList = new[] { color1, color2, color3 };
            colors = new[] { new Color(color1), new Color(color2), new Color(3) };

            evaluator = new ArgbEvaluator();

            mViewPager.AddOnPageChangeListener(new ViewPagerPageChangeListener());

            mNextBtn.Click += MNextBtn_Click;
            mSkipBtn.Click += MSkipBtn_Click;
            mFinishBtn.Click += MFinishBtn_Click;

            prefEditor = MainApp.Instance.SharedPreferences.Edit();
        }

        private void MNextBtn_Click(object sender, EventArgs e)
        {
            page += 1;
            mViewPager.CurrentItem = page;
        }

        private void MSkipBtn_Click(object sender, EventArgs e)
        {
            ProccedToAuthActivity();
        }

        private void MFinishBtn_Click(object sender, EventArgs e)
        {
            ProccedToAuthActivity();
        }

        private void ProccedToAuthActivity()
        {
            prefEditor.PutBoolean("welcome", true).Apply();
            var intent = new Intent(this, typeof(AuthorizationActivity));
            intent.SetFlags(ActivityFlags.ClearTop);
            StartActivity(intent);
            Finish();
        }

        private static void UpdateIndicators(int position)
        {
            for (int i = 0; i < indicators.Length; i++)
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
                page = position;
                UpdateIndicators(page);
                
                switch (position)
                {
                    case 0:
                        mViewPager.SetBackgroundColor(colors[0]);
                        break;
                    case 1:
                        mViewPager.SetBackgroundColor(colors[1]);
                        break;
                    case 2:
                        mViewPager.SetBackgroundColor(colors[2]);
                        break;
                }

                mNextBtn.Visibility = position == 2 ? ViewStates.Gone : ViewStates.Visible;
                mFinishBtn.Visibility = position == 2 ? ViewStates.Visible : ViewStates.Gone;
            }

            public new void Dispose()
            {
             
            }

            public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
            {
                int colorUpdate = (int)evaluator.Evaluate(positionOffset, colorList[position], colorList[position == 2 ? position : position + 1]);
                var color = new Color(colorUpdate);
                mViewPager.SetBackgroundColor(color);
            }
        }
    }
}