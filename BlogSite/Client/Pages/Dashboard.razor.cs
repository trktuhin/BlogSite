using BlogSite.Client.Utility;
using BlogSite.Shared.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace BlogSite.Client.Pages
{
    public class DashboardBase: ComponentBase
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }
        [Inject]
        protected AuthenticationStateProvider AuthSateProvider { get; set; }

        protected AppUserViewModel UserViewModel { get; set; } = new();

        protected List<DemoChartViewModel> SampleData { get; set; } = new List<DemoChartViewModel>
        {
            new DemoChartViewModel{PropertyName="January", PropertyValue = 10},
            new DemoChartViewModel{PropertyName="February", PropertyValue = 8},
            new DemoChartViewModel{PropertyName="March", PropertyValue = 12},
            new DemoChartViewModel{PropertyName="April", PropertyValue = 7},
            new DemoChartViewModel{PropertyName="May", PropertyValue = 10},
            new DemoChartViewModel{PropertyName="June", PropertyValue = 11},
            new DemoChartViewModel{PropertyName="July", PropertyValue = 2},
            new DemoChartViewModel{PropertyName="August", PropertyValue = 6},
            new DemoChartViewModel{PropertyName="Semptember", PropertyValue = 8},
            new DemoChartViewModel{PropertyName="October", PropertyValue = 0},
            new DemoChartViewModel{PropertyName="November", PropertyValue = 5},
            new DemoChartViewModel{PropertyName="December", PropertyValue = 16}
        };

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("setActiveSideNav", "nav-dashboard");
                UserViewModel = await ((CustomAuthStateProvider)AuthSateProvider).GetUserDetails();
                StateHasChanged();
            }
        }

        protected string GetColor(DemoChartViewModel model)
        {
            if (model.PropertyName == "February")
            {
                return "#ffd500";
            }
            else if(model.PropertyName == "May")
            {
                return "#34eba1";
            }
            else if (model.PropertyName == "August")
            {
                return "#cc41d1";
            }
            else if( model.PropertyName == "November")
            {
                return "#d1416c";
            }
            return "#41c0d1";
        }
    }
}
