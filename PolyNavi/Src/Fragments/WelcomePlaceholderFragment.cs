using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace PolyNavi
{
    public class WelcomePlaceholderFragment : Android.Support.V4.App.Fragment
    {
        private static readonly String ARG_SECTION_NUMBER = "section_number";

        ImageView img;
        readonly int[] bgs = new int[] { Resource.Drawable.welcome_blueprint, Resource.Drawable.welcome_route, Resource.Drawable.welcome_calendar };

        public WelcomePlaceholderFragment()
        {
        }

        public static WelcomePlaceholderFragment NewInstance(int sectionNumber)
        {
            var fragment = new WelcomePlaceholderFragment();
            var args = new Bundle();
            args.PutInt(ARG_SECTION_NUMBER, sectionNumber);
            fragment.Arguments = args;
            return fragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var rootView = inflater.Inflate(Resource.Layout.fragment_welcome_pager, container, false);
            var header = rootView.FindViewById<TextView>(Resource.Id.textview_welcome_header);
            var description = rootView.FindViewById<TextView>(Resource.Id.textview_welcome_description);

            var pos = Arguments.GetInt(ARG_SECTION_NUMBER);
            string headerText = null, descriptionText = null;

            switch (pos)
            {
                case 1:
                    headerText = GetString(Resource.String.welcome_header_first);
                    descriptionText = GetString(Resource.String.welcome_description_first);
                    break;
                case 2:
                    headerText = GetString(Resource.String.welcome_header_second);
                    descriptionText = GetString(Resource.String.welcome_description_second);
                    break;
                case 3:
                    headerText = GetString(Resource.String.welcome_header_third);
                    descriptionText = GetString(Resource.String.welcome_description_third);
                    break;
            }

            if (headerText != null)
            {
                header.Text = headerText;
            }
            if (descriptionText != null)
            {
                description.Text = descriptionText;
            }

            img = rootView.FindViewById<ImageView>(Resource.Id.imageview_welcome_featureimage);
            img.SetBackgroundResource(bgs[Arguments.GetInt(ARG_SECTION_NUMBER) - 1]);

            return rootView;
        }
    }
}