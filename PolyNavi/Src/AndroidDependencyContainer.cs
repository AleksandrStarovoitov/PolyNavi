using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Polynavi.Bll;
using Polynavi.Common;
using Polynavi.Common.Repositories;
using Polynavi.Common.Services;
using Polynavi.Dal;
using PolyNavi.Services;
using Xamarin.Android.Net;

namespace PolyNavi.Src
{
    internal sealed class AndroidDependencyContainer : BllDependencyContainer
    {
        public static AndroidDependencyContainer Instance { get; private set; }
        private static Context context;

        public static void EnsureInitialized(Context c)
        {
            if (Instance != null)
                return;

            Instance = new AndroidDependencyContainer();
            context = c;
        }

        protected override HttpClient CreateHttpClient()
        {
            var httpHandler = new AndroidClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            return new HttpClient(httpHandler, true);
        }

        protected override INetworkChecker CreateNetworkChecker()
        {
            return new NetworkChecker(context);
        }

        protected override IScheduleRepository CreateScheduleRepository()
        {
            return new ScheduleRepository(new SQLiteDatabase("")); //TODO
        }

        protected override ISettingsProvider CreateSettingsProvider()
        {
            throw new NotImplementedException();
        }
    }
}