using Nito.AsyncEx;
using Polynavi.Common.Models;
using Polynavi.Common.Repositories;
using Polynavi.Common.Services;
using System;
using System.Threading.Tasks;

namespace Polynavi.Bll.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository scheduleRepository;
        private readonly IScheduleDownloader scheduleDownloader;
        private readonly AsyncLock mutex = new AsyncLock();

        public ScheduleService(IScheduleRepository scheduleRepository, IScheduleDownloader scheduleDownloader)
        {
            this.scheduleRepository = scheduleRepository;
            this.scheduleDownloader = scheduleDownloader;
        }

        public async Task<WeekSchedule> GetLatestAsync(DateTime date)
        {
            using (await mutex.LockAsync())
            {
                await scheduleRepository.DeleteWeekAsync(date);

                return await DownloadAndSaveAsync(date);
            }
        }

        public async Task<WeekSchedule> GetSavedOrLatestAsync(DateTime date)
        {
            using (await mutex.LockAsync())
            {
                var schedule = await scheduleRepository.GetScheduleAsync(date);

                if (schedule == null)
                {
                    return await DownloadAndSaveAsync(date);
                }

                return schedule;
            }
        }

        private async Task<WeekSchedule> DownloadAndSaveAsync(DateTime date)
        {
            var scheduleFromWeb = await scheduleDownloader.GetScheduleFromWebAsync(date);
            await scheduleRepository.SaveScheduleAsync(scheduleFromWeb);

            return scheduleFromWeb;
        }
    }
}
