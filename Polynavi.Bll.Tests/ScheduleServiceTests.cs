using AutoFixture;
using FluentAssertions;
using Moq;
using Polynavi.Bll.Services;
using Polynavi.Common.Models;
using Polynavi.Common.Repositories;
using Polynavi.Common.Services;
using Polynavi.Tests.Common;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Polynavi.Bll.Tests
{
    public class ScheduleServiceTests
    {
        private readonly Fixture fixture;

        public ScheduleServiceTests()
        {
            fixture = FixtureInitializer.InitializeFixture();
        }

        [Fact]
        public async Task Gets_Latest_Schedule()
        {
            var date = DateTime.Now;
            var schedule = fixture.Create<WeekSchedule>();
            var scheduleRepository = new Mock<IScheduleRepository>();
            var scheduleDownloader = new Mock<IScheduleDownloader>();
            scheduleDownloader.Setup(sd => sd.GetScheduleFromWebAsync(date))
                .ReturnsAsync(schedule);

            var sut = new ScheduleService(scheduleRepository.Object, scheduleDownloader.Object);
            var result = await sut.GetLatestAsync(date);

            scheduleRepository.Verify(sr => sr.DeleteWeekAsync(date), Times.Once);
            scheduleRepository.Verify(sr => sr.SaveScheduleAsync(schedule), Times.Once);
            result.Should().Be(schedule);
        }

        [Fact]
        public async Task Gets_Saved_Schedule()
        {
            var date = DateTime.Now;
            var schedule = fixture.Create<WeekSchedule>();
            var scheduleRepository = new Mock<IScheduleRepository>();
            var scheduleDownloader = new Mock<IScheduleDownloader>();
            scheduleRepository.Setup(sr => sr.GetScheduleAsync(date))
                .ReturnsAsync(schedule);

            var sut = new ScheduleService(scheduleRepository.Object, scheduleDownloader.Object);
            var result = await sut.GetSavedOrLatestAsync(date);

            scheduleRepository.Verify(sr => sr.GetScheduleAsync(date), Times.Once);
            scheduleDownloader.Verify(sd => sd.GetScheduleFromWebAsync(date), Times.Never);
            result.Should().Be(schedule);
        }

        [Fact]
        public async Task Gets_Schedule_From_Web_If_No_Saved()
        {
            var date = DateTime.Now;
            var schedule = fixture.Create<WeekSchedule>();
            var scheduleRepository = new Mock<IScheduleRepository>();
            var scheduleDownloader = new Mock<IScheduleDownloader>();
            scheduleDownloader.Setup(sd => sd.GetScheduleFromWebAsync(date))
                .ReturnsAsync(schedule);
            scheduleRepository.Setup(sr => sr.GetScheduleAsync(date))
                .ReturnsAsync((WeekSchedule)null);

            var sut = new ScheduleService(scheduleRepository.Object, scheduleDownloader.Object);
            var result = await sut.GetSavedOrLatestAsync(date);

            scheduleRepository.Verify(sr => sr.GetScheduleAsync(date), Times.Once);
            scheduleDownloader.Verify(sd => sd.GetScheduleFromWebAsync(date), Times.Once);
            scheduleRepository.Verify(sr => sr.SaveScheduleAsync(schedule), Times.Once);
            result.Should().Be(schedule);
        }
    }
}
