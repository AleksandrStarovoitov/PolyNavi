using Newtonsoft.Json;
using Polynavi.Common.Constants;
using Polynavi.Common.Exceptions;
using Polynavi.Common.Models;
using Polynavi.Common.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Polynavi.Bll.Services
{
    public class SuggestionsService : ISuggestionsService
    {
        private readonly INetworkChecker networkChecker;
        private readonly IHttpClientService httpClientService;

        public SuggestionsService(INetworkChecker networkChecker, IHttpClientService httpClientService)
        {
            this.networkChecker = networkChecker;
            this.httpClientService = httpClientService;
        }

        public async Task<GroupRoot> GetSuggestedGroupsAsync(string groupName) //TODO Non static?
        {
            if (!networkChecker.IsConnected()) //TODO Move to httpclient service?
            {
                throw new NetworkException();
            }

            var requestUrl = ScheduleLinkConstants.GroupSearchLink + groupName;
            var resultJson = await httpClientService.GetResponseAsStringAsync(requestUrl, new CancellationToken());

            return JsonConvert.DeserializeObject<GroupRoot>(resultJson);
        }

        public async Task<TeachersRoot> GetSuggestedTeachersAsync(string teacherName)
        {
            if (!networkChecker.IsConnected()) //TODO Move to httpclient service?
            {
                throw new NetworkException();
            }

            var requestUrl = ScheduleLinkConstants.TeacherSearchLink + teacherName;

            var resultJson = await httpClientService.GetResponseAsStringAsync(requestUrl, new CancellationToken());

            return JsonConvert.DeserializeObject<TeachersRoot>(resultJson);
        }
    }
}
