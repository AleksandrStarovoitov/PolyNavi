using Polynavi.Common.Models;
using Polynavi.Common.Repositories;
using Polynavi.Common.Settings;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Polynavi.Dal
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly IScheduleSettings scheduleSettings;
        private SQLiteDatabase database;

        public ScheduleRepository(IScheduleSettings scheduleSettings)
        {
            this.scheduleSettings = scheduleSettings;
        }

        public static Task<ScheduleRepository> CreateAsync(IScheduleSettings scheduleSettings, 
            SQLiteDatabase database)
        {
            var repository = new ScheduleRepository(scheduleSettings);

            return repository.InitializeAsync(database);
        }

        private async Task<ScheduleRepository> InitializeAsync(SQLiteDatabase database)
        {
            this.database = database;

            await CreateTablesAsync();
            await RemoveExpiredWeeks();

            return this;
        }

        public async Task DeleteWeekAsync(DateTime date)
        {
            await database.DeleteItemsAsync<WeekSchedule>(w => w.Week.ContainsDate(date));
        }

        public async Task<WeekSchedule> GetScheduleAsync(DateTime date)
        {
            var weekRoots = await database.GetItemsAsync<WeekSchedule>();
            return weekRoots.SingleOrDefault(w => w.Week.ContainsDate(date));
        }

        public async Task RemoveExpiredWeeks()
        {
            if (scheduleSettings.IsUserTeacher)
            {
                var currentTeacherId = scheduleSettings.TeacherId; //TODO Not found exception?

                await database.DeleteItemsAsync<WeekSchedule>(w =>
                    w.Week.IsExpired() ||
                    w.Days.Any(d => d.Lessons.Any(l => l.Teachers.Any(t => t.Id != currentTeacherId)))); //TODO
            }
            else
            {
                var currentGroupId = scheduleSettings.GroupId; //TODO Not found exception?

                await database.DeleteItemsAsync<WeekSchedule>(w =>
                    w.Week.IsExpired() || w.Group.Id != currentGroupId);
            }
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
