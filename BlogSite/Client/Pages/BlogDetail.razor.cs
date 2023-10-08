using Blazored.Toast.Services;
using BlogSite.Client.Utility;
using BlogSite.Shared.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;

namespace BlogSite.Client.Pages
{
    public class BlogDetailBase: ComponentBase
    {
        [Parameter]
        public string BlogSlug { get; set; }
        [Inject]
        protected HttpClient HttpClient { get; set; }
        [Inject]
        protected NavigationManager NavManager { get; set; }
        [Inject]
        protected AuthenticationStateProvider AuthSateProvider { get; set; }
        [Inject]
        protected IToastService ToastService { get; set; }
        protected BlogViewModel ViewModel { get; set; } = new();
        protected List<BlogViewModel> RelatedBlogs { get; set; } = new();
        protected List<CommentViewModel> BlogComments { get; set; } = new();
        protected string CommentInputText { get; set; } = string.Empty;
        protected bool FetchingDetails { get; set; } = false;
        protected bool FetchingRelatedBlogs { get; set; } = false;
        protected bool FetchingComments { get; set; } = false;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await HttpClient.SetAuthorization(AuthSateProvider);
                if (!string.IsNullOrEmpty(BlogSlug))
                {
                    await LoadBlog(BlogSlug);
                    await LoadRelatedBlogs(BlogSlug);
                    await LoadComments(BlogSlug);
                }
            }
        }

        private async Task LoadBlog(string BlogSlug)
        {
            FetchingDetails = true;
            StateHasChanged();
            var blog = await HttpClient.GetFromJsonAsync<BlogViewModel>($"api/Blog/GetBlogBySlug/{BlogSlug}");
            if (blog != null)
            {
                ViewModel = blog;
            }
            FetchingDetails = false;
            StateHasChanged();
        }

        private async Task LoadRelatedBlogs(string BlogSlug)
        {
            FetchingRelatedBlogs = true;
            StateHasChanged();
            var relatedBlogs = await HttpClient.GetFromJsonAsync<List<BlogViewModel>>($"api/Blog/GetRelatedBlogs/{BlogSlug}");
            if (relatedBlogs != null)
            {
                RelatedBlogs = relatedBlogs;
            }
            FetchingRelatedBlogs = false;
            StateHasChanged();
        }
        private async Task LoadComments(string BlogSlug)
        {
            FetchingComments = true;
            StateHasChanged();
            var comments = await HttpClient.GetFromJsonAsync<List<CommentViewModel>>($"api/Blog/GetBlogComments/{BlogSlug}");
            if (comments != null)
            {
                BlogComments = comments;
            }
            FetchingComments = false;
            StateHasChanged();
        }

        protected async Task SubmitComment()
        {
            if(string.IsNullOrEmpty(CommentInputText))
            {
                ToastService.ShowWarning("Type your comment first");
                return;
            }

            var comment = new CommentViewModel
            {
                TargetBlogId = ViewModel.Id,
                Content = CommentInputText
            };

            var response = await HttpClient.PostAsJsonAsync("api/Blog/AddComment", comment);

            if (response != null && response.IsSuccessStatusCode)
            {
                CommentInputText = string.Empty;
                ToastService.ShowSuccess("Comment added successfully");
                await LoadComments(ViewModel.Slug);
            }
        }
    }
}
