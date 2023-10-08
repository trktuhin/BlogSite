using Blazored.Toast.Services;
using BlogSite.Shared;
using BlogSite.Shared.SearchParams;
using BlogSite.Shared.ViewModels;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace BlogSite.Client.Pages
{
    public class IndexBase: ComponentBase
    {
        [Inject]
        protected HttpClient HttpClient { get; set; }
        [Inject]
        protected IToastService ToastService { get; set; }
        [Inject]
        protected NavigationManager NavManager { get; set; }
        protected bool FetchingBlogs { get; set; }
        protected List<BlogViewModel> FilteredBlogs { get; set; } = new();
        protected BlogParams PageParams { get; set; } = new ();

        protected int totalPage = 0;

        private bool notReached = true;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await FetchBlogs();
            }
        }

        protected async Task FetchBlogs()
        {
            FetchingBlogs = true;
            StateHasChanged();
            PageParams.PageNumber = 1;
            PageParams.PageSize = 12;

            var response = await HttpClient.PostAsJsonAsync<BlogParams>("api/Blog/GetAllPagedBlogs", PageParams);

            if (response != null && response.IsSuccessStatusCode)
            {
                try
                {
                    var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<BlogViewModel>>();
                    if (pagedResponse != null && pagedResponse.Items.Count() > 0)
                    {
                        FilteredBlogs = pagedResponse.Items.ToList();
                        totalPage = pagedResponse.TotalPages;
                        notReached = true;
                    }
                    else
                    {
                        FilteredBlogs = new List<BlogViewModel>();
                    }
                }
                catch (Exception)
                {
                    ToastService.ShowError("Something went wrong!");
                }
            }

            FetchingBlogs = false;
            StateHasChanged();
        }

        protected async Task LoadMore()
        {
            if (PageParams.PageNumber >= totalPage) return;

            if (notReached)
            {
                notReached = false;
                return;
            }

            PageParams.PageNumber++;
            var response = await HttpClient.PostAsJsonAsync<BlogParams>("api/Blog/GetAllPagedBlogs", PageParams);
            if (response != null && response.IsSuccessStatusCode)
            {
                var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<BlogViewModel>>();
                if (pagedResponse != null && pagedResponse.Items.Count() > 0)
                {
                    var moreBlogs = pagedResponse.Items.ToList();
                    FilteredBlogs.AddRange(moreBlogs);
                }
            }
            notReached = true;
        }
    }
}
