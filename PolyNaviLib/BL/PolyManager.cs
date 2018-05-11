﻿using System;
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

		private async Task<PolyManager> InitializeAsync(string dbPath, INetworkChecker checker)
		{
			repository = await Repository.CreateAsync(dbPath, checker);
			return this;
		}

		public static Task<PolyManager> CreateAsync(string dbPath, INetworkChecker checker)
		{
			var manager = new PolyManager();
			return manager.InitializeAsync(dbPath, checker);
		}

		Nito.AsyncEx.AsyncLock mutex = new Nito.AsyncEx.AsyncLock();
		//Получить расписание на неделю
		public async Task<Week> GetWeekAsync(DateTime weekDate, string groupNumber)
		{
			using (await mutex.LockAsync())
			{
				return await repository.GetWeekAsync(weekDate, groupNumber);
			}
		}

	}
}
