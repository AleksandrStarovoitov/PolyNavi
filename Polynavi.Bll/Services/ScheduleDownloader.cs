using Newtonsoft.Json;
using Polynavi.Common.Constants;
using Polynavi.Common.Exceptions;
using Polynavi.Common.Models;
using Polynavi.Common.Services;
using Polynavi.Common.Settings;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Polynavi.Bll.Services
{
    public class ScheduleDownloader : IScheduleDownloader
    {
        private readonly INetworkChecker networkChecker;
        private readonly IScheduleSettings scheduleSettings;
        private readonly IHttpClientService httpClientService;

        public ScheduleDownloader(INetworkChecker networkChecker, IScheduleSettings scheduleSettings,
            IHttpClientService httpClientService)
        {
            this.networkChecker = networkChecker;
            this.scheduleSettings = scheduleSettings;
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
            var dateStr = date.ToString("yyyy-M-d", new CultureInfo("ru-RU"));

            if (scheduleSettings.IsUserTeacher)
            {
                return ScheduleLinkConstants.TeacherScheduleLink + 
                    scheduleSettings.TeacherId + "/scheduler" + "?&date=" + dateStr;
            }
            
            return ScheduleLinkConstants.ScheduleLink + 
                scheduleSettings.GroupId + "?&date=" + dateStr;
        }
    }
}
