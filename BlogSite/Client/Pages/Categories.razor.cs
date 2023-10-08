using Blazored.Toast.Services;
using BlogSite.Client.Utility;
using BlogSite.Shared.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace BlogSite.Client.Pages
{
    public class CategoriesBase: ComponentBase
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }
        [Inject]
        protected HttpClient HttpClient { get; set; }
        [Inject]
        protected AuthenticationStateProvider AuthSateProvider { get; set; }
        [Inject]
        protected IToastService ToastService { get; set; }
        protected BlogCategoryViewModel SelectedCategoryVM { get; set; } = new();
        protected List<BlogCategoryViewModel> CategoryList { get; set; } = new();
        protected string DetailModalTitle { get; set; } = string.Empty;
        protected bool FetchingCategory { get; set; }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("setActiveSideNav", "nav-category");
                await FetchCategories();
            }
        }
        protected async Task OpenDetailModal(int id = 0)
        {
            if(id == 0)
            {
                SelectedCategoryVM = new();
                DetailModalTitle = "New Category";
            }
            else
            {
                var selectedCategory = CategoryList.FirstOrDefault(x => x.Id == id);
                if(selectedCategory != null)
                {
                    SelectedCategoryVM = selectedCategory;
                }
                DetailModalTitle = "Edit Category";
            }

            StateHasChanged();
            await JSRuntime.InvokeVoidAsync("showModal", "categoryDetailModal");
        }
        protected async Task OpenDeleteModal(int id)
        {
            var selectedCategory = CategoryList.FirstOrDefault(x => x.Id == id);
            if (selectedCategory != null)
            {
                SelectedCategoryVM = selectedCategory;
                StateHasChanged();
                await JSRuntime.InvokeVoidAsync("showModal", "categoryDeleteModal");
            }
        }
        protected async Task HandleDelete()
        {
            await HttpClient.SetAuthorization(AuthSateProvider);
            var endpoint = "api/BlogCategory/Delete/";

            var response = await HttpClient.DeleteAsync(endpoint + SelectedCategoryVM.Id);

            await JSRuntime.InvokeVoidAsync("hideModal", "categoryDeleteModal");

            if (response.IsSuccessStatusCode)
            {
                ToastService.ShowSuccess("Category deleted successfully");
                await FetchCategories();
            }
            else
            {
                ToastService.ShowError("Something went wrong!");
            }
        }

        protected async Task HandleSubmit()
        {
            await HttpClient.SetAuthorization(AuthSateProvider);
            var endpoint = "api/BlogCategory/Create";

            if (SelectedCategoryVM.Id > 0)
            {
                endpoint = "api/BlogCategory/Update";
            }

            var response = await HttpClient.PostAsJsonAsync(endpoint, SelectedCategoryVM);

            await JSRuntime.InvokeVoidAsync("hideModal", "categoryDetailModal");

            if (response.IsSuccessStatusCode)
            {
                ToastService.ShowSuccess("Category saved successfully");
                await FetchCategories();
            }
            else
            {
                ToastService.ShowError("Something went wrong!");
            }
        }

        private async Task FetchCategories()
        {
            FetchingCategory = true;
            StateHasChanged();

            var categories = await HttpClient.GetFromJsonAsync<IEnumerable<BlogCategoryViewModel>>("api/BlogCategory/GetAll");
            if (categories != null && categories.Count() > 0)
            {
                CategoryList = categories.ToList();
            }
            else
            {
                CategoryList = new();
            }
            FetchingCategory = false;
            StateHasChanged();
        }
    }
}
