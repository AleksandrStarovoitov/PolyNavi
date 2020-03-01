using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PolyNaviLib.BL;
using PolyNaviLib.Constants;
using PolyNaviLib.DL;
using PolyNaviLib.SL;

namespace PolyNaviLib.DAL
{
    public class Repository
    {
        private const string scheduleLink = @"http://m.spbstu.ru/p/proxy.php?csurl=http://ruz.spbstu.ru/api/v1/ruz/scheduler/";

        private SQLiteDatabase database;
        private INetworkChecker checker;
        private ISettingsProvider settings;
        private HttpClient client;

        private Repository()
        {
        }

        private async Task<Repository> InitializeAsync(string dbPath, INetworkChecker checker, ISettingsProvider settings)
        {
            database = new SQLiteDatabase(dbPath);

            await database.CreateTableAsync<WeekRoot>();
            await database.CreateTableAsync<Week>();
            await database.CreateTableAsync<Day>();
            await database.CreateTableAsync<Lesson>();
            await database.CreateTableAsync<TypeObj>();
            await database.CreateTableAsync<Group>();
            await database.CreateTableAsync<Faculty>();
            await database.CreateTableAsync<Teacher>();
            await database.CreateTableAsync<Auditory>();
            await database.CreateTableAsync<Building>();

            this.checker = checker;
            this.settings = settings;
            client = new HttpClient();

            await RemoveExpiredWeeksAsync();
            return this;
        }

        public static Task<Repository> CreateAsync(string dbPath, INetworkChecker networkChecker, ISettingsProvider settings)
        {
            var repo = new Repository();
            return repo.InitializeAsync(dbPath, networkChecker, settings);
        }

        public async Task<WeekRoot> GetWeekRootAsync(DateTime weekDate, bool forceUpdate)
        {
            if (await database.IsEmptyAsync<WeekRoot>())
            {
                var weekRoot = await LoadWeekRootFromWebAsync(weekDate);
                await database.SaveItemAsync(weekRoot);
                return weekRoot;
            }

            var weekFromDb = (await database.GetItemsAsync<WeekRoot>()).SingleOrDefault(w => w.Week.DateEqual(weekDate));
            if (weekFromDb == null)
            {
                var newWeek = (await LoadWeekRootFromWebAsync(weekDate));
                await database.SaveItemAsync(newWeek);
                return newWeek;
            }

            if (forceUpdate)
            {
                var newWeek = (await LoadWeekRootFromWebAsync(weekDate));
                await database.DeleteItemsAsync<WeekRoot>(w => w.Week.DateEqual(weekDate));
                await database.SaveItemAsync(newWeek);
                return newWeek;
            }

            return weekFromDb;
        }

        private async Task<WeekRoot> LoadWeekRootFromWebAsync(DateTime weekDate)
        {
            if (checker.IsConnected() == false)
            {
                throw new NetworkException("No internet connection"); //TODO
            }

            var groupId = settings[PreferencesConstants.GroupIdPreferenceKey];

            var dateStr = weekDate.ToString("yyyy-M-d", new CultureInfo("ru-RU"));
            var resultJson = await HttpClientService.GetResponseAsync(client, scheduleLink + groupId + "&date=" + dateStr, new CancellationToken());
            var weekRoot = JsonConvert.DeserializeObject<WeekRoot>(resultJson);
            weekRoot.LastUpdated = DateTime.Now;

            return weekRoot;
        }

        private async Task RemoveExpiredWeeksAsync()
        {
            await database.DeleteItemsAsync<WeekRoot>(w => w.Week.IsExpired(Convert.ToInt32(settings[PreferencesConstants.GroupIdPreferenceKey]))); //TODO
        }
    }
}
