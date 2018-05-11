using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

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
			await db.CreateTableAsync<T>();
		}

		public async Task DropTableAsync<T>() where T : IBusinessEntity, new()
		{
			await db.DropTableAsync<T>();
		}

		public async Task<int> CountAsync<T>() where T : IBusinessEntity, new()
		{
			return await db.Table<T>().CountAsync();
		}

		public async Task<bool> IsEmptyAsync<T>() where T :IBusinessEntity, new()
		{
			int count = await CountAsync<T>();
			return count == 0;
		}

		public async Task<List<T>> GetItemsAsync<T>() where T : IBusinessEntity, new()
		{
			return await db.GetAllWithChildrenAsync<T>(recursive: true);
			//return await db.Table<T>().ToListAsync();
		}

		public async Task<List<T>> GetOrderedItemsAsync<T, TKey>(Func<T, TKey> keySelector) where T : IBusinessEntity, new()
		{
			try
			{
				var list = await GetItemsAsync<T>();
				return list.OrderBy(keySelector).ToList();
			}
			catch (Exception ex)
			{
				int a = 0;
				throw;
			}
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
