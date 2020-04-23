using Polynavi.Common.Models;
using Polynavi.Common.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Polynavi.Dal
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly SQLiteDatabase database;

        public ScheduleRepository(SQLiteDatabase database)
        {
            this.database = database;
        }

        public async Task<WeekSchedule> GetScheduleAsync(DateTime date)
        {
            var weekRoots = await database.GetItemsAsync<WeekSchedule>();
            return weekRoots.SingleOrDefault(w => w.Week.ContainsDate(date));
        }

        public Task RecreateDatabase()
        {
            throw new NotImplementedException();
        }

        public Task RemoveExpiredWeeks()
        {
            throw new NotImplementedException();
        }

        public async Task SaveScheduleAsync(WeekSchedule weekSchedule)
        {
            await database.SaveItemAsync(weekSchedule);
        }
    }
}
