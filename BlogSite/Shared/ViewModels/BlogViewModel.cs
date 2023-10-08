using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BlogSite.Shared.ViewModels
{
    public class BlogViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? SubTitle { get; set; }
        public string Content { get; set; } = string.Empty;
        [Required]
        [RegularExpression(@"^[^#\?\.\s]+$", ErrorMessage = "Whitespace and any special characters are not allowed.")]
        public string Slug { get; set; } = string.Empty;
        public string? BannerImageUrl { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage ="Please select a category")]
        public int CategoryId { get; set; }
        public BlogCategoryViewModel? Category { get; set; }
        public IFormFile? BannerImage { get; set; }
        public AppUserViewModel? CreatedBy { get; set; }
        public string CreatedById { get; set; } = string.Empty;
        public DateTime? Created { get; set; }
        public DateTime? LastUpdated { get; set; }
        public bool Editable { get; set; }
    }
}
