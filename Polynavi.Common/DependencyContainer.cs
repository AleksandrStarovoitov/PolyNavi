using Graph;
using Nito.AsyncEx;
using Polynavi.Common.Repositories;
using Polynavi.Common.Services;
using Polynavi.Common.Settings;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Polynavi.Common
{
    public abstract class DependencyContainer
    {
        private readonly Lazy<IScheduleService> scheduleService;
        private readonly Lazy<ISuggestionsService> suggestionsService;
        private readonly Lazy<IScheduleDownloader> scheduleDownloader;
        private readonly AsyncLazy<IScheduleRepository> scheduleRepository;
        private readonly Lazy<INetworkChecker> networkChecker;
        private readonly Lazy<IKeyValueStorage> keyValueStorage;
        private readonly Lazy<IScheduleSettings> scheduleSettings;
        private readonly Lazy<ILoginStateSettings> loginStateSettings;
        private readonly Lazy<IAppInfoSettings> appInfoSettings;
        private readonly Lazy<IHttpClientService> httpClientService;
        private readonly Lazy<IAssetsProvider> assetsProvider;
        private readonly Lazy<IGraphService> graphService;
        private readonly Lazy<HttpClient> httpClient;

        public IScheduleService ScheduleService => scheduleService.Value;
        public ISuggestionsService SuggestionsService => suggestionsService.Value;
        public IScheduleDownloader ScheduleDownloader => scheduleDownloader.Value;
        public Task<IScheduleRepository> ScheduleRepository => scheduleRepository.Task;
        public INetworkChecker NetworkChecker => networkChecker.Value;
        public IKeyValueStorage KeyValueStorage => keyValueStorage.Value;
        public IScheduleSettings ScheduleSettings => scheduleSettings.Value;
        public ILoginStateSettings LoginStateSettings => loginStateSettings.Value;
        public IAppInfoSettings AppInfoSettings => appInfoSettings.Value;
        public IHttpClientService HttpClientService => httpClientService.Value;
        public IAssetsProvider AssetsProvider => assetsProvider.Value;
        public IGraphService GraphService => graphService.Value;
        public HttpClient HttpClient => httpClient.Value;

        protected DependencyContainer()
        {
            scheduleService = new Lazy<IScheduleService>(CreateScheduleService);
            suggestionsService = new Lazy<ISuggestionsService>(CreateSuggestionsService);
            scheduleDownloader = new Lazy<IScheduleDownloader>(CreateScheduleDownloader);
            scheduleRepository = new AsyncLazy<IScheduleRepository>(CreateScheduleRepository);
            networkChecker = new Lazy<INetworkChecker>(CreateNetworkChecker);
            keyValueStorage = new Lazy<IKeyValueStorage>(CreateKeyValueStorage);
            scheduleSettings = new Lazy<IScheduleSettings>(CreateScheduleSettings);
            loginStateSettings = new Lazy<ILoginStateSettings>(CreateLoginStateSettings);
            appInfoSettings = new Lazy<IAppInfoSettings>(CreateAppInfoSettings);
            httpClientService = new Lazy<IHttpClientService>(CreateHttpClientService);
            assetsProvider = new Lazy<IAssetsProvider>(CreateAssetsProvider);
            graphService = new Lazy<IGraphService>(CreateGraphService);
            httpClient = new Lazy<HttpClient>(CreateHttpClient);
        }

        protected abstract IScheduleService CreateScheduleService();
        protected abstract ISuggestionsService CreateSuggestionsService();
        protected abstract IScheduleDownloader CreateScheduleDownloader();
        protected abstract Task<IScheduleRepository> CreateScheduleRepository();
        protected abstract INetworkChecker CreateNetworkChecker();
        protected abstract IKeyValueStorage CreateKeyValueStorage();
        protected abstract IScheduleSettings CreateScheduleSettings();
        protected abstract ILoginStateSettings CreateLoginStateSettings();
        protected abstract IAppInfoSettings CreateAppInfoSettings();
        protected abstract IHttpClientService CreateHttpClientService();
        protected abstract IAssetsProvider CreateAssetsProvider();
        protected abstract IGraphService CreateGraphService();
        protected abstract HttpClient CreateHttpClient();
    }
}
