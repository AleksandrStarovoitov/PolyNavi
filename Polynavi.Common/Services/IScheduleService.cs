using Polynavi.Common.Models;
using System;
using System.Threading.Tasks;

namespace Polynavi.Common.Services
{
    public interface IScheduleService
    {
        Task<WeekSchedule> GetGroupScheduleAsync(int groupId, DateTime date);
        Task<WeekSchedule> GetTeacherScheduleAsync(int teacherId, DateTime date);
    }
}
