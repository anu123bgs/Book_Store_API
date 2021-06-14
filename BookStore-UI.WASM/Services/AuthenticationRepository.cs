using Blazored.LocalStorage;
using BookStore_UI.WASM.Contracts;
using BookStore_UI.WASM.Models;
using BookStore_UI.WASM.Providers;
using BookStore_UI.WASM.Static;
using Microsoft.AspNetCore.Components.Authorization;
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
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly HttpClient client_;
        private readonly ILocalStorageService localStorage_;
        private readonly AuthenticationStateProvider authenticationStateProvider_;
        public AuthenticationRepository(HttpClient client,
            ILocalStorageService localStorage,
            AuthenticationStateProvider authenticationStateProvider)
        {
            client_ = client;
            localStorage_ = localStorage;
            authenticationStateProvider_ = authenticationStateProvider;
        }

        public async Task<bool> Login(LoginModel user)
        {
            HttpResponseMessage response = await client_.PostAsJsonAsync(EndPoints.LoginEndpoint,user);

            if (!response.IsSuccessStatusCode)
                return false;
            var content = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<TokenResponse>(content);
            // store token
            await localStorage_.SetItemAsync("authToken", token.Token);
            //change auth state
            await ((ApiAuthenticationStateProvider)authenticationStateProvider_).LoggedIn();
            client_.DefaultRequestHeaders.Authorization =
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
            HttpResponseMessage response = await client_.PostAsJsonAsync(EndPoints.RegisterEndpoint, user);
            
            return response.IsSuccessStatusCode;
        }
    }
}
