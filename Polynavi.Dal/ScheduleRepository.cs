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
        private readonly ISettingsProvider settingsProvider;
        private SQLiteDatabase database;

        public ScheduleRepository(ISettingsProvider settingsProvider)
        {
            this.settingsProvider = settingsProvider;
        }

        public static Task<ScheduleRepository> CreateAsync(ISettingsProvider settingsProvider, 
            SQLiteDatabase database)
        {
            var repository = new ScheduleRepository(settingsProvider);

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
            var isTeacher = settingsProvider[PreferenceConstants.IsUserTeacherPreferenceKey];

            if (isTeacher.Equals(true))
            {
                var currentTeacherId = Convert.ToInt32(settingsProvider[PreferenceConstants.TeacherIdPreferenceKey]);

                await database.DeleteItemsAsync<WeekSchedule>(w =>
                    w.Week.IsExpired() ||
                    w.Days.Any(d => d.Lessons.Any(l => l.Teachers.Any(t => t.Id != currentTeacherId)))); //TODO
            }
            else
            {
                var currentGroupId = Convert.ToInt32(settingsProvider[PreferenceConstants.GroupIdPreferenceKey]);

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
