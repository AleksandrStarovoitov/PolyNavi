using Polynavi.Common.Constants;
using Polynavi.Common.Models;
using Polynavi.Common.Repositories;
using Polynavi.Common.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Polynavi.Dal
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly ISettingsStorage settingsStorage;
        private SQLiteDatabase database;

        public ScheduleRepository(ISettingsStorage settingsStorage)
        {
            this.settingsStorage = settingsStorage;
        }

        public static Task<ScheduleRepository> CreateAsync(ISettingsStorage settingsStorage, 
            SQLiteDatabase database)
        {
            var repository = new ScheduleRepository(settingsStorage);

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

        public Task RecreateDatabase()
        {
            throw new NotImplementedException();
        }

        public async Task RemoveExpiredWeeks()
        {
            var isTeacher = settingsStorage.GetBoolean(PreferenceConstants.IsUserTeacherPreferenceKey, false);

            if (isTeacher)
            {
                var currentTeacherId = settingsStorage.GetInt(PreferenceConstants.TeacherIdPreferenceKey, 0); //TODO Not found exception?

                await database.DeleteItemsAsync<WeekSchedule>(w =>
                    w.Week.IsExpired() ||
                    w.Days.Any(d => d.Lessons.Any(l => l.Teachers.Any(t => t.Id != currentTeacherId)))); //TODO
            }
            else
            {
                var currentGroupId = settingsStorage.GetInt(PreferenceConstants.GroupIdPreferenceKey, 0); //TODO Not found exception?

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
