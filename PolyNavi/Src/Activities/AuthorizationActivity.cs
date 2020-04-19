using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.AppCompat.App;
using Java.Lang;
using PolyNavi.Extensions;
using PolyNavi.Services;
using PolyNaviLib.BL;
using PolyNaviLib.Constants;

namespace PolyNavi.Activities
{
    [Activity(
        Label = "PolyNavi",
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class AuthorizationActivity : AppCompatActivity, TextView.IOnEditorActionListener, ITextWatcher
    {
        private AutoCompleteTextView autoCompleteTextViewAuth;
        private NetworkChecker networkChecker;
        private Dictionary<string, int> suggestionsAndIds;
        private Timer searchTimer;
        private ISharedPreferencesEditor preferencesEditor;
        private bool isTeacher;
        private const int MillsToSearch = 700;

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
            networkChecker = new NetworkChecker(this);

            autoCompleteTextViewAuth = FindViewById<AutoCompleteTextView>(Resource.Id.autocompletetextview_auth);
            autoCompleteTextViewAuth.SetOnEditorActionListener(this);
            autoCompleteTextViewAuth.AddTextChangedListener(this);
            suggestionsAndIds = new Dictionary<string, int>();

            var buttonAuth = FindViewById<Button>(Resource.Id.button_auth);
            buttonAuth.Click += ButtonAuth_Click;

            var skipAuthTextView = FindViewById<TextView>(Resource.Id.textview_auth_skip);
            skipAuthTextView.Click += SkipAuthTextView_Click;

            preferencesEditor = MainApp.Instance.SharedPreferences.Edit();
            isTeacher = Intent.Extras.GetBoolean(UserTypeSelectActivity.IsTeacherIntentExtraName);

            var titleTextView = FindViewById<TextView>(Resource.Id.textview_auth_edittext_title);

            if (isTeacher)
            {
                skipAuthTextView.Text = Resources.GetString(Resource.String.auth_teacher_later);
                titleTextView.Text = Resources.GetString(Resource.String.auth_teacher_title);
                autoCompleteTextViewAuth.Hint = Resources.GetString(Resource.String.auth_teacher_hint);
            }
            else
            {
                skipAuthTextView.Text = Resources.GetString(Resource.String.group_later);
                titleTextView.Text = Resources.GetString(Resource.String.edittext_title);
                autoCompleteTextViewAuth.Hint = Resources.GetString(Resource.String.edittext_auth);
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
            if (suggestionsAndIds.TryGetValue(autoCompleteTextViewAuth.Text, out var id))
            {
                if (isTeacher)
                {
                    preferencesEditor.PutString(PreferenceConstants.TeacherNamePreferenceKey,
                        autoCompleteTextViewAuth.Text).Apply();
                    preferencesEditor.PutInt(PreferenceConstants.TeacherIdPreferenceKey, id).Apply();
                }
                else
                {

                    preferencesEditor.PutString(PreferenceConstants.GroupNumberPreferenceKey,
                        autoCompleteTextViewAuth.Text).Apply();
                    preferencesEditor.PutInt(PreferenceConstants.GroupIdPreferenceKey, id).Apply();
                }
                ProceedToMainActivity();
            }
            else
            {
                autoCompleteTextViewAuth.Error = GetString(isTeacher
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
            preferencesEditor.PutBoolean(PreferenceConstants.AuthCompletedPreferenceKey, true).Apply();
            preferencesEditor.PutBoolean(PreferenceConstants.IsUserTeacherPreferenceKey, isTeacher).Apply();

            var mainIntent = new Intent(this, typeof(MainActivity));
            mainIntent.SetFlags(ActivityFlags.ClearTop);

            StartActivity(mainIntent);
            Finish();
        }

        public void AfterTextChanged(IEditable s)
        {
        }

        public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
        {
        }

        public void OnTextChanged(ICharSequence s, int start, int before, int count)
        {
            autoCompleteTextViewAuth.Clear();

            SetupTimer(s, before, count);
        }

        private void SetupTimer(ICharSequence s, int before, int count) //TODO Move to lib, catch ex
        {
            if (searchTimer != null)
            {
                searchTimer.Stop();
                //TODO Return?
            }
            else
            {
                searchTimer = new Timer(MillsToSearch);

                searchTimer.Elapsed += delegate
                {
                    if (networkChecker.IsConnected())
                    {
                        Task.Run(async () =>
                        {
                            suggestionsAndIds = isTeacher
                                ? await Utils.Utils.GetSuggestedTeachersDictionary(s.ToString())
                                : await Utils.Utils.GetSuggestedGroupsDictionary(s.ToString());

                            if (s.Length() > 0 && before != count) //TODO Local method?
                            {
                                RunOnUiThread(() =>
                                {
                                    autoCompleteTextViewAuth.UpdateSuggestions(suggestionsAndIds, this);
                                });
                            }
                        });
                    }
                    else
                    {
                        Toast.MakeText(this, GetString(Resource.String.no_connection_title), ToastLength.Short).Show();
                    }

                    searchTimer.Close();
                    searchTimer = null;
                };
            }

            searchTimer.Start();
        }
    }
}
