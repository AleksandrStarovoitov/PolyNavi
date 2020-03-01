using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nito.AsyncEx;
using PolyNaviLib.DAL;
using PolyNaviLib.SL;

namespace PolyNaviLib.BL
{

    public class PolyManager
    {
        private Repository repository;
        private static HttpClient client;

        private const string GroupSearchLink =
            "http://m.spbstu.ru/p/proxy.php?csurl=http://ruz.spbstu.ru/api/v1/ruz/search/groups&q=";

        private PolyManager()
        {
        }

        private async Task<PolyManager> InitializeAsync(string dbPath, INetworkChecker checker,
            ISettingsProvider settings)
        {
            repository = await Repository.CreateAsync(dbPath, checker, settings);
            client = new HttpClient(); //TODO
            return this;
        }

        public static Task<PolyManager> CreateAsync(string dbPath, INetworkChecker checker, ISettingsProvider settings)
        {
            var manager = new PolyManager();
            return manager.InitializeAsync(dbPath, checker, settings);
        }

        private readonly AsyncLock mutex = new AsyncLock();

        public async Task<WeekRoot> GetWeekRootAsync(DateTime weekDate, bool forceUpdate)
        {
            using (await mutex.LockAsync())
            {
                return await repository.GetWeekRootAsync(weekDate, forceUpdate);
            }
        }

        public static async Task<GroupRoot> GetSuggestedGroups(string groupName)
        {
            var resultJson = await HttpClientService.GetResponseAsync(client, GroupSearchLink + groupName, new CancellationToken());
            var groups = JsonConvert.DeserializeObject<GroupRoot>(resultJson);

            return groups;
        }
    }
}
