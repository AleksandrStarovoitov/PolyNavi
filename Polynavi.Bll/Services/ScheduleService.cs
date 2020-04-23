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

        public ScheduleService(IScheduleRepository scheduleRepository, IScheduleDownloader scheduleDownloader)
        {
            this.scheduleRepository = scheduleRepository;
            this.scheduleDownloader = scheduleDownloader;
        }

        public async Task<WeekSchedule> GetScheduleAsync(DateTime date)
        {
            var schedule = await scheduleRepository.GetScheduleAsync(date);

            if (schedule == null)
            {
                var scheduleFromWeb = await scheduleDownloader.GetScheduleFromWebAsync(date);

                await scheduleRepository.SaveScheduleAsync(scheduleFromWeb);

                return scheduleFromWeb;
            }

            return schedule;
        }
    }
}
