using Polynavi.Common.Models;
using System;
using System.Threading.Tasks;

namespace Polynavi.Common.Repositories
{
    public interface IScheduleRepository
    {
        Task DeleteWeekAsync(DateTime date);
        Task<WeekSchedule> GetScheduleAsync(DateTime date);
        Task SaveScheduleAsync(WeekSchedule weekSchedule);
        Task RecreateDatabase();
        Task RemoveExpiredWeeks();
    }
}
