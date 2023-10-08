using AutoMapper;
using BlogSite.Server.Entity;
using BlogSite.Shared.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogSite.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogCategoryController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public BlogCategoryController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<List<BlogCategoryViewModel>>> GetAllBlogCategories()
        {
            var categories = await _context.BlogCategories.OrderByDescending(x => x.Created).ToListAsync();

            var viewModels = _mapper.Map<List<BlogCategoryViewModel>>(categories);

            return Ok(viewModels);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var blogCategory = await _context.BlogCategories.FindAsync(id);
            if (blogCategory == null)
            {
                return NotFound();
            }
            var categoryVm = _mapper.Map<BlogCategoryViewModel>(blogCategory);

            return Ok(categoryVm);
        }

        [HttpPost("Create")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] BlogCategoryViewModel viewModel)
        {
            try
            {
                viewModel.Created = DateTime.Now;
                viewModel.LastUpdated = DateTime.Now;

                var blogCategory = _mapper.Map<BlogCategory>(viewModel);
                _context.BlogCategories.Add(blogCategory);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpPost("Update")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update([FromBody] BlogCategoryViewModel ViewModel)
        {
            var categoryInDb = await _context.BlogCategories.FindAsync(ViewModel.Id);
            if (categoryInDb == null)
            {
                return NotFound();
            }

            categoryInDb.Title = ViewModel.Title;
            categoryInDb.LastUpdated = DateTime.Now;
            _context.Update(categoryInDb);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("Delete/{Id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int Id)
        {
            var categoryInDb = await _context.BlogCategories.FindAsync(Id);
            if (categoryInDb == null)
            {
                return NotFound();
            }

            _context.BlogCategories.Remove(categoryInDb);
            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}
