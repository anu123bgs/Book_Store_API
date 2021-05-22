using Blazored.LocalStorage;
using BookStore_UI.Contracts;
using BookStore_UI.Models;
using BookStore_UI.Providers;
using BookStore_UI.Static;
using Microsoft.AspNetCore.Components.Authorization;
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
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly IHttpClientFactory client_;
        private readonly ILocalStorageService localStorage_;
        private readonly AuthenticationStateProvider authenticationStateProvider_;
        public AuthenticationRepository(IHttpClientFactory client,
            ILocalStorageService localStorage,
            AuthenticationStateProvider authenticationStateProvider)
        {
            client_ = client;
            localStorage_ = localStorage;
            authenticationStateProvider_ = authenticationStateProvider;
        }

        public async Task<bool> Login(LoginModel user)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, EndPoints.LoginEndpoint);
            request.Content = new StringContent(JsonConvert.SerializeObject(user), 
                Encoding.UTF8, "application/json");
            var client = client_.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return false;
            var content = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<TokenResponse>(content);
            // store token
            await localStorage_.SetItemAsync("authToken", token.Token);
            //change auth state
            await ((ApiAuthenticationStateProvider)authenticationStateProvider_).LoggedIn();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", token.Token);
            return true;
        }

        public async Task Logout()
        {
            await localStorage_.RemoveItemAsync("authToken");
            ((ApiAuthenticationStateProvider)authenticationStateProvider_).LoggedOut();
        }

        public async Task<bool> Register(RegistrationModel user)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, EndPoints.RegisterEndpoint);
            request.Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            var client = client_.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);
            
            return response.IsSuccessStatusCode;
        }
    }
}
