using Newtonsoft.Json;
using Polynavi.Common.Constants;
using Polynavi.Common.Exceptions;
using Polynavi.Common.Models;
using Polynavi.Common.Services;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Polynavi.Bll.Services
{
    public class ScheduleDownloader : IScheduleDownloader
    {
        private readonly INetworkChecker networkChecker;
        private readonly ISettingsStorage settingsStorage;
        private readonly IHttpClientService httpClientService;

        public ScheduleDownloader(INetworkChecker networkChecker, ISettingsStorage settingsStorage,
            IHttpClientService httpClientService)
        {
            this.networkChecker = networkChecker;
            this.settingsStorage = settingsStorage;
            this.httpClientService = httpClientService;
        }

        public async Task<WeekSchedule> GetScheduleFromWebAsync(DateTime date)
        {
            if (!networkChecker.IsConnected())
            {
                throw new NetworkException("No internet connection"); //TODO Constants?
            }

            var requestUrl = GetLink(date);

            var result = await httpClientService.GetResponseAsStringAsync(requestUrl, new CancellationToken());

            var weekSchedule = JsonConvert.DeserializeObject<WeekSchedule>(result);
            weekSchedule.LastUpdated = DateTime.Now;

            return weekSchedule;
        }

        private string GetLink(DateTime date)
        {
            var isTeacher = settingsStorage.GetBoolean(PreferenceConstants.IsUserTeacherPreferenceKey, false);
            var dateStr = date.ToString("yyyy-M-d", new CultureInfo("ru-RU"));

            if (isTeacher)
            {
                var teacherId = settingsStorage.GetInt(PreferenceConstants.TeacherIdPreferenceKey, 0); //TODO Not found exception
                return ScheduleLinkConstants.TeacherScheduleLink + teacherId + "/scheduler" + "?&date=" + dateStr;
            }

            var groupId = settingsStorage.GetInt(PreferenceConstants.GroupIdPreferenceKey, 0); //TODO Not found exception
            return ScheduleLinkConstants.ScheduleLink + groupId + "?&date=" + dateStr;
        }
    }
}
