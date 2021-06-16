using Blazored.LocalStorage;
using BookStore_UI.WASM.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace BookStore_UI.WASM.Services
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly HttpClient client_;
        private readonly ILocalStorageService localStorage_;

        public BaseRepository(HttpClient client, ILocalStorageService localStorage)
        {
            client_ = client;
            localStorage_ = localStorage;
        }
        public async Task<bool> Create(string url, T obj)
        {
            if (obj == null)
                return false;
            client_.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", await GetBearerToken());
            HttpResponseMessage response = await client_.PostAsJsonAsync<T>(url, obj);
            if (response.StatusCode == System.Net.HttpStatusCode.Created)
                return true;
            return false;
        }

        public async Task<bool> Delete(string url, int id)
        {
            if (id < 1)
                return false;
            client_.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", await GetBearerToken());
            HttpResponseMessage response = await client_.DeleteAsync(url + id);
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return true;
            return false;
        }

        public async Task<T> Get(string url, int id)
        {
            if (id < 1)
                return null;
            client_.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", await GetBearerToken());
            var response = await client_.GetFromJsonAsync<T>(url + id);
            return response;
        }

        public async Task<IList<T>> Get(string url)
        {
            client_.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", await GetBearerToken());
            var response = await client_.GetFromJsonAsync<IList<T>>(url);
            return response;
        }

        public async Task<bool> Update(string url, T obj, int id)
        {
            if (obj == null)
                return false;
            client_.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", await GetBearerToken());
            HttpResponseMessage response = await client_.PutAsJsonAsync<T>(url + id, obj);
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return true;
            return false;
        }
        private async Task<string> GetBearerToken()
        {
            return await localStorage_.GetItemAsync<string>("authToken");
        }
    }
}
