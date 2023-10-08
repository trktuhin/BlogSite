using BlogSite.Client.Utility;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlogSite.Client.Shared
{
    public class TopBarBase: ComponentBase
    {
        [Inject]
        protected AuthenticationStateProvider AuthStateProvider { get; set; }
        [Inject]
        protected NavigationManager NavManager { get; set; }

        protected async Task Logout()
        {
            var custAuthSateProvider = (CustomAuthStateProvider)AuthStateProvider;
            await custAuthSateProvider.UpdateAuthenticationState(null);
            NavManager.NavigateTo("/");
        }
    }
}
