using System;
using System.Threading.Tasks;
using Nito.AsyncEx;
using PolyNaviLib.DAL;

namespace PolyNaviLib.BL
{
    public class PolyManager
    {
        private Repository repository;
        private readonly AsyncLock mutex = new AsyncLock();
        
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

        public async Task ReinitializeDatabaseAsync()
        {
            await repository.ReinitializeDatabaseAsync();
        }

        public Task<GroupRoot> GetSuggestedGroupsAsync(string groupName)
        {
            return repository.GetSuggestedGroupsAsync(groupName);
        }

        public Task<TeachersRoot> GetSuggestedTeachersAsync(string teacherName)
        {
            return repository.GetSuggestedTeachersAsync(teacherName);
        }
    }
}
