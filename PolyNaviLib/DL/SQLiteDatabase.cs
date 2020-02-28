using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SQLite;
using SQLiteNetExtensionsAsync.Extensions;

using PolyNaviLib.BL;
using SQLiteNetExtensions.Extensions;

namespace PolyNaviLib.DL
{
    public class SQLiteDatabase
	{
        private SQLiteAsyncConnection db;

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
			var count = await CountAsync<T>();
			return count == 0;
		}

		public async Task<List<T>> GetItemsAsync<T>() where T : IBusinessEntity, new()
		{
			return await db.GetAllWithChildrenAsync<T>(recursive: true);
		}

		public async Task<List<T>> GetOrderedItemsAsync<T, TKey>(Func<T, TKey> keySelector) where T : IBusinessEntity, new()
		{
			var list = await GetItemsAsync<T>();
			return list.OrderBy(keySelector).ToList();
		}

		public async Task<T> GetItemAsync<T>(int id) where T : IBusinessEntity, new()
		{
			return await db.GetWithChildrenAsync<T>(id);
		}

		public async Task SaveItemAsync<T>(T item) where T : IBusinessEntity, new()
		{
			if (item.IDD == 0)
			{
                await db.RunInTransactionAsync(conn =>
                {
                    conn.InsertWithChildren(item, recursive: true);
                });
			}
			else
			{
				await db.UpdateWithChildrenAsync(item);
			}
		}

		public async Task DeleteItemAsync<T>(T item) where T : IBusinessEntity, new()
		{
			await db.DeleteAsync(item, recursive: true);
		}

		public async Task DeleteItemsAsync<T>(Predicate<T> predicate) where T : IBusinessEntity, new()
		{
			var items = await GetItemsAsync<T>();
			foreach (var item in items)
			{
				if (predicate(item))
				{
					await DeleteItemAsync(item);
				}
			}
		}
	}
}
