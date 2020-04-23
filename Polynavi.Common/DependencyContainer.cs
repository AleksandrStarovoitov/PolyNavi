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
        private readonly Lazy<IScheduleDownloader> scheduleDownloader;
        private readonly AsyncLazy<IScheduleRepository> scheduleRepository;
        private readonly Lazy<INetworkChecker> networkChecker;
        private readonly Lazy<ISettingsProvider> settingsProvider;
        private readonly Lazy<HttpClient> httpClient;

        public IScheduleService ScheduleService => scheduleService.Value;
        public IScheduleDownloader ScheduleDownloader => scheduleDownloader.Value;
        public Task<IScheduleRepository> ScheduleRepository => scheduleRepository.Task;
        public INetworkChecker NetworkChecker => networkChecker.Value;
        public ISettingsProvider SettingsProvider => settingsProvider.Value;
        public HttpClient HttpClient => httpClient.Value;

        protected DependencyContainer()
        {
            scheduleService = new Lazy<IScheduleService>(CreateScheduleService);
            scheduleDownloader = new Lazy<IScheduleDownloader>(CreateScheduleDownloader);
            scheduleRepository = new AsyncLazy<IScheduleRepository>(CreateScheduleRepository);
            networkChecker = new Lazy<INetworkChecker>(CreateNetworkChecker);
            settingsProvider = new Lazy<ISettingsProvider>(CreateSettingsProvider);
            httpClient = new Lazy<HttpClient>(CreateHttpClient);
        }

        protected abstract IScheduleService CreateScheduleService();
        protected abstract IScheduleDownloader CreateScheduleDownloader();
        protected abstract Task<IScheduleRepository> CreateScheduleRepository();
        protected abstract INetworkChecker CreateNetworkChecker();
        protected abstract ISettingsProvider CreateSettingsProvider();

        protected virtual HttpClient CreateHttpClient() => new HttpClient();
    }
}
