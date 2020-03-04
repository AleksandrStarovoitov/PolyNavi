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
using PolyNaviLib.Exceptions;
using PolyNaviLib.SL;

namespace PolyNaviLib.DAL
{
    public class Repository
    {
        private SQLiteDatabase database;
        private INetworkChecker checker;
        private ISettingsProvider settings;
        private HttpClient client;

        private Repository()
        {
        }

        private async Task<Repository> InitializeAsync(string dbPath, INetworkChecker checker,
            ISettingsProvider settings)
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

        public static Task<Repository> CreateAsync(string dbPath, INetworkChecker networkChecker,
            ISettingsProvider settings)
        {
            var repo = new Repository();
            return repo.InitializeAsync(dbPath, networkChecker, settings);
        }

        public async Task<WeekRoot> GetSchedule(DateTime weekDate)
        {
            var weekRoots = await database.GetItemsAsync<WeekRoot>();
            var weekRoot =  weekRoots.SingleOrDefault(w => w.Week.ContainsDate(weekDate));

            if (weekRoot == null)
            {
                var newWeek = await LoadWeekRootFromWebAsync(weekDate);
                await database.SaveItemAsync(newWeek);
                return newWeek;
            }

            return weekRoot;
        }

        public async Task<WeekRoot> GetLatestSchedule(DateTime weekDate) //TODO "Get" with delete... Rename? 
        {
            var newWeek = await LoadWeekRootFromWebAsync(weekDate);

            await database.DeleteItemsAsync<WeekRoot>(w => w.Week.ContainsDate(weekDate));
            await database.SaveItemAsync(newWeek);

            return newWeek;
        }

        private async Task<WeekRoot> LoadWeekRootFromWebAsync(DateTime weekDate)
        {
            if (!checker.IsConnected())
            {
                throw new NetworkException("No internet connection"); //TODO
            }

            var dateStr = weekDate.ToString("yyyy-M-d", new CultureInfo("ru-RU"));
            var groupId = settings[PreferencesConstants.GroupIdPreferenceKey];

            var resultJson = await HttpClientService.GetResponseAsync(client,
                ScheduleLinksConstants.ScheduleLink + groupId + "?&date=" + dateStr, new CancellationToken()); //TODO uri

            var weekRoot = JsonConvert.DeserializeObject<WeekRoot>(resultJson);
            weekRoot.LastUpdated = DateTime.Now;

            return weekRoot;
        }

        private async Task RemoveExpiredWeeksAsync()
        {
            var currentGroupId = Convert.ToInt32(settings[PreferencesConstants.GroupIdPreferenceKey]);

            await database.DeleteItemsAsync<WeekRoot>(w =>
                w.Week.IsExpired() || w.Group.Id != currentGroupId);
        }
    }
}