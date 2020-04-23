﻿using Newtonsoft.Json;
using Polynavi.Common.Constants;
using Polynavi.Common.Exceptions;
using Polynavi.Common.Models;
using Polynavi.Common.Services;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Polynavi.Bll.Services
{
    public class ScheduleDownloader : IScheduleDownloader
    {
        private readonly INetworkChecker networkChecker;
        private readonly ISettingsProvider settingsProvider;
        private readonly HttpClient httpClient;

        public ScheduleDownloader(INetworkChecker networkChecker, ISettingsProvider settingsProvider)
        {
            this.networkChecker = networkChecker;
            this.settingsProvider = settingsProvider;
            this.httpClient = new HttpClient();
        }

        public async Task<WeekSchedule> GetScheduleFromWebAsync(DateTime date)
        {
            if (!networkChecker.IsConnected())
            {
                throw new NetworkException("No internet connection"); //TODO Constants?
            }

            var requestUrl = GetLink(date);

            var result = await GetResponseAsync(requestUrl, new CancellationToken());

            var weekSchedule = JsonConvert.DeserializeObject<WeekSchedule>(result);
            weekSchedule.LastUpdated = DateTime.Now;

            return weekSchedule;
        }

        private string GetLink(DateTime date)
        {
            var isTeacher = settingsProvider[PreferenceConstants.IsUserTeacherPreferenceKey];
            var dateStr = date.ToString("yyyy-M-d", new CultureInfo("ru-RU"));

            if (isTeacher.Equals(true)) //TODO GetBoolean
            {
                var teacherId = settingsProvider[PreferenceConstants.TeacherIdPreferenceKey];
                return ScheduleLinkConstants.TeacherScheduleLink + teacherId + "/scheduler" + "?&date=" + dateStr;
            }

            var groupId = settingsProvider[PreferenceConstants.GroupIdPreferenceKey];
            return ScheduleLinkConstants.ScheduleLink + groupId + "?&date=" + dateStr;
        }

        private async Task<string> GetResponseAsync(string uri, CancellationToken cts)
        {
            try // Move try catch?
            {
                using (var response = await httpClient.GetAsync(uri, cts))
                {
                    response.EnsureSuccessStatusCode();

                    var responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
            }
            catch (HttpRequestException)
            {
                //TODO Log, throw?
                return null;
            }
        }
    }
}