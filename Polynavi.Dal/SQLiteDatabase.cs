using Polynavi.Common.Models;
using SQLite;
using SQLiteNetExtensions.Extensions;
using SQLiteNetExtensionsAsync.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Polynavi.Dal
{
    public class SQLiteDatabase
    {
        private readonly SQLiteAsyncConnection dbConnection;

        public SQLiteDatabase(string dbPath)
        {
            dbConnection = new SQLiteAsyncConnection(dbPath);
        }

        public async Task CreateTableAsync<T>() where T : Entity, new()
        {
            await dbConnection.CreateTableAsync<T>();
        }

        public async Task DropTableAsync<T>() where T : Entity, new()
        {
            await dbConnection.DropTableAsync<T>();
        }

        public async Task<int> CountAsync<T>() where T : Entity, new()
        {
            return await dbConnection.Table<T>().CountAsync();
        }

        public async Task<bool> IsEmptyAsync<T>() where T : Entity, new()
        {
            var count = await CountAsync<T>();
            return count == 0;
        }

        public async Task<List<T>> GetItemsAsync<T>() where T : Entity, new()
        {
            return await dbConnection.GetAllWithChildrenAsync<T>(recursive: true);
        }

        public async Task<List<T>> GetOrderedItemsAsync<T, TKey>(Func<T, TKey> keySelector)
            where T : Entity, new()
        {
            var list = await GetItemsAsync<T>();
            return list.OrderBy(keySelector).ToList();
        }

        public async Task<T> GetItemAsync<T>(int id) where T : Entity, new()
        {
            return await dbConnection.GetWithChildrenAsync<T>(id);
        }

        public async Task SaveItemAsync<T>(T item) where T : Entity, new()
        {
            if (item.Db_Id == Guid.Empty)
            {
                await dbConnection.RunInTransactionAsync(conn =>
                {
                    conn.InsertWithChildren(item, true);
                });
            }
            else
            {
                await dbConnection.UpdateWithChildrenAsync(item);
            }
        }

        public async Task DeleteItemAsync<T>(T item) where T : Entity, new()
        {
            await dbConnection.DeleteAsync(item, recursive: true);
        }

        public async Task DeleteItemsAsync<T>(Predicate<T> predicate) where T : Entity, new()
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
