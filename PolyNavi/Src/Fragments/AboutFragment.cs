using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using PolyNavi.Activities;
using Fragment = AndroidX.Fragment.App.Fragment;
using Uri = Android.Net.Uri;

namespace PolyNavi.Fragments
{
    [Activity(Label = "AboutFragment")]
    public class AboutFragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_about, container, false);

            var mailFrame = view.FindViewById<FrameLayout>(Resource.Id.framelayout_email_about);
            var rateFrame = view.FindViewById<FrameLayout>(Resource.Id.framelayout_rate_about);
            var githubFrame = view.FindViewById<FrameLayout>(Resource.Id.framelayout_github_about);
            var copyrightFrame = view.FindViewById<FrameLayout>(Resource.Id.framelayout_copyright_about);

            mailFrame.Click += MailFrame_Click;
            rateFrame.Click += RateFrame_Click;
            githubFrame.Click += GithubFrame_Click;
            copyrightFrame.Click += CopyrightFrame_Click;

            return view;
        }

        private void MailFrame_Click(object sender, EventArgs e)
        {
            var emailIntent = new Intent(Intent.ActionSend);
            emailIntent.SetType("message/rfc822");
            emailIntent.PutExtra(Intent.ExtraEmail, new[] { GetString(Resource.String.about_email_address) });

            StartActivity(Intent.CreateChooser(emailIntent, GetString(Resource.String.email_send_intent)));
        }

        private void RateFrame_Click(object sender, EventArgs e)
        {
            var rateIntent = new Intent(Intent.ActionView, Uri.Parse(GetString(Resource.String.about_rate_link)));

            StartActivity(rateIntent);
        }

        private void GithubFrame_Click(object sender, EventArgs e)
        {
            var githubIntent = new Intent(Intent.ActionView, Uri.Parse(GetString(Resource.String.about_github_link)));

            StartActivity(githubIntent);
        }

        private void CopyrightFrame_Click(object sender, EventArgs e)
        {
            var copyrightIntent = new Intent(Activity.BaseContext, typeof(CopyrightActivity));

            StartActivity(copyrightIntent);
        }
    }
}
