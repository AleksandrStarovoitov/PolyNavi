using Polynavi.Common.Models;
using System;
using System.Threading.Tasks;

namespace Polynavi.Common.Services
{
    public interface IScheduleDownloader
    {
        Task<WeekSchedule> GetScheduleFromWebAsync(DateTime date);
    }
}
