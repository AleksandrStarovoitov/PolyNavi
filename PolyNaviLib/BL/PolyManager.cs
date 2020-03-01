using System;
using System.Threading.Tasks;
using Nito.AsyncEx;
using PolyNaviLib.DAL;

namespace PolyNaviLib.BL
{

    public class PolyManager
    {
        private Repository repository;

        private PolyManager()
        {
        }

        private async Task<PolyManager> InitializeAsync(string dbPath, INetworkChecker checker, ISettingsProvider settings)
        {
            repository = await Repository.CreateAsync(dbPath, checker, settings);
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
    }
}
