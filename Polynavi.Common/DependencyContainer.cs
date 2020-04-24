using Nito.AsyncEx;
using Polynavi.Common.Repositories;
using Polynavi.Common.Services;
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
        private readonly Lazy<ISettingsStorage> settingsStorage;
        private readonly Lazy<IHttpClientService> httpClientService;
        private readonly Lazy<HttpClient> httpClient;

        public IScheduleService ScheduleService => scheduleService.Value;
        public ISuggestionsService SuggestionsService => suggestionsService.Value;
        public IScheduleDownloader ScheduleDownloader => scheduleDownloader.Value;
        public Task<IScheduleRepository> ScheduleRepository => scheduleRepository.Task;
        public INetworkChecker NetworkChecker => networkChecker.Value;
        public ISettingsStorage SettingsStorage => settingsStorage.Value;
        public IHttpClientService HttpClientService => httpClientService.Value;
        public HttpClient HttpClient => httpClient.Value;

        protected DependencyContainer()
        {
            scheduleService = new Lazy<IScheduleService>(CreateScheduleService);
            suggestionsService = new Lazy<ISuggestionsService>(CreateSuggestionsService);
            scheduleDownloader = new Lazy<IScheduleDownloader>(CreateScheduleDownloader);
            scheduleRepository = new AsyncLazy<IScheduleRepository>(CreateScheduleRepository);
            networkChecker = new Lazy<INetworkChecker>(CreateNetworkChecker);
            settingsStorage = new Lazy<ISettingsStorage>(CreateSettingsStorage);
            httpClientService = new Lazy<IHttpClientService>(CreateHttpClientService);
            httpClient = new Lazy<HttpClient>(CreateHttpClient);
        }

        protected abstract IScheduleService CreateScheduleService();
        protected abstract ISuggestionsService CreateSuggestionsService();
        protected abstract IScheduleDownloader CreateScheduleDownloader();
        protected abstract Task<IScheduleRepository> CreateScheduleRepository();
        protected abstract INetworkChecker CreateNetworkChecker();
        protected abstract ISettingsStorage CreateSettingsStorage();
        protected abstract IHttpClientService CreateHttpClientService();
        protected abstract HttpClient CreateHttpClient();
    }
}
