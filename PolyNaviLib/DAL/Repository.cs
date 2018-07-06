using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PolyNaviLib.BL;
using PolyNaviLib.SL;
using PolyNaviLib.DL;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;

namespace PolyNaviLib.DAL
{
    public class Repository
    {
        readonly string scheduleLink = @"http://m.spbstu.ru/p/proxy.php?csurl=http://ruz.spbstu.ru/api/v1/ruz/scheduler/";

        SQLiteDatabase database;
        INetworkChecker checker;
        ISettingsProvider settings;

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
                        
            await RemoveExpiredWeeksAsync();
            return this;
        }

        public static Task<Repository> CreateAsync(string dbPath, INetworkChecker networkChecker, ISettingsProvider settings)
        {
            var repo = new Repository();
            return repo.InitializeAsync(dbPath, networkChecker, settings);
        }

        public async Task<WeekRoot> GetWeekRootAsync(DateTime weekDate)
        {
            if (await database.IsEmptyAsync<WeekRoot>())
            {
                var weekRoot = await LoadWeekRootFromWebAsync(weekDate);
                await database.SaveItemAsync(weekRoot);
                return weekRoot;
            }
            else
            {
                var weekFromDb = (await database.GetItemsAsync<WeekRoot>()).Where(w => w.Week.DateEqual(weekDate)).SingleOrDefault();
                if (weekFromDb == null)
                {
                    var newWeek = (await LoadWeekRootFromWebAsync(weekDate));
                    await database.SaveItemAsync(newWeek);
                    return newWeek;
                }
                else
                {
                    return weekFromDb;
                }
            }
        }

        public async Task<WeekRoot> LoadWeekRootFromWebAsync(DateTime weekDate)
        {
            if (checker.Check() == false)
            {
                throw new NetworkException("No internet connection");
            }

            var client = new HttpClient();

            var groupId = settings["groupid"];

            string dateStr = weekDate.ToString("yyyy-M-d", new CultureInfo("ru-RU"));
            var resultJson = await HttpClientSL.GetResponseAsync(client, scheduleLink + groupId + "&date=" + dateStr);
            var weekRoot = JsonConvert.DeserializeObject<WeekRoot>(resultJson);
            return weekRoot;
        }
        
        private async Task RemoveExpiredWeeksAsync()
        {
            await database.DeleteItemsAsync<WeekRoot>(w => w.Week.IsExpired(Convert.ToInt32(settings["groupid"])));
        }
    }
}
