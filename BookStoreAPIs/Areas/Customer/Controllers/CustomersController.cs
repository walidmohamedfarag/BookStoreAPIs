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
        [HttpGet("GetAllBook")]
        public async Task<IActionResult> GetAllBook(int? categoryId , int page = 1)
        {
            var booksInDB = await bookRepo.GetAllAsync(includes: [a=>a.Author , c=>c.Category]);
            if (categoryId is not null)
                booksInDB = await bookRepo.GetAllAsync(c => c.CategoryId == categoryId, includes: [a => a.Author, c => c.Category]);
            var currentPage = page;
            var totalPages = Math.Ceiling((double)booksInDB.Count() / 4);
            booksInDB = booksInDB.Skip(0).Take(4);
            var books = booksInDB.Adapt<List<BookResponse>>();
            return Ok(new GetBookResponse
            {
                Books = books,
                CurrentPage = currentPage,
                TotalPages = totalPages
            });
        }
    }
}
