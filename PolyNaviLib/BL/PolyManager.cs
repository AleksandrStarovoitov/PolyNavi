using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nito.AsyncEx;
using PolyNaviLib.Constants;
using PolyNaviLib.DAL;
using PolyNaviLib.SL;

namespace PolyNaviLib.BL
{
    public class PolyManager
    {
        private Repository repository;
        private static readonly HttpClient Client;
        private readonly AsyncLock mutex = new AsyncLock();

        private PolyManager()
        {
        }

        static PolyManager()
        {
            Client = new HttpClient();
        }

        private async Task<PolyManager> InitializeAsync(string dbPath, INetworkChecker checker,
            ISettingsProvider settings)
        {
            repository = await Repository.CreateAsync(dbPath, checker, settings);
            return this;
        }

        public static Task<PolyManager> CreateAsync(string dbPath, INetworkChecker checker, ISettingsProvider settings)
        {
            var manager = new PolyManager();
            return manager.InitializeAsync(dbPath, checker, settings);
        }

        public async Task<WeekRoot> GetSchedule(DateTime weekDate)
        {
            using (await mutex.LockAsync())
            {
                return await repository.GetSchedule(weekDate);
            }
        }

        public async Task<WeekRoot> GetLatestSchedule(DateTime weekDate)
        {
            using (await mutex.LockAsync())
            {
                return await repository.GetLatestSchedule(weekDate);
            }
        }

        public static async Task<GroupRoot> GetSuggestedGroups(string groupName) //TODO Non static?
        {
            var resultJson = await HttpClientService.GetResponseAsync(Client, ScheduleLinksConstants.GroupSearchLink + groupName, new CancellationToken());
            var groups = JsonConvert.DeserializeObject<GroupRoot>(resultJson);

            return groups;
        }
    }
}