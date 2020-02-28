using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using PolyNavi.Activities;

namespace PolyNavi.Fragments
{
    [Activity(Label = "AboutFragment")]
	public class AboutFragment : Android.Support.V4.App.Fragment
	{
        private View view;

	    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			view = inflater.Inflate(Resource.Layout.fragment_about, container, false);

			var frameMail = view.FindViewById<FrameLayout>(Resource.Id.framelayout_email_about);
			var frameRate = view.FindViewById<FrameLayout>(Resource.Id.framelayout_rate_about);
			var frameGithub = view.FindViewById<FrameLayout>(Resource.Id.framelayout_github_about);
			var frameCopyright = view.FindViewById<FrameLayout>(Resource.Id.framelayout_copyright_about);

			frameMail.Click += FrameMail_Click;
			frameRate.Click += FrameRate_Click;
            frameGithub.Click += FrameGithub_Click;
			frameCopyright.Click += FrameCopyright_Click;

			var sashaLink = view.FindViewById<TextView>(Resource.Id.textview_contacts_sasha_link_about);
			var kirillLink = view.FindViewById<TextView>(Resource.Id.textview_contacts_kirill_link_about);

			sashaLink.MovementMethod = LinkMovementMethod.Instance;
			kirillLink.MovementMethod = LinkMovementMethod.Instance;

			return view;
		}

		private void FrameMail_Click(object sender, EventArgs e)
		{
            var emailIntent = new Intent(Intent.ActionSend);
			emailIntent.SetType("message/rfc822");
			emailIntent.PutExtra(Intent.ExtraEmail, new string[] { GetString(Resource.String.about_email_address) });
			StartActivity(Intent.CreateChooser(emailIntent, GetString(Resource.String.email_send_intent)));
		}

		private void FrameRate_Click(object sender, EventArgs e)
		{
			var rateIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(GetString(Resource.String.about_rate_link)));
			StartActivity(rateIntent);
		}

		private void FrameGithub_Click(object sender, EventArgs e)
		{
			var githubIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(GetString(Resource.String.about_github_link)));
			StartActivity(githubIntent);
		}

		private void FrameCopyright_Click(object sender, EventArgs e)
		{
			var copyrightIntent = new Intent(Activity.BaseContext, typeof(CopyrightActivity));
			StartActivity(copyrightIntent);
		}
	}
}