using Polynavi.Common.Models;
using System;
using System.Threading.Tasks;

namespace Polynavi.Common.Services
{
    public interface IScheduleService
    {
        Task<WeekSchedule> GetLatestAsync(DateTime date);
        Task<WeekSchedule> GetSavedOrLatestAsync(DateTime date);
    }
}
