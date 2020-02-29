using Android.OS;
using Android.Views;
using PolyNavi.Views;

namespace PolyNavi.Fragments
{
    public class MainBuildingMapFragment : Android.Support.V4.App.Fragment
    {
        public MainBuildingView MapView { get; private set; }
        private readonly int drawableId;

        public MainBuildingMapFragment()
        {

        }

        public MainBuildingMapFragment(int id)
        {
            drawableId = id;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            MapView = new MainBuildingView(Activity.BaseContext, drawableId);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return MapView;
        }
    }
}