using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Essentials;
using rkbcMobile.Models;

namespace rkbcMobile.Repository
{
    
    public class DataStore<T> : IDataStore<T> where T : class
    {
        HttpClient client;

        IEnumerable<T> items;
        protected string webApiService;
        public DataStore(string _webApiService)
        {
            client = new HttpClient();
            client.BaseAddress = new Uri($"{App.AzureBackendUrl}/");

            items = new List<T>();
            webApiService = _webApiService;
        }

        bool IsConnected => Connectivity.NetworkAccess == NetworkAccess.Internet;
        public async Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false)
        {
            if (forceRefresh && IsConnected)
            {
                var json = await client.GetStringAsync(webApiService + "List");
                //var list = JsonConvert.DeserializeObject<IEnumerable<Item>>(json);
                items = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<T>>(json));
            }

            return items;
        }

        public async Task<T> GetItemAsync(string id)
        {
            if (id != null && IsConnected)
            {
                var json = await client.GetStringAsync($"api/item/{id}");
                return await Task.Run(() => JsonConvert.DeserializeObject<T>(json));
            }

            return null;
        }

        public async Task<bool> AddItemAsync(T item)
        {
            if (item == null || !IsConnected)
                return false;

            var serializedItem = JsonConvert.SerializeObject(item);

            var response = await client.PostAsync($"api/item", new StringContent(serializedItem, Encoding.UTF8, "application/json"));

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateItemAsync(T item)
        {
            //if (item == null || item.Id == null || !IsConnected)
            //    return false;

            var serializedItem = JsonConvert.SerializeObject(item);
            var buffer = Encoding.UTF8.GetBytes(serializedItem);
            var byteContent = new ByteArrayContent(buffer);
            var id = 1;
            var response = await client.PutAsync(new Uri($"api/item/{id}"), byteContent);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            if (string.IsNullOrEmpty(id) && !IsConnected)
                return false;

            var response = await client.DeleteAsync($"api/item/{id}");

            return response.IsSuccessStatusCode;
        }
    }
}
