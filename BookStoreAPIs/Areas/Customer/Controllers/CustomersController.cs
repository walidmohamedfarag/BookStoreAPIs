using System.Threading.Tasks;

namespace BookStoreAPIs.Areas.Customer.Controllers
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Customer")]
    public class CustomersController : ControllerBase
    {
        private readonly IReposatory<Book> bookRepo;
        private readonly IReposatory<Category> categoryRepo;
        private readonly IReposatory<Author> authorRepo;

        public CustomersController(IReposatory<Book> _bookRepo , IReposatory<Category> _categoryRepo , IReposatory<Author> _authorRepo)
        {
            bookRepo = _bookRepo;
            categoryRepo = _categoryRepo;
            authorRepo = _authorRepo;
        }
        [HttpGet("GetAllBook/{categoryId}")]
        public async Task<IActionResult> GetAllBook(int? categoryId , int page = 1)
        {
            var books = await bookRepo.GetAllAsync(includes: [a=>a.Author , c=>c.Category]);
            if (categoryId is not null)
                books = await bookRepo.GetAllAsync(c => c.CategoryId == categoryId, includes: [a => a.Author, c => c.Category]);
            var categories = await categoryRepo.GetAllAsync();
            var currentPage = page;
            var totalPages = Math.Ceiling((double)books.Count() / 4);
            books = books.Skip(0).Take(4);
            return Ok(new
            {
                Books = books,
                Categories = categories,
                CurrentPage = currentPage,
                TotalPages = totalPages
            });
        }
        [HttpGet("GetAuthor/{authorId}")]
        public async Task<IActionResult> GetAuthor(int authorId)
        {
            var author = await authorRepo.GetOneAsync(a => a.Id == authorId);
            if (author is null)
                return NotFound();
            return Ok(author);
        }
    }
}
