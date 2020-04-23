using Polynavi.Bll.Services;
using Polynavi.Common;
using Polynavi.Common.Services;

namespace Polynavi.Bll
{
    public abstract class BllDependencyContainer : DependencyContainer
    {
        protected override IScheduleDownloader CreateScheduleDownloader()
        {
            return new ScheduleDownloader(NetworkChecker, SettingsProvider);
        }

        protected override IScheduleService CreateScheduleService()
        {
            return new ScheduleService(ScheduleRepository, ScheduleDownloader);
        }
    }
}
