using FluentAssertions;
using Moq;
using Polynavi.Common.Models;
using Polynavi.Common.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Polynavi.Dal.Tests
{
    public class ScheduleRepositoryTests
    {
        private SQLiteDatabase database;

        public ScheduleRepositoryTests()
        {
            database = new SQLiteDatabase("db.sqlite");

            InitDatabase();
        }

        private void InitDatabase()
        {
            CreateTables().Wait();
            database.DeleteItemsAsync<WeekSchedule>((s) => true).Wait();
        }

        private async Task CreateTables()
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

        [Fact]
        public async Task Gets_Schedule()
        {
            var schedule = CreateSchedule();
            var scheduleSettings = new Mock<IScheduleSettings>();
            
            await database.SaveItemAsync(schedule);

            var sut = await ScheduleRepository.CreateAsync(scheduleSettings.Object, database);
            var result = await sut.GetScheduleAsync(DateTime.Today);

            RemoveCyclicReferences(result);
            RemoveCyclicReferences(schedule);

            result.Should().BeEquivalentTo(schedule);
        }

        [Fact]
        public async Task Saves_Schedule()
        {
            var schedule = CreateSchedule();
            var scheduleSettings = new Mock<IScheduleSettings>();

            var sut = await ScheduleRepository.CreateAsync(scheduleSettings.Object, database);
            await sut.SaveScheduleAsync(schedule);
            var result = (await database.GetItemsAsync<WeekSchedule>()).SingleOrDefault();

            RemoveCyclicReferences(result);
            RemoveCyclicReferences(schedule);

            result.Should().BeEquivalentTo(schedule);
        }

        [Fact]
        public async Task Deletes_Schedule()
        {
            var schedule = CreateSchedule();
            var scheduleSettings = new Mock<IScheduleSettings>();

            await database.SaveItemAsync(schedule);

            var sut = await ScheduleRepository.CreateAsync(scheduleSettings.Object, database);
            await sut.DeleteWeekAsync(DateTime.Today);
            var result = await database.GetItemsAsync<WeekSchedule>();

            result.Should().BeEmpty();
        }

        private WeekSchedule CreateSchedule()
        {
            var lesson = new Lesson();
            var lessons = new List<Lesson>() { lesson };
            var day = new Day() { Lessons = lessons };
            var week = new Week()
            {
                Date_Start = DateTime.Today.AddDays(-4),
                Date_End = DateTime.Today.AddDays(3),
            };
            var schedule = new WeekSchedule()
            {
                Days = new List<Day>() { day },
                Week = week
            };
            schedule.Week.Date_Start = DateTime.Today;
            schedule.Week.Date_End = DateTime.Today;

            return schedule;
        }

        private void RemoveCyclicReferences(WeekSchedule schedule)
        {
            schedule.Week.WeekRoot = null;
            schedule.Days.ForEach(d => d.WeekRoot = null);
            schedule.Days.ForEach(d => d.Lessons.ForEach(l => l.Day = null));
            schedule.Days.ForEach(d => d.Lessons
                .ForEach(l => l.Teachers.ForEach(t => t.Lesson = null)));
            schedule.Days.ForEach(d => d.Lessons
                .ForEach(l => l.Groups.ForEach(t => t.Lesson = null)));
            schedule.Days.ForEach(d => d.Lessons
                .ForEach(l => l.Auditories.ForEach(t => t.Lesson = null)));
            schedule.Days.ForEach(d => d.Lessons.ForEach(l => l.TypeObj.Lesson = null));
            schedule.Group.WeekRoot = null;
            schedule.Group.Faculty.Group = null;
        }
    }
}
