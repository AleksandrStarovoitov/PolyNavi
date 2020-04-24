using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.AppCompat.App;
using Polynavi.Common.Constants;
using Polynavi.Common.Services;
using Polynavi.Droid.Utils;

namespace Polynavi.Droid.Activities
{
    [Activity(
        Label = "PolyNavi",
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class AuthorizationActivity : AppCompatActivity, TextView.IOnEditorActionListener
    {
        private AutoCompleteTextView autoCompleteTextView;
        private ISettingsStorage settingsStorage;
        private bool isTeacher;
        private TextChangeListener textChangeListener;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            MainApp.ChangeLanguage(this);

            SetTheme(Resource.Style.MyAppTheme);
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_authorization);

            Setup();
        }

        private void Setup()
        {
            autoCompleteTextView = FindViewById<AutoCompleteTextView>(Resource.Id.autocompletetextview_auth);
            autoCompleteTextView.SetOnEditorActionListener(this);

            textChangeListener = new TextChangeListener(this, autoCompleteTextView);
            autoCompleteTextView.AddTextChangedListener(textChangeListener);

            var buttonAuth = FindViewById<Button>(Resource.Id.button_auth);
            buttonAuth.Click += ButtonAuth_Click;

            var skipAuthTextView = FindViewById<TextView>(Resource.Id.textview_auth_skip);
            skipAuthTextView.Click += SkipAuthTextView_Click;

            settingsStorage = AndroidDependencyContainer.Instance.SettingsStorage;

            isTeacher = Intent.Extras.GetBoolean(UserTypeSelectActivity.IsTeacherIntentExtraName);

            var titleTextView = FindViewById<TextView>(Resource.Id.textview_auth_edittext_title);

            if (isTeacher)
            {
                skipAuthTextView.Text = Resources.GetString(Resource.String.auth_teacher_later);
                titleTextView.Text = Resources.GetString(Resource.String.auth_teacher_title);
                autoCompleteTextView.Hint = Resources.GetString(Resource.String.auth_teacher_hint);
            }
            else
            {
                skipAuthTextView.Text = Resources.GetString(Resource.String.group_later);
                titleTextView.Text = Resources.GetString(Resource.String.edittext_title);
                autoCompleteTextView.Hint = Resources.GetString(Resource.String.edittext_auth);
            }
        }

        private void ButtonAuth_Click(object sender, EventArgs e)
        {
            CheckGroupNumberAndProceedToMainActivity();
        }

        public bool OnEditorAction(TextView v, [GeneratedEnum] ImeAction actionId, KeyEvent e)
        {
            if (IsGoButtonPressed())
            {
                CheckGroupNumberAndProceedToMainActivity();
            }

            return false;

            bool IsGoButtonPressed() => actionId == ImeAction.Go;
        }

        private void CheckGroupNumberAndProceedToMainActivity()
        {
            if (textChangeListener.SuggestionsAndIds.TryGetValue(autoCompleteTextView.Text, out var id))
            {
                if (isTeacher)
                {
                    settingsStorage.PutString(PreferenceConstants.TeacherNamePreferenceKey,
                        autoCompleteTextView.Text);
                    settingsStorage.PutInt(PreferenceConstants.TeacherIdPreferenceKey, id);
                }
                else
                {
                    settingsStorage.PutString(PreferenceConstants.GroupNumberPreferenceKey,
                        autoCompleteTextView.Text);
                    settingsStorage.PutInt(PreferenceConstants.GroupIdPreferenceKey, id);
                }
                ProceedToMainActivity();
            }
            else
            {
                autoCompleteTextView.Error = GetString(isTeacher
                    ? Resource.String.wrong_teacher
                    : Resource.String.wrong_group);
            }
        }

        private void SkipAuthTextView_Click(object sender, EventArgs e)
        {
            ProceedToMainActivity();
        }

        private void ProceedToMainActivity()
        {
            settingsStorage.PutBoolean(PreferenceConstants.AuthCompletedPreferenceKey, true);
            settingsStorage.PutBoolean(PreferenceConstants.IsUserTeacherPreferenceKey, isTeacher);

            var mainIntent = new Intent(this, typeof(MainActivity));
            mainIntent.SetFlags(ActivityFlags.ClearTop);

            StartActivity(mainIntent);
            Finish();
        }
    }
}
