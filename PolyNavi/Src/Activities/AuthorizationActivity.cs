using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

namespace PolyNavi
{
	[Activity(
		Label = "PolyNavi",
		ScreenOrientation = ScreenOrientation.Portrait,
		ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
	public class AuthorizationActivity : AppCompatActivity, TextView.IOnEditorActionListener
	{
        AutoCompleteTextView autoCompleteTextViewAuth;
        NetworkChecker networkChecker;
        ArrayAdapter suggestAdapter;
        ProgressBar progressBar;
        string[] array;
        ISharedPreferencesEditor prefEditor;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			MainApp.ChangeLanguage(this);
			SetTheme(Resource.Style.MyAppTheme);
			base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_authorization);

            networkChecker = new NetworkChecker(this);
            autoCompleteTextViewAuth = FindViewById<AutoCompleteTextView>(Resource.Id.autocompletetextview_auth);
            autoCompleteTextViewAuth.SetOnEditorActionListener(this);
            progressBar = FindViewById<ProgressBar>(Resource.Id.progressbar_auth);

            progressBar.Visibility = ViewStates.Visible;
            Task.Run(async () =>
            {
                array = (await MainApp.Instance.GroupsDictionary).Select(x => x.Key).ToArray();
                
                RunOnUiThread(() =>
                {
                    suggestAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, array);

                    autoCompleteTextViewAuth.Adapter = suggestAdapter;

                    progressBar.Visibility = ViewStates.Invisible;
                });
            });

            var buttonAuth = FindViewById<Button>(Resource.Id.button_auth);
			var textViewLater = FindViewById<TextView>(Resource.Id.textview_later_auth);
            var textViewUpdate = FindViewById<TextView>(Resource.Id.textview_groupsupdate_auth);

			buttonAuth.Click += ButtonAuth_Click;
			textViewLater.Click += TextViewLater_Click;
            textViewUpdate.Click += TextViewUpdate_Click;

			prefEditor = MainApp.Instance.SharedPreferences.Edit();
		}

        private void TextViewUpdate_Click(object sender, EventArgs e)
        {
            if (networkChecker.Check())
            {
                progressBar.Visibility = ViewStates.Visible;
                Task.Run(async () =>
                {

                    MainApp.Instance.GroupsDictionary = new Nito.AsyncEx.AsyncLazy<Dictionary<string, int>>(async () => { return await MainApp.FillGroupsDictionary(true); });
                    array = (await MainApp.Instance.GroupsDictionary).Select(x => x.Key).ToArray();

                    RunOnUiThread(() =>
                    {
                        suggestAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, array);

                        autoCompleteTextViewAuth.Adapter = null;
                        autoCompleteTextViewAuth.Adapter = suggestAdapter;

                        progressBar.Visibility = ViewStates.Invisible;
                    });
                });
            }
            else
            {
                Toast.MakeText(this, "No internet", ToastLength.Short).Show();
            }
        }

        private void ButtonAuth_Click(object sender, EventArgs e)
		{
			if (!autoCompleteTextViewAuth.Text.Equals(""))
			{ 	
				ProceedToMainActivity();
			}
		}

		public bool OnEditorAction(TextView v, [GeneratedEnum] ImeAction actionId, KeyEvent e)
		{
			if (actionId == ImeAction.Go)
			{
				if (!autoCompleteTextViewAuth.Text.Equals(""))
				{
					ProceedToMainActivity();
				}
			}
			return false;
		}

		private void TextViewLater_Click(object sender, EventArgs e)
		{
			ProceedToMainActivity();
		}

		public void ProceedToMainActivity()
		{
            prefEditor.PutBoolean("auth", true).Apply();
            if (!autoCompleteTextViewAuth.Text.Equals(""))
            {
                var d = MainApp.Instance.GroupsDictionary.Task.Result;
                prefEditor.PutString("groupnumber", autoCompleteTextViewAuth.Text).Apply();
                prefEditor.PutInt("groupid", d[autoCompleteTextViewAuth.Text]).Apply();
            }
            var mainIntent = new Intent(this, typeof(MainActivity));
			mainIntent.SetFlags(ActivityFlags.ClearTop);
			StartActivity(mainIntent);
			Finish();
		}
	}
}