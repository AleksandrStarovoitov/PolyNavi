using AutoFixture;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using Polynavi.Bll.Services;
using Polynavi.Common.Exceptions;
using Polynavi.Common.Models;
using Polynavi.Common.Services;
using Polynavi.Common.Settings;
using Polynavi.Tests.Common;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Polynavi.Bll.Tests
{
    public class ScheduleDownloaderTests
    {
        private readonly Fixture fixture;

        public ScheduleDownloaderTests()
        {
            fixture = FixtureInitializer.InitializeFixture();
        }

        [Fact]
        public async Task GetScheduleFromWebAsync_Throws_NetworkException_If_No_Connection()
        {
            var networkChecker = new Mock<INetworkChecker>();
            var scheduleSettings = new Mock<IScheduleSettings>();
            var httpClientService = new Mock<IHttpClientService>();
            networkChecker.Setup(nc => nc.IsConnected()).Returns(false);

            var sut = new ScheduleDownloader(networkChecker.Object, scheduleSettings.Object,
                httpClientService.Object);
            Func<Task> act = async () => { await sut.GetScheduleFromWebAsync(DateTime.Now); };

            await act.Should().ThrowAsync<NetworkException>();
        }

        [Fact]
        public async Task Gets_Schedule_From_Web()
        {
            var date = DateTime.Now;
            var schedule = fixture.Create<WeekSchedule>();
            var scheduleJson = JsonConvert.SerializeObject(schedule);
            var networkChecker = new Mock<INetworkChecker>();
            var scheduleSettings = new Mock<IScheduleSettings>();
            var httpClientService = new Mock<IHttpClientService>();
            networkChecker.Setup(nc => nc.IsConnected())
                .Returns(true);
            httpClientService.Setup(hs => hs.GetResponseAsStringAsync(It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                    .ReturnsAsync(scheduleJson);
            scheduleSettings.Setup(ss => ss.GroupId).Returns(1);
            
            var sut = new ScheduleDownloader(networkChecker.Object, scheduleSettings.Object,
                httpClientService.Object);
            var result = await sut.GetScheduleFromWebAsync(date);

            result.Should().BeEquivalentTo(schedule, opt => 
                opt.Excluding(s => s.LastUpdated));
        }
    }
}
