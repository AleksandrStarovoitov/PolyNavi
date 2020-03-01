using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolyNaviLib.BL;
using SQLite;
using SQLiteNetExtensions.Extensions;
using SQLiteNetExtensionsAsync.Extensions;

namespace PolyNaviLib.DL
{
    public class SQLiteDatabase
    {
        private readonly SQLiteAsyncConnection dbConnection;

        public SQLiteDatabase(string dbPath)
        {
            dbConnection = new SQLiteAsyncConnection(dbPath);
        }

        public async Task CreateTableAsync<T>() where T : IBusinessEntity, new()
        {
            await dbConnection.CreateTableAsync<T>();
        }

        public async Task DropTableAsync<T>() where T : IBusinessEntity, new()
        {
            await dbConnection.DropTableAsync<T>();
        }

        public async Task<int> CountAsync<T>() where T : IBusinessEntity, new()
        {
            return await dbConnection.Table<T>().CountAsync();
        }

        public async Task<bool> IsEmptyAsync<T>() where T : IBusinessEntity, new()
        {
            var count = await CountAsync<T>();
            return count == 0;
        }

        public async Task<List<T>> GetItemsAsync<T>() where T : IBusinessEntity, new()
        {
            return await dbConnection.GetAllWithChildrenAsync<T>(recursive: true);
        }

        public async Task<List<T>> GetOrderedItemsAsync<T, TKey>(Func<T, TKey> keySelector) where T : IBusinessEntity, new()
        {
            var list = await GetItemsAsync<T>();
            return list.OrderBy(keySelector).ToList();
        }

        public async Task<T> GetItemAsync<T>(int id) where T : IBusinessEntity, new()
        {
            return await dbConnection.GetWithChildrenAsync<T>(id);
        }

        public async Task SaveItemAsync<T>(T item) where T : IBusinessEntity, new()
        {
            if (item.IDD == 0)
            {
                await dbConnection.RunInTransactionAsync(conn =>
                {
                    conn.InsertWithChildren(item, recursive: true);
                });
            }
            else
            {
                await dbConnection.UpdateWithChildrenAsync(item);
            }
        }

        public async Task DeleteItemAsync<T>(T item) where T : IBusinessEntity, new()
        {
            await dbConnection.DeleteAsync(item, recursive: true);
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
