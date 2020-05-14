using AutoFixture;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using Polynavi.Bll.Services;
using Polynavi.Common.Exceptions;
using Polynavi.Common.Models;
using Polynavi.Common.Services;
using Polynavi.Tests.Common;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Polynavi.Bll.Tests
{
    public class SuggestionServiceTests
    {
        private readonly Fixture fixture;

        public SuggestionServiceTests()
        {
            fixture = FixtureInitializer.InitializeFixture();
        }

        [Fact]
        public async Task GetSuggestedGroupsAsync_Throws_NetworkException_If_Not_Connected()
        {
            var httpClientService = new Mock<IHttpClientService>();
            var networkChecker = new Mock<INetworkChecker>();
            networkChecker.Setup(nc => nc.IsConnected())
                          .Returns(false);

            var sut = new SuggestionsService(networkChecker.Object, httpClientService.Object);
            Func<Task> act = () => sut.GetSuggestedGroupsAsync("");

            await act.Should().ThrowAsync<NetworkException>();
        }

        [Fact]
        public async Task GetSuggestedTeachersAsync_Throws_NetworkException_If_Not_Connected()
        {
            var httpClientService = new Mock<IHttpClientService>();
            var networkChecker = new Mock<INetworkChecker>();
            networkChecker.Setup(nc => nc.IsConnected())
                .Returns(false);

            var sut = new SuggestionsService(networkChecker.Object, httpClientService.Object);
            Func<Task> act = () => sut.GetSuggestedTeachersAsync("");

            await act.Should().ThrowAsync<NetworkException>();
        }

        [Fact]
        public async Task Gets_Groups_Suggestions() 
        {
            var date = DateTime.Now;
            var groupRoot = fixture.Create<GroupRoot>();
            var groupRootJson = JsonConvert.SerializeObject(groupRoot);

            var networkChecker = new Mock<INetworkChecker>();
            var httpClientService = new Mock<IHttpClientService>();
            networkChecker.Setup(nc => nc.IsConnected())
                .Returns(true);
            httpClientService.Setup(hs => hs.GetResponseAsStringAsync(It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                    .ReturnsAsync(groupRootJson);

            var sut = new SuggestionsService(networkChecker.Object, httpClientService.Object);
            var result = await sut.GetSuggestedGroupsAsync("");

            var expected = groupRoot.Groups.ToDictionary(x => x.Name, x => x.Id);
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Gets_Teachers_Suggestions()
        {
            var date = DateTime.Now;
            var teachersRoot = fixture.Create<TeachersRoot>();
            var teachersRootJson = JsonConvert.SerializeObject(teachersRoot);

            var networkChecker = new Mock<INetworkChecker>();
            var httpClientService = new Mock<IHttpClientService>();
            networkChecker.Setup(nc => nc.IsConnected())
                .Returns(true);
            httpClientService.Setup(hs => hs.GetResponseAsStringAsync(It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                    .ReturnsAsync(teachersRootJson);

            var sut = new SuggestionsService(networkChecker.Object, httpClientService.Object);
            var result = await sut.GetSuggestedTeachersAsync("");

            var expected = teachersRoot.Teachers.ToDictionary(x => x.Full_Name, x => x.Id);
            result.Should().BeEquivalentTo(expected);
        }
    }
}