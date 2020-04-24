using Newtonsoft.Json;
using Polynavi.Common.Constants;
using Polynavi.Common.Exceptions;
using Polynavi.Common.Models;
using Polynavi.Common.Services;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<Dictionary<string, int>> GetSuggestedGroupsAsync(string groupName) //TODO Non static?
        {
            if (!networkChecker.IsConnected()) //TODO Move to httpclient service?
            {
                throw new NetworkException();
            }

            var requestUrl = ScheduleLinkConstants.GroupSearchLink + groupName;
            var resultJson = await httpClientService.GetResponseAsStringAsync(requestUrl, new CancellationToken());
            var groups = JsonConvert.DeserializeObject<GroupRoot>(resultJson);

            return groups.Groups.ToDictionary(x => x.Name, x => x.Id);
        }

        public async Task<Dictionary<string, int>> GetSuggestedTeachersAsync(string teacherName)
        {
            if (!networkChecker.IsConnected()) //TODO Move to httpclient service?
            {
                throw new NetworkException();
            }

            var requestUrl = ScheduleLinkConstants.TeacherSearchLink + teacherName;
            var resultJson = await httpClientService.GetResponseAsStringAsync(requestUrl, new CancellationToken());
            var teachers = JsonConvert.DeserializeObject<TeachersRoot>(resultJson);

            return teachers.Teachers.ToDictionary(t => t.Full_Name, t => t.Id);
        }
    }
}
