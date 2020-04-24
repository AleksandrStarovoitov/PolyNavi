using Android.Content;
using Android.Net;
using Polynavi.Common.Services;

namespace Polynavi.Droid.Services
{
    public class NetworkChecker : INetworkChecker
    {
        private readonly Context context;

        public NetworkChecker(Context context)
        {
            this.context = context;
        }

        public bool IsConnected()
        {
            var cm = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
            var info = cm.ActiveNetworkInfo;
            return info != null && info.IsConnected;
        }
    }
}
