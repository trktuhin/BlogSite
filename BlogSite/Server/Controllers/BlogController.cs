using AutoMapper;
using BlogSite.Server.Entity;
using BlogSite.Server.Utility;
using BlogSite.Shared;
using BlogSite.Shared.SearchParams;
using BlogSite.Shared.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlogSite.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public BlogController(AppDbContext context, IMapper mapper, UserManager<AppUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _env = env;
        }

        [HttpPost("GetAllPagedBlogs")]
        public async Task<ActionResult<PagedResponse<BlogViewModel>>> GetAllPagedBlogs([FromBody]BlogParams blogParmas)
        {
            var blogs = _context.Blogs.Include(x => x.CreatedBy).Include(x => x.Category).OrderByDescending(x => x.Created).AsQueryable();
            if (!string.IsNullOrEmpty(blogParmas.SearchText))
            {
                blogs = blogs.Where(x => x.Title.ToLower().Contains(blogParmas.SearchText.ToLower()));
            }
            if (blogParmas.CategoryId > 0)
            {
                blogs = blogs.Where(x => x.CategoryId == blogParmas.CategoryId);
            }
            var pagedBlogs = await PagedList<Blog>.CreateAsync(blogs, blogParmas.PageNumber, blogParmas.PageSize);

            var viewModels = _mapper.Map<List<BlogViewModel>>(pagedBlogs.ToList());

            var emailClaim = User.FindFirst(ClaimTypes.Email);
            if (emailClaim != null)
            {
                var user = await _userManager.FindByEmailAsync(emailClaim.Value);
                if(user != null)
                {
                    foreach (var item in viewModels)
                    {
                        if (item.CreatedById == user.Id)
                        {
                            item.Editable = true;
                        }
                    }
                }
            }

            var response = new PagedResponse<BlogViewModel>
            {
                CurrentPage = pagedBlogs.CurrentPage,
                PageSize = pagedBlogs.PageSize,
                TotalCount = pagedBlogs.TotalCount,
                TotalPages = pagedBlogs.TotalPages,
                Items = viewModels
            };

            return Ok(response);
        }

        [HttpGet("GetBlogBySlug/{blogSlug}")]
        public async Task<ActionResult<BlogViewModel>> GetBlogBySlug(string blogSlug)
        {
            var blog = await _context.Blogs.Include(x => x.CreatedBy).Include(x => x.Category).FirstOrDefaultAsync(x => x.Slug.ToLower() == blogSlug.ToLower());

            if(blog == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<BlogViewModel>(blog));
        }
        [HttpGet("GetRelatedBlogs/{blogSlug}")]
        public async Task<ActionResult<List<BlogViewModel>>> GetRelatedBlogs(string blogSlug)
        {
            var blog = await _context.Blogs.FirstOrDefaultAsync(x => x.Slug.ToLower() == blogSlug.ToLower());

            if (blog == null)
            {
                return NotFound();
            }

            var relatedBlogs = await _context.Blogs
                               .Where(x => x.CategoryId == blog.CategoryId && x.Id != blog.Id)
                               .OrderByDescending(x => x.Created).Take(4)
                               .ToListAsync();

            return Ok(_mapper.Map<List<BlogViewModel>>(relatedBlogs));
        }

        [HttpGet("GetBlogComments/{blogSlug}")]
        public async Task<ActionResult<List<CommentViewModel>>> GetBlogComments(string blogSlug)
        {
            var blog = await _context.Blogs.FirstOrDefaultAsync(x => x.Slug.ToLower() == blogSlug.ToLower());

            if (blog == null)
            {
                return NotFound();
            }

            var comments = await _context.Comments
                               .Include(x => x.CommentedBy)
                               .Where(x => x.TargetBlogId == blog.Id)
                               .OrderByDescending(x => x.Created)
                               .ToListAsync();

            return Ok(_mapper.Map<List<CommentViewModel>>(comments));
        }

        [HttpGet("GetBlogById/{id}")]
        public async Task<ActionResult<BlogViewModel>> GetBlogById(int id)
        {
            var blog = await _context.Blogs.Include(x => x.CreatedBy).Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == id);

            if (blog == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<BlogViewModel>(blog));
        }


        [HttpGet("GetOwnPagedBlogs")]
        [Authorize]
        public async Task<ActionResult<PagedResponse<BlogViewModel>>> GetOwnPagedBlogs([FromQuery] BlogParams blogParmas)
        {
            string userId = "";
            var emailClaim = User.FindFirst(ClaimTypes.Email);
            if (emailClaim != null)
            {
                var user = await _userManager.FindByEmailAsync(emailClaim.Value);
                if (user != null)
                {
                    userId = user.Id;
                }
            }
            var blogs = _context.Blogs.Where(x => x.CreatedById == userId).OrderByDescending(x => x.Created).AsQueryable();
            if (!string.IsNullOrEmpty(blogParmas.SearchText))
            {
                blogs = blogs.Where(x => x.Title.ToLower().Contains(blogParmas.SearchText.ToLower()));
            }
            var pagedBlogs = await PagedList<Blog>.CreateAsync(blogs, blogParmas.PageNumber, blogParmas.PageSize);

            var response = new PagedResponse<BlogViewModel>
            {
                CurrentPage = pagedBlogs.CurrentPage,
                PageSize = pagedBlogs.PageSize,
                TotalCount = pagedBlogs.TotalCount,
                TotalPages = pagedBlogs.TotalPages,
                Items = _mapper.Map<List<BlogViewModel>>(pagedBlogs.ToList())
            };

            return Ok(response);
        }

        [HttpPost("Create")]
        [Authorize]
        public async Task<IActionResult> Create([FromForm] BlogViewModel viewModel)
        {
            var blogWithSameSlug = await _context.Blogs.FirstOrDefaultAsync(x => x.Slug.ToLower() == viewModel.Slug.ToLower());

            if (blogWithSameSlug != null)
            {
                return BadRequest("Blog with same slug exists already");
            }

            if (viewModel.BannerImage != null)
            {
                if (viewModel.BannerImage.Length > 5242880) return BadRequest("Maximum size exceeded");

                string[] acceptedFileTypes = { ".jpg", ".jpeg", ".png" };
                if (!(acceptedFileTypes.Any(s => s == Path.GetExtension(viewModel.BannerImage.FileName).ToLower())))
                {
                    return BadRequest("Invalid file type");
                }

                await UploadPhoto(viewModel.BannerImage, viewModel);
            }

            viewModel.Created = DateTime.Now;
            viewModel.LastUpdated = DateTime.Now;

            var emailClaim = User.FindFirst(ClaimTypes.Email);
            if (emailClaim != null)
            {
                var user = await _userManager.FindByEmailAsync(emailClaim.Value);
                if (user != null)
                {
                    viewModel.CreatedById = user.Id;
                }
            }
            var blog = _mapper.Map<Blog>(viewModel);
            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("Update")]
        [Authorize]
        public async Task<IActionResult> Update([FromForm] BlogViewModel viewModel)
        {
            var blogWithSameSlug = await _context.Blogs.FirstOrDefaultAsync(x => x.Slug.ToLower() == viewModel.Slug.ToLower());

            if (blogWithSameSlug != null && blogWithSameSlug.Id != viewModel.Id)
            {
                return BadRequest("Blog with same slug exists already");
            }

            var blog = await _context.Blogs.FindAsync(viewModel.Id);
            if (blog == null)
            {
                return NotFound();
            }

            // upload banner image
            if (viewModel.BannerImage != null)
            {
                if (viewModel.BannerImage.Length > 5242880) return BadRequest("Maximum size exceeded");

                string[] acceptedFileTypes = { ".jpg", ".jpeg", ".png" };
                if (!(acceptedFileTypes.Any(s => s == Path.GetExtension(viewModel.BannerImage.FileName).ToLower())))
                {
                    return BadRequest("Invalid file type");
                }
                await UploadPhoto(viewModel.BannerImage, viewModel);
            }

            blog.Title = viewModel.Title;
            blog.SubTitle = viewModel.SubTitle;
            blog.Content = viewModel.Content;
            blog.CategoryId = viewModel.CategoryId;
            blog.Slug = viewModel.Slug;
            blog.BannerImageUrl = viewModel.BannerImageUrl;
            blog.LastUpdated = DateTime.Now;

            _context.Update(blog);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("Delete/{blogId}")]
        [Authorize]
        public async Task<IActionResult> Delete(int blogId)
        {
            var blog = await _context.Blogs.FindAsync(blogId);
            if (blog == null)
            {
                return NotFound();
            }

            var emailClaim = User.FindFirst(ClaimTypes.Email);
            if (emailClaim != null)
            {
                var user = await _userManager.FindByEmailAsync(emailClaim.Value);
                if (user != null)
                {
                    var isAdmin = await _userManager.IsInRoleAsync(user, "admin");
                    if(isAdmin || blog.CreatedById == user.Id)
                    {
                        _context.Blogs.Remove(blog);
                        await _context.SaveChangesAsync();
                        return Ok();
                    }
                }
            }

            return Unauthorized();
        }

        [HttpPost("AddComment")]
        public async Task<ActionResult> AddComment([FromBody] CommentViewModel viewModel)
        {
            var emailClaim = User.FindFirst(ClaimTypes.Email);
            if (emailClaim != null)
            {
                var user = await _userManager.FindByEmailAsync(emailClaim.Value);
                if (user != null)
                {
                    viewModel.CommentedById = user.Id;
                }
            }
            viewModel.Created = DateTime.Now;

            var comment = _mapper.Map<Comment>(viewModel);
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private async Task UploadPhoto(IFormFile imageFile, BlogViewModel viewModel)
        {
            try
            {
                var uploadFolderPath = Path.Combine(_env.WebRootPath, "images");
                //creating folder if doesn't exist
                if (!Directory.Exists(uploadFolderPath))
                {
                    Directory.CreateDirectory(uploadFolderPath);
                }
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploadFolderPath, fileName);


                //removing existing photoUrl
                if (!string.IsNullOrEmpty(viewModel.BannerImageUrl))
                {
                    var existingFilePath = Path.Combine(uploadFolderPath, viewModel.BannerImageUrl);
                    System.IO.File.Delete(existingFilePath);

                    var existingThumnailPath = Path.Combine(uploadFolderPath, "thumbnail-" + viewModel.BannerImageUrl);
                    System.IO.File.Delete(existingThumnailPath);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                viewModel.BannerImageUrl = fileName;

                var thumnailFilePath = Path.Combine(uploadFolderPath, "thumbnail-" + fileName);
                SaveThumbnailImage(imageFile, thumnailFilePath);
            }
            catch (Exception ex)
            {
            }
        }

        private void SaveThumbnailImage(IFormFile file, string outputPath)
        {
            // Load the image from the IFormFile
            using (var image = Image.Load(file.OpenReadStream()))
            {
                int targetWidth = image.Width;
                int targetHeight = image.Height;

                if ((double)image.Width / image.Height > 1.5)
                {
                    targetWidth = (int)(image.Height * 1.5);
                }
                else
                {
                    targetHeight = (int)(image.Width / 1.5);
                }

                int xposition = (image.Width - targetWidth) / 2;
                int yposition = (image.Height - targetHeight) / 2;

                image.Mutate(x => x.Crop(new Rectangle(xposition, yposition, targetWidth, targetHeight)));

                // Save the cropped image to the specified output path
                image.Save(outputPath);
            }
        }
    }
}
