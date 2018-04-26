using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using SQLite;

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
		
		public async Task CreateTableAsync<T>() where T : IBuisnessEntity, new()
		{
			await db.CreateTableAsync<T>();
		}

		public async Task<List<T>> GetItemsAsync<T>() where T : IBuisnessEntity, new()
		{
			return await db.Table<T>().ToListAsync();
		}

		public async Task<T> GetItemAsync<T>(int id) where T : IBuisnessEntity, new()
		{
			return await db.GetAsync<T>(id);
		}

		public async Task<int> SaveItemAsync<T>(T item) where T : IBuisnessEntity, new()
		{
			if (item.ID == 0)
			{
				return await db.InsertAsync(item);
			}
			else
			{
				return await db.UpdateAsync(item);
			}
		}

		public async Task<int> DeleteItemAsync<T>(T item) where T : IBuisnessEntity, new()
		{
			return await db.DeleteAsync(item);
		}
	}
}
