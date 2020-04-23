using Polynavi.Common.Models;
using Polynavi.Common.Repositories;
using Polynavi.Common.Services;
using System;
using System.Threading.Tasks;

namespace Polynavi.Bll
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository scheduleRepository;

        public ScheduleService(IScheduleRepository scheduleRepository)
        {
            this.scheduleRepository = scheduleRepository;
        }

        public Task<WeekSchedule> GetGroupScheduleAsync(int groupId, DateTime date)
        {
            return scheduleRepository.GetGroupScheduleAsync(groupId, date);
        }

        public Task<WeekSchedule> GetTeacherScheduleAsync(int teacherId, DateTime date)
        {
            return scheduleRepository.GetTeacherScheduleAsync(teacherId, date);
        }
    }
}
