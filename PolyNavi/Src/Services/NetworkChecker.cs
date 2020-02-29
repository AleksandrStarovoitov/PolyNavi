using Android.Content;
using Android.Net;
using PolyNaviLib.BL;

namespace PolyNavi.Services
{

    public class NetworkChecker : INetworkChecker
    {
        private readonly Context context;

        public NetworkChecker(Context context)
        {
            this.context = context;
        }

        public bool Check()
        {
            var cm = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
            var info = cm.ActiveNetworkInfo;
            return info != null && info.IsConnected;
        }
    }
}