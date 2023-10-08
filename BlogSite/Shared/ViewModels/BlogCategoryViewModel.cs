using System.ComponentModel.DataAnnotations;

namespace BlogSite.Shared.ViewModels
{
    public class BlogCategoryViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
