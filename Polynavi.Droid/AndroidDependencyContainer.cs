using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Android.App;
using AndroidX.Preference;
using Graph;
using Polynavi.Bll;
using Polynavi.Bll.Settings;
using Polynavi.Common.Repositories;
using Polynavi.Common.Services;
using Polynavi.Common.Settings;
using Polynavi.Dal;
using Polynavi.Droid.Services;
using Xamarin.Android.Net;

namespace Polynavi.Droid
{
    internal sealed class AndroidDependencyContainer : BllDependencyContainer
    {
        private readonly Lazy<SettingsStorage> settingsStorage;

        public static AndroidDependencyContainer Instance { get; private set; }

        public static void EnsureInitialized()
        {
            if (Instance != null)
                return;

            Instance = new AndroidDependencyContainer();
        }

        public AndroidDependencyContainer()
        {
            settingsStorage = new Lazy<SettingsStorage>(() => 
                new SettingsStorage(KeyValueStorage));
        }

        protected override HttpClient CreateHttpClient()
        {
            var httpHandler = new AndroidClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            return new HttpClient(httpHandler, true);
        }

        protected override INetworkChecker CreateNetworkChecker() =>
            new NetworkChecker(Application.Context);

        protected override async Task<IScheduleRepository> CreateScheduleRepository() =>
            await Dal.ScheduleRepository.CreateAsync(ScheduleSettings, 
                new SQLiteDatabase(MainApp.GetFileFullPath(MainApp.DatabaseFilename))); //TODO

        protected override IAssetsProvider CreateAssetsProvider() =>
            new AssetsProvider(Application.Context);

        protected override IGraphService CreateGraphService()
            => new GraphService(AssetsProvider);

        protected override IKeyValueStorage CreateKeyValueStorage()
            => new SharedPreferencesStorage(PreferenceManager.GetDefaultSharedPreferences(Application.Context));

        protected override IScheduleSettings CreateScheduleSettings()
            => settingsStorage.Value;

        protected override ILoginStateSettings CreateLoginStateSettings()
            => settingsStorage.Value;

        protected override IAppInfoSettings CreateAppInfoSettings()
            => settingsStorage.Value;
    }
}