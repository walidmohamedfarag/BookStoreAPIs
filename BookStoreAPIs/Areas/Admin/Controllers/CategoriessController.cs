using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BookStoreAPIs.Areas.Admin.Controllers
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Admin")]
    public class CategoriessController : ControllerBase
    {
        private readonly IReposatory<Category> categoryRepo;

        public CategoriessController(IReposatory<Category> _categoryRepo)
        {
            categoryRepo = _categoryRepo;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categoriesInDB = await categoryRepo.GetAllAsync(includes: [c => c.Books]);
            if (categoriesInDB is null)
                return NotFound();
            var categories = categoriesInDB.Adapt<List<CategoryResponse>>();
            return Ok(categories);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var categoryInDB = await categoryRepo.GetOneAsync(c => c.Id == id, includes: [c => c.Books]);
            if (categoryInDB is null)
                return NotFound();
            var category = categoryInDB.Adapt<CategoryResponse>();
            return Ok(category);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await categoryRepo.GetOneAsync(c => c.Id == id, includes: [c => c.Books]);
            if (category is null)
                return NotFound();
            categoryRepo.Delete(category);
            await categoryRepo.CommitAsync();
            return NoContent();
        }
    }
}
