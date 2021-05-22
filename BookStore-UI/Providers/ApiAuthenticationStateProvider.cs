using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookStore_UI.Providers
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService localStorage_;
        private readonly JwtSecurityTokenHandler tokenHandler_;
        public ApiAuthenticationStateProvider(ILocalStorageService localStorage, 
            JwtSecurityTokenHandler tokenHandler)
        {
            localStorage_ = localStorage;
            tokenHandler_ = tokenHandler;
        }
        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var savedToken = await localStorage_.GetItemAsync<string>("authToken");
                if (string.IsNullOrEmpty(savedToken))
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                var tokenContent = tokenHandler_.ReadJwtToken(savedToken);
                var expiry = tokenContent.ValidTo;
                if (DateTime.Now < expiry)
                {
                    await localStorage_.RemoveItemAsync("authToken");
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }
                //Get Claims from token and build auth User object
                var claims = ParseClaims(tokenContent);
                var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
                return new AuthenticationState(user);
            }
            catch (Exception e)
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }
        public async Task LoggedIn()
        {
            var savedToken = await localStorage_.GetItemAsync<string>("authToken");
            var tokenContent = tokenHandler_.ReadJwtToken(savedToken);
            var claims = ParseClaims(tokenContent);
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
            var authState = Task.FromResult(new AuthenticationState(user));
            NotifyAuthenticationStateChanged(authState);
        }
        public void LoggedOut()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(user));
            NotifyAuthenticationStateChanged(authState);
        }
        private IList<Claim> ParseClaims(JwtSecurityToken tokenContent)
        {
            var claims = tokenContent.Claims.ToList();
            claims.Add(new Claim(ClaimTypes.Name, tokenContent.Subject));
            return claims;
        }
    }
}
