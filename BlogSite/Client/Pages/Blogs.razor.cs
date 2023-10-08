using Blazored.TextEditor;
using Blazored.Toast.Services;
using BlogSite.Client.Utility;
using BlogSite.Shared;
using BlogSite.Shared.SearchParams;
using BlogSite.Shared.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace BlogSite.Client.Pages
{
    public class BlogsBase: ComponentBase
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }
        [Inject]
        protected HttpClient HttpClient { get; set; }
        [Inject]
        protected AuthenticationStateProvider AuthSateProvider { get; set; }
        [Inject]
        protected IToastService ToastService { get; set; }
        protected BlogViewModel SelectedBlogVM { get; set; } = new();
        protected List<BlogViewModel> BlogList { get; set; } = new();
        protected List<BlogCategoryViewModel> CategoryList { get; set; } = new();

        protected BlogParams pageParams = new BlogParams();
        protected string DetailModalTitle { get; set; } = string.Empty;
        protected IBrowserFile? ImageFile { get; set; }
        protected byte[] ImageData { get; set; }
        protected string ImageDataUrl { get; set; }

        protected BlazoredTextEditor QuillHtml;

        protected int totalPage = 0;
        protected int currentPage = 1;
        protected int totalCount = 1;

        protected bool FetchingBlogs { get; set; }
        protected bool FetchingCategory { get; set; }
        protected bool IsAdmin { get; set; }
        public Guid inputFileId = Guid.NewGuid();
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("setActiveSideNav", "nav-blog");
                await HttpClient.SetAuthorization(AuthSateProvider);
                IsAdmin = await ((CustomAuthStateProvider)AuthSateProvider).IsAdmin();
                await FetchBlogs();
                await FetchCategories();
            }
        }
        protected async Task OpenDetailModal(int id = 0)
        {
            if (id == 0)
            {
                SelectedBlogVM = new();
                DetailModalTitle = "New Blog";
            }
            else
            {
                var selectedCategory = BlogList.FirstOrDefault(x => x.Id == id);
                if (selectedCategory != null)
                {
                    SelectedBlogVM = selectedCategory;
                }
                DetailModalTitle = "Edit Blog";
            }

            inputFileId = Guid.NewGuid();
            ImageDataUrl = string.Empty;
            ImageFile = null;

            StateHasChanged();
            await JSRuntime.InvokeVoidAsync("showModal", "blogDetailModal");
            if (!string.IsNullOrEmpty(SelectedBlogVM.Content))
            {
                await QuillHtml.LoadHTMLContent(SelectedBlogVM.Content);
            }
        }
        protected async Task OpenDeleteModal(int id)
        {
            var selectedCategory = BlogList.FirstOrDefault(x => x.Id == id);
            if (selectedCategory != null)
            {
                SelectedBlogVM = selectedCategory;
                StateHasChanged();
                await JSRuntime.InvokeVoidAsync("showModal", "blogDeleteModal");
            }
        }
        protected async Task HandleDelete()
        {
            await HttpClient.SetAuthorization(AuthSateProvider);
            var endpoint = "api/Blog/Delete/";

            var response = await HttpClient.DeleteAsync(endpoint + SelectedBlogVM.Id);

            await JSRuntime.InvokeVoidAsync("hideModal", "blogDeleteModal");

            if (response.IsSuccessStatusCode)
            {
                ToastService.ShowSuccess("Category deleted successfully");
                await FetchBlogs();
            }
            else
            {
                ToastService.ShowError("Something went wrong!");
            }
        }
        protected async Task HandleSubmit()
        {
            var content = await QuillHtml.GetHTML();
            if (string.IsNullOrEmpty(content))
            {
                ToastService.ShowError("Please enter some content and submit again!");
                return;
            }

            var formContent = new MultipartFormDataContent();
            formContent.Add(new StringContent(SelectedBlogVM.Id.ToString()), "id");
            formContent.Add(new StringContent(SelectedBlogVM.Title), "title");
            formContent.Add(new StringContent(SelectedBlogVM.SubTitle ?? ""), "subTitle");
            formContent.Add(new StringContent(SelectedBlogVM.Slug ?? ""), "slug");
            formContent.Add(new StringContent(SelectedBlogVM.BannerImageUrl ?? ""), "bannerImageUrl");
            formContent.Add(new StringContent(SelectedBlogVM.CategoryId.ToString()), "categoryId");
            formContent.Add(new StringContent(content), "content");

            // Add the image file
            if (ImageFile != null)
            {
                var imageContent = new ByteArrayContent(ImageData);
                imageContent.Headers.ContentType = new MediaTypeHeaderValue(ImageFile.ContentType);
                imageContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "bannerImage",
                    FileName = ImageFile.Name
                };
                formContent.Add(imageContent);
            }

            var endpoint = "api/Blog/Create";

            if (SelectedBlogVM.Id > 0)
            {
                endpoint = "api/Blog/Update";
            }

            var response = await HttpClient.PostAsync(endpoint, formContent);

            await JSRuntime.InvokeVoidAsync("hideModal", "blogDetailModal");

            if (response.IsSuccessStatusCode)
            {
                ToastService.ShowSuccess("Blog saved successfully");
                await FetchBlogs();
            }
            else
            {
                ToastService.ShowError("Something went wrong!");
            }
        }
        protected async Task FetchBlogs()
        {
            FetchingBlogs = true;
            StateHasChanged();

            var response = await HttpClient.PostAsJsonAsync<BlogParams>("api/Blog/GetAllPagedBlogs", pageParams);

            if (response != null && response.IsSuccessStatusCode)
            {
                try
                {
                    var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<BlogViewModel>>();
                    if (pagedResponse != null && pagedResponse.Items.Count() > 0)
                    {
                        BlogList = pagedResponse.Items.ToList();
                        totalPage = pagedResponse.TotalPages;
                        currentPage = pagedResponse.CurrentPage;
                        totalCount = pagedResponse.TotalCount;
                    }
                    else
                    {
                        BlogList = new List<BlogViewModel>();
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
        protected async Task HandleFileSelection(InputFileChangeEventArgs e)
        {
            ImageFile = e.File;
            var buffer = new byte[ImageFile.Size];
            await ImageFile.OpenReadStream().ReadAsync(buffer);
            ImageData = buffer;
            ImageDataUrl = $"data:image/jpg;base64,{Convert.ToBase64String(ImageData)}";
            StateHasChanged();
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

        protected async Task OnCategoryChanged(int value)
        {
            pageParams.CategoryId = value;
            await FetchBlogs();
        }
        protected void HandleInputBlur(FocusEventArgs args)
        {
            SelectedBlogVM.Slug = ReplaceSpecialCharacters(SelectedBlogVM.Title);
        }

        private string ReplaceSpecialCharacters(string input)
        {
            // Use a regular expression to replace white spaces, '?', '#', and '.'
            string pattern = @"[\s?#.]";
            string replacement = "-";
            string result = Regex.Replace(input, pattern, replacement);
            return result;
        }
    }
}
