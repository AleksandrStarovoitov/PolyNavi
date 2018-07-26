using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PolyNaviLib.DAL;

namespace PolyNaviLib.BL
{

	public class PolyManager
	{
		Repository repository;

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

        Nito.AsyncEx.AsyncLock mutex = new Nito.AsyncEx.AsyncLock();

        public async Task<WeekRoot> GetWeekRootAsync(DateTime weekDate)
        {
            using (await mutex.LockAsync())
            {
                return await repository.GetWeekRootAsync(weekDate);
            }
        }

    }
}
