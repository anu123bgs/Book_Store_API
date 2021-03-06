using Blazored.LocalStorage;
using BookStore_UI.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BookStore_UI.Services
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly IHttpClientFactory client_;
        private readonly ILocalStorageService localStorage_;

        public BaseRepository(IHttpClientFactory client, ILocalStorageService localStorage)
        {
            client_ = client;
            localStorage_ = localStorage;
        }
        public async Task<bool> Create(string url, T obj)
        {
            if (obj == null)
                return false;
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new StringContent(JsonConvert.SerializeObject(obj), 
                Encoding.UTF8, "application/json");
            var client = client_.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", await GetBearerToken());
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Created)
                return true;
            return false;
        }

        public async Task<bool> Delete(string url, int id)
        {
            if (id < 1)
                return false;
            var request = new HttpRequestMessage(HttpMethod.Delete, url + id);
            var client = client_.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", await GetBearerToken());
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return true;
            return false;
        }

        public async Task<T> Get(string url, int id)
        {
            if (id < 1)
                return null;
            var request = new HttpRequestMessage(HttpMethod.Get, url + id);
            var client = client_.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", await GetBearerToken());
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(content);
            }
            return null;
        }

        public async Task<IList<T>> Get(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var client = client_.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IList<T>>(content);
            }
            return null;
        }

        public async Task<bool> Update(string url, T obj, int id)
        {
            if (obj == null)
                return false;
            var request = new HttpRequestMessage(HttpMethod.Put, url+id);
            request.Content = new StringContent(JsonConvert.SerializeObject(obj), 
                Encoding.UTF8, "application/json");
            var client = client_.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", await GetBearerToken());
            HttpResponseMessage response = await client.SendAsync(request);
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
