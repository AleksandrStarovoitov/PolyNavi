using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using SQLite;
using SQLiteNetExtensionsAsync.Extensions;

using PolyNaviLib.BL;

namespace PolyNaviLib.DL
{
	public class SQLiteDatabase
	{
		SQLiteAsyncConnection db;

		public SQLiteDatabase(string dbPath)
		{
			db = new SQLiteAsyncConnection(dbPath);
		}
		
		public async Task CreateTableAsync<T>() where T : IBusinessEntity, new()
		{
			try
			{
				await db.CreateTableAsync<T>();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
				throw;                                   //TODO убрать когда закончим
			}
		}

		public async Task<List<T>> GetItemsAsync<T>() where T : IBusinessEntity, new()
		{
			return await db.GetAllWithChildrenAsync<T>(recursive: true);
			//return await db.Table<T>().ToListAsync();
		}

		public async Task<T> GetItemAsync<T>(int id) where T : IBusinessEntity, new()
		{
			return await db.GetWithChildrenAsync<T>(id);
			//return await db.GetAsync<T>(id);
		}

		public async Task/*<int>*/ SaveItemAsync<T>(T item) where T : IBusinessEntity, new()
		{
			if (item.ID == 0)
			{
				await db.InsertWithChildrenAsync(item, recursive: true);
				//return await db.InsertAsync(item);
			}
			else
			{
				await db.UpdateWithChildrenAsync(item);
				//return await db.UpdateAsync(item);
			}
		}

		public async Task/*<int>*/ DeleteItemAsync<T>(T item) where T : IBusinessEntity, new()
		{
			await db.DeleteAsync(item, recursive: true);
			//return await db.DeleteAsync(item);
		}
	}
}
