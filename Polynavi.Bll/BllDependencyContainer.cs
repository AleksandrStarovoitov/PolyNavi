using Polynavi.Bll.Services;
using Polynavi.Common;
using Polynavi.Common.Services;

namespace Polynavi.Bll
{
    public abstract class BllDependencyContainer : DependencyContainer
    {
        protected override IScheduleDownloader CreateScheduleDownloader() =>
            new ScheduleDownloader(NetworkChecker, SettingsStorage, HttpClientService);

        protected override IScheduleService CreateScheduleService() =>
            new ScheduleService(ScheduleRepository.Result, ScheduleDownloader); //TODO Async

        protected override IHttpClientService CreateHttpClientService() =>
            new HttpClientService(HttpClient);

        protected override ISuggestionsService CreateSuggestionsService() =>
            new SuggestionsService(NetworkChecker, HttpClientService);
    }
}
