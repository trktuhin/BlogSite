using Blazored.SessionStorage;
using BlogSite.Shared.ViewModels;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlogSite.Client.Utility
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ISessionStorageService _sessionStorageService;
        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        public CustomAuthStateProvider(ISessionStorageService sessionStorageService)
        {
            _sessionStorageService = sessionStorageService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var userSession = await _sessionStorageService.ReadEncryptedItemAsync<AppUserViewModel>("UserSession");
                if (userSession == null)
                {
                    return await Task.FromResult(new AuthenticationState(_anonymous));
                }
                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userSession.Email),
                    new Claim(ClaimTypes.Role, userSession.Role ?? "")
                }, "JwtAuth"));
                return await Task.FromResult(new AuthenticationState(claimsPrincipal));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new AuthenticationState(_anonymous));
            }
        }

        public async Task UpdateAuthenticationState(AppUserViewModel? userSession)
        {
            ClaimsPrincipal claimsPrincipal;
            if (userSession != null)
            {
                claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userSession.Email),
                    new Claim(ClaimTypes.Role, userSession.Role??"")
                }));
                userSession.ExpiryTimeStamp = DateTime.Now.AddSeconds(userSession.ExpiresIn);
                await _sessionStorageService.SaveItemEncryptedAsync("UserSession", userSession);

            }
            else
            {
                claimsPrincipal = _anonymous;
                await _sessionStorageService.RemoveItemAsync("UserSession");
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        public async Task<string> GetToken()
        {
            var result = string.Empty;
            try
            {
                var userSession = await _sessionStorageService.ReadEncryptedItemAsync<AppUserViewModel>("UserSession");
                if (userSession != null && DateTime.Now < userSession.ExpiryTimeStamp)
                {
                    result = userSession.Token;
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }

        public async Task<bool> IsAdmin()
        {
            try
            {
                var userSession = await _sessionStorageService.ReadEncryptedItemAsync<AppUserViewModel>("UserSession");
                if(userSession?.Role?.ToLower() == "admin")
                {
                    return true;
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        public async Task<AppUserViewModel> GetUserDetails()
        {
            return await _sessionStorageService.ReadEncryptedItemAsync<AppUserViewModel>("UserSession");
        }
    }
}
