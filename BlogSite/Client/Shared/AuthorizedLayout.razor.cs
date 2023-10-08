using BlogSite.Client.Utility;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace BlogSite.Client.Shared
{
    public class AuthorizedLayoutBase : LayoutComponentBase
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }
        [Inject]
        protected AuthenticationStateProvider AuthStateProvider { get; set; }
        [Inject]
        protected NavigationManager NavManager { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("initializeTooltip");
            }
        }

        protected async Task Logout()
        {
            var custAuthSateProvider = (CustomAuthStateProvider)AuthStateProvider;
            await custAuthSateProvider.UpdateAuthenticationState(null);
            NavManager.NavigateTo("/");
        }
    }
}
