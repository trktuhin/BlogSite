namespace BlogSite.Server.Entity
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public Blog TargetBlog { get; set; }
        public int TargetBlogId { get; set; }
        public AppUser CommentedBy { get; set; }
        public string? CommentedById { get; set; }
        public DateTime Created { get; set; }
    }
}
