using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace rkbcMobile.Repository
{
    public interface IDataStore<T> where T : class
    {

        Task<bool> AddItemAsync(T item);
        Task<bool> UpdateItemAsync(T item);
        Task<bool> DeleteItemAsync(string id);
        Task<T> GetItemAsync(string id);
        Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);
    }
}
