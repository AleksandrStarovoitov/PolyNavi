using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
		EditText editTextAuth;
		ISharedPreferencesEditor prefEditor;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			MainApp.ChangeLanguage(this);
			SetTheme(Resource.Style.MyAppTheme);
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.activity_authorization);

			editTextAuth = FindViewById<EditText>(Resource.Id.edittext_auth);
			editTextAuth.SetOnEditorActionListener(this);

			var buttonAuth = FindViewById<Button>(Resource.Id.button_auth);
			var textViewLater = FindViewById<TextView>(Resource.Id.textview_later_auth);

			buttonAuth.Click += ButtonAuth_Click;
			textViewLater.Click += TextViewLater_Click;

			prefEditor = MainApp.Instance.SharedPreferences.Edit();
		}

		private void ButtonAuth_Click(object sender, EventArgs e)
		{
			if (!editTextAuth.Text.Equals(""))
			{
				prefEditor.PutString("groupnumber", editTextAuth.Text).Apply();
				prefEditor.PutBoolean("auth", true).Apply();
				ProceedToMainActivity();
			}
		}

		public bool OnEditorAction(TextView v, [GeneratedEnum] ImeAction actionId, KeyEvent e)
		{
			if (actionId == ImeAction.Go)
			{
				if (!editTextAuth.Text.Equals(""))
				{
					prefEditor.PutString("groupnumber", editTextAuth.Text).Apply();
					prefEditor.PutBoolean("auth", true).Apply();
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
			var mainIntent = new Intent(this, typeof(MainActivity));
			mainIntent.SetFlags(ActivityFlags.ClearTop);
			StartActivity(mainIntent);
			Finish();
		}
	}
}