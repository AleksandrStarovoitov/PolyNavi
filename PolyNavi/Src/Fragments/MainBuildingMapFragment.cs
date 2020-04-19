using Android.OS;
using Android.Views;
using AndroidX.Fragment.App;
using PolyNavi.Views;

namespace PolyNavi.Fragments
{
    public class MainBuildingMapFragment : Fragment
    {
        public MainBuildingView MapView { get; private set; }
        private readonly int drawableId;

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
