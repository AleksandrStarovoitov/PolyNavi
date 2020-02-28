using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Java.Lang;
using Newtonsoft.Json;
using PolyNavi.Services;
using PolyNaviLib.BL;
using PolyNaviLib.SL;

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
        private ArrayAdapter suggestAdapter;
        private Dictionary<string, int> groupsDictionary;
        private string[] array;
        private Timer searchTimer;
        private const int millsToSearch = 700;
        private const string groupSearchLink =
            "http://m.spbstu.ru/p/proxy.php?csurl=http://ruz.spbstu.ru/api/v1/ruz/search/groups&q=";

        private ISharedPreferencesEditor prefEditor;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            MainApp.ChangeLanguage(this);
            SetTheme(Resource.Style.MyAppTheme);
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_authorization);

            networkChecker = new NetworkChecker(this);
            autoCompleteTextViewAuth = FindViewById<AutoCompleteTextView>(Resource.Id.autocompletetextview_auth);
            autoCompleteTextViewAuth.SetOnEditorActionListener(this);
            autoCompleteTextViewAuth.AddTextChangedListener(this);

            var buttonAuth = FindViewById<Button>(Resource.Id.button_auth);
            var textViewLater = FindViewById<TextView>(Resource.Id.textview_later_auth);

            buttonAuth.Click += ButtonAuth_Click;
            textViewLater.Click += TextViewLater_Click;

            prefEditor = MainApp.Instance.SharedPreferences.Edit();
        }

        private void ButtonAuth_Click(object sender, EventArgs e)
        {
            CheckGroupNumberAndProceed();
        }

        public bool OnEditorAction(TextView v, [GeneratedEnum] ImeAction actionId, KeyEvent e)
        {
            if (actionId == ImeAction.Go)
            {
                CheckGroupNumberAndProceed();
            }

            return false;
        }

        private void CheckGroupNumberAndProceed()
        {

            if (groupsDictionary.TryGetValue(autoCompleteTextViewAuth.Text, out int groupId))
            {
                prefEditor.PutString("groupnumber", autoCompleteTextViewAuth.Text).Apply();
                prefEditor.PutInt("groupid", groupId).Apply();

                ProceedToMainActivity();
            }
            else
            {
                autoCompleteTextViewAuth.Error = GetString(Resource.String.wrong_group);
            }
        }

        private void TextViewLater_Click(object sender, EventArgs e)
        {
            ProceedToMainActivity();
        }

        public void ProceedToMainActivity()
        {
            prefEditor.PutBoolean("auth", true).Apply();
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
            autoCompleteTextViewAuth.Adapter = null;
            autoCompleteTextViewAuth.DismissDropDown();
            if (autoCompleteTextViewAuth.Error != null && s.ToString().Length != 0)
            {
                autoCompleteTextViewAuth.Error = null;
            }

            if (searchTimer != null)
            {
                searchTimer.Stop();
            }
            else
            {
                searchTimer = new Timer(millsToSearch);
                searchTimer.Elapsed += delegate
                {
                    if (networkChecker.Check())
                    {
                        var client = new HttpClient();
                        Task.Run(async () =>
                        {
                            var resultJson = await HttpClientSL.GetResponseAsync(client,
                                groupSearchLink +
                                s.ToString(), new System.Threading.CancellationToken());
                            var groups = JsonConvert.DeserializeObject<GroupRoot>(resultJson);
                            groupsDictionary = groups.Groups.ToDictionary(x => x.Name, x => x.Id);
                            array = groupsDictionary.Select(x => x.Key).ToArray();
                            RunOnUiThread(() =>
                            {
                                suggestAdapter = new ArrayAdapter(this,
                                    Android.Resource.Layout.SimpleDropDownItem1Line,
                                    array);
                                autoCompleteTextViewAuth.Adapter = null;
                                autoCompleteTextViewAuth.Adapter = suggestAdapter;
                                if (s.Length() > 0 && before != count)
                                {
                                    autoCompleteTextViewAuth.ShowDropDown();
                                }
                            });
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