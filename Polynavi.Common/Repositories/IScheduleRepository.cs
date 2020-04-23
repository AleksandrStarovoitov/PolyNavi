using Polynavi.Common.Models;
using System;
using System.Threading.Tasks;

namespace Polynavi.Common.Repositories
{
    public interface IScheduleRepository
    {
        Task<WeekSchedule> GetGroupScheduleAsync(int groupId, DateTime date);
        Task<WeekSchedule> GetTeacherScheduleAsync(int techerId, DateTime date);
        Task RemoveExpiredWeeks();
        Task RecreateDatabase();
    }
}
