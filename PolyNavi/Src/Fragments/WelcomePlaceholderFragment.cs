using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;

namespace PolyNavi.Fragments
{
    public class WelcomePlaceholderFragment : Fragment
    {
        private const string ArgSectionNumber = "section_number";

        private readonly int[] backgroundDrawableIds =
        {
            Resource.Drawable.welcome_blueprint, 
            Resource.Drawable.welcome_route,
            Resource.Drawable.welcome_calendar
        };

        private WelcomePlaceholderFragment()
        {
        }

        public static WelcomePlaceholderFragment NewInstance(int sectionNumber)
        {
            var fragment = new WelcomePlaceholderFragment();

            var args = new Bundle();
            args.PutInt(ArgSectionNumber, sectionNumber);

            fragment.Arguments = args;
            return fragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var rootView = inflater.Inflate(Resource.Layout.fragment_welcome_pager, container, false);
            var header = rootView.FindViewById<TextView>(Resource.Id.textview_welcome_header);
            var description = rootView.FindViewById<TextView>(Resource.Id.textview_welcome_description);

            var pos = Arguments.GetInt(ArgSectionNumber);
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

            var image = rootView.FindViewById<ImageView>(Resource.Id.imageview_welcome_featureimage);
            image.SetBackgroundResource(backgroundDrawableIds[Arguments.GetInt(ArgSectionNumber) - 1]);

            return rootView;
        }
    }
}
