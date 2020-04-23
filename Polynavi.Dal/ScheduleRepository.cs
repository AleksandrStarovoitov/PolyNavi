using Polynavi.Common.Models;
using Polynavi.Common.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Polynavi.Dal
{
    public class ScheduleRepository : IScheduleRepository
    {
        private SQLiteDatabase database;

        public ScheduleRepository()
        {
        }

        public static Task<ScheduleRepository> CreateAsync(SQLiteDatabase database)
        {
            var repository = new ScheduleRepository();

            return repository.InitializeAsync(database);
        }

        private async Task<ScheduleRepository> InitializeAsync(SQLiteDatabase database)
        {
            this.database = database;

            await CreateTablesAsync();

            return this;
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

        private async Task CreateTablesAsync()
        {
            await database.CreateTableAsync<WeekSchedule>();
            await database.CreateTableAsync<Week>();
            await database.CreateTableAsync<Day>();
            await database.CreateTableAsync<Lesson>();
            await database.CreateTableAsync<TypeObj>();
            await database.CreateTableAsync<Group>();
            await database.CreateTableAsync<Faculty>();
            await database.CreateTableAsync<Teacher>();
            await database.CreateTableAsync<Auditory>();
            await database.CreateTableAsync<Building>();
        }
    }
}
