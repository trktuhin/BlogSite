namespace BlogSite.Server.Entity
{
    public class Blog
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? SubTitle { get; set; }
        public string Content { get; set; }
        public string Slug { get; set; }
        public string? BannerImageUrl { get; set; }
        public AppUser CreatedBy { get; set; }
        public BlogCategory Category { get; set; }
        public int CategoryId { get; set; }
        public string CreatedById { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
