using Polynavi.Common.Models;
using Polynavi.Common.Repositories;
using System;
using System.Threading.Tasks;

namespace Polynavi.Dal
{
    public class ScheduleRepository : IScheduleRepository
    {
        private SQLiteDatabase database;

        public ScheduleRepository(SQLiteDatabase database)
        {
            this.database = database;
        }

        public Task<WeekSchedule> GetGroupScheduleAsync(int groupId, DateTime date)
        {
            throw new NotImplementedException();
        }

        public Task<WeekSchedule> GetTeacherScheduleAsync(int techerId, DateTime date)
        {
            throw new NotImplementedException();
        }

        public Task RecreateDatabase()
        {
            throw new NotImplementedException();
        }

        public Task RemoveExpiredWeeks()
        {
            throw new NotImplementedException();
        }
    }
}
