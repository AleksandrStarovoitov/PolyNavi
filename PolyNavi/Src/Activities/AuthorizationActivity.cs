using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
using PolyNaviLib.Constants;
using Timer = System.Timers.Timer;

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
        private Dictionary<string, int> groupsDictionary;
        private Timer searchTimer;
        private ISharedPreferencesEditor preferencesEditor;
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

            var buttonAuth = FindViewById<Button>(Resource.Id.button_auth);
            buttonAuth.Click += ButtonAuth_Click;

            var skipAuthTextView = FindViewById<TextView>(Resource.Id.textview_auth_skip);
            skipAuthTextView.Click += SkipAuthTextView_Click;

            preferencesEditor = MainApp.Instance.SharedPreferences.Edit();
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
            if (groupsDictionary.TryGetValue(autoCompleteTextViewAuth.Text, out var groupId))
            {
                preferencesEditor.PutString(PreferencesConstants.GroupNumberPreferenceKey, autoCompleteTextViewAuth.Text).Apply();
                preferencesEditor.PutInt(PreferencesConstants.GroupIdPreferenceKey, groupId).Apply();

                ProceedToMainActivity();
            }
            else
            {
                autoCompleteTextViewAuth.Error = GetString(Resource.String.wrong_group);
            }
        }

        private void SkipAuthTextView_Click(object sender, EventArgs e)
        {
            ProceedToMainActivity();
        }

        private void ProceedToMainActivity()
        {
            preferencesEditor.PutBoolean(PreferencesConstants.AuthCompletedPreferenceKey, true).Apply();

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
        
        private void SetupTimer(ICharSequence s, int before, int count)
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
                            groupsDictionary = await Utils.Utils.GetSuggestedGroupsDictionary(s.ToString());

                            if (s.Length() > 0 && before != count) //TODO Local method?
                            {
                                RunOnUiThread(() =>
                                {
                                    autoCompleteTextViewAuth.UpdateSuggestions(groupsDictionary, this);
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