
using Mapster;
using System.Threading.Tasks;

namespace BookStoreAPIs.Areas.Admin.Controllers
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Admin")]
    public class BooksController : ControllerBase
    {
        private readonly IReposatory<Book> bookRepo;
        private readonly IReposatory<Author> authorRepo;
        private readonly IReposatory<Category> categoryRepo;

        public BooksController(IReposatory<Book> _bookRepo , IReposatory<Author> _authorRepo , IReposatory<Category> _categoryRepo)
        {
            bookRepo = _bookRepo;
            authorRepo = _authorRepo;
            categoryRepo = _categoryRepo;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBookRequest createBook , CreateAuthorRequest createAuthor )
        {
            #region create author
            var author = createAuthor.Adapt<Author>();
            if (createAuthor.Image is not null && createAuthor.Image.Length > 0)
            {
                var authorImageName = Guid.NewGuid().ToString() + Path.GetExtension(createAuthor.Image.FileName);
                var authorImagePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\AuthorImage", authorImageName);
                using(var stream = System.IO.File.Create(authorImagePath))
                {
                    createAuthor.Image.CopyTo(stream);
                }
                author.AuthorImage = authorImageName;
            }
            author.Age = DateTime.Now.Year - createAuthor.BirthDate.Year;
            await authorRepo.AddAsync(author);
            await authorRepo.CommitAsync();
            #endregion

            #region create category
            var category = new Category()
            {
                CategoryName = createBook.CategoryName
            };
            await categoryRepo.AddAsync(category);
            await categoryRepo.CommitAsync();
            #endregion

            #region create book
            //create image of book
            var book = createBook.Adapt<Book>();
            if (createBook.BookImage is not null && createBook.BookImage.Length > 0)
            {
                var imgMame = Guid.NewGuid().ToString() + Path.GetExtension(createBook.BookImage.FileName);
                var imgPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Imagebook", imgMame);
                using (var stream = System.IO.File.Create(imgPath))
                {
                    await createBook.BookImage.CopyToAsync(stream);
                }
                book.BookImage = imgMame;
            }
            book.AuthorId = author.Id;
            book.CategoryId = category.Id;
            await bookRepo.AddAsync(book);
            await bookRepo.CommitAsync();
            #endregion

            return Ok(new
            {
                Success = "Created Book Successfully"
            });
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var books = await bookRepo.GetAllAsync(includes: [a=>a.Author , c=>c.Category]);
            return Ok(books);
        }
    }
}
