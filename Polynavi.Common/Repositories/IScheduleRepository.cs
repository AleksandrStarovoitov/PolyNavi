using Polynavi.Common.Models;
using System;
using System.Threading.Tasks;

namespace Polynavi.Common.Repositories
{
    public interface IScheduleRepository
    {
        Task RemoveExpiredWeeks();
        Task RecreateDatabase();

        Task<WeekSchedule> GetScheduleAsync(DateTime date);
        Task SaveScheduleAsync(WeekSchedule weekSchedule);
    }
}
