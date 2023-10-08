namespace BlogSite.Shared.ViewModels
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int TargetBlogId { get; set; }
        public AppUserViewModel? CommentedBy { get; set; }
        public string? CommentedById { get; set; }
        public DateTime? Created { get; set; }
    }
}
