using AutoMapper;
using BlogSite.Server.Entity;
using BlogSite.Shared.ViewModels;

namespace BlogSite.Server.Utility
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<AppUser, AppUserViewModel>().ReverseMap();
            CreateMap<Blog, BlogViewModel>().ReverseMap();
            CreateMap<BlogCategory, BlogCategoryViewModel>().ReverseMap();
            CreateMap<Comment, CommentViewModel>().ReverseMap();
        }
    }
}
