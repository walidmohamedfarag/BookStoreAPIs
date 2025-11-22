
using Mapster;
using Microsoft.IdentityModel.Tokens;
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

        public BooksController(IReposatory<Book> _bookRepo, IReposatory<Author> _authorRepo, IReposatory<Category> _categoryRepo)
        {
            bookRepo = _bookRepo;
            authorRepo = _authorRepo;
            categoryRepo = _categoryRepo;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBookRequest createBook, CreateAuthorRequest createAuthor)
        {
            #region create author
            var author = createAuthor.Adapt<Author>();
            if (createAuthor.Image is not null && createAuthor.Image.Length > 0)
            {
                var authorImageName = Guid.NewGuid().ToString() + Path.GetExtension(createAuthor.Image.FileName);
                var authorImagePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\AuthorImage", authorImageName);
                using (var stream = System.IO.File.Create(authorImagePath))
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
            var books = await bookRepo.GetAllAsync(includes: [a => a.Author, c => c.Category]);
            return Ok(books);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var book = await bookRepo.GetOneAsync(b => b.Id == id, includes: [a => a.Author, c => c.Category]);
            if (book is null)
                return NotFound();
            return Ok(book);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Edite(int id, EditeBookRequest editeBook)
        {
            var bookInDB = await bookRepo.GetOneAsync(b => b.Id == id, includes: [c => c.Category]);
            if (bookInDB is null)
                return NotFound();
            // if there is new image for book update it 
            if (editeBook.BookImage is not null && editeBook.BookImage.Length > 0)
            {
                // delete old image from wwwroot folder
                if (bookInDB.BookImage is not null)
                {
                    var oldPathImage = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Imagebook", bookInDB.BookImage);
                    if (System.IO.File.Exists(oldPathImage))
                        System.IO.File.Delete(oldPathImage);
                }
                var nameImage = Guid.NewGuid().ToString() + Path.GetExtension(editeBook.BookImage.FileName);
                var pathImage = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Imagebook", nameImage);
                using (var straem = System.IO.File.Create(pathImage))
                {
                    editeBook.BookImage.CopyTo(straem);
                }
                bookInDB.BookImage = nameImage;
            }
            // update category name if changed 
            var category = await categoryRepo.GetOneAsync(c => c.Id == bookInDB.CategoryId);
            if (category is not null && !editeBook.CategoryName.IsNullOrEmpty())
            {
                category.CategoryName = editeBook.CategoryName;
                await categoryRepo.CommitAsync();
            }
            if (!editeBook.Title.IsNullOrEmpty())
                bookInDB.Title = editeBook.Title;
            if (!editeBook.Description.IsNullOrEmpty())
                bookInDB.Description = editeBook.Description;
            if (editeBook.Created != default(DateTime))
                bookInDB.Created = editeBook.Created;
            if (editeBook.Price != default(decimal))
                bookInDB.Price = editeBook.Price;
            if (editeBook.Discount != default(double))
                bookInDB.Discount = editeBook.Discount;
            await bookRepo.CommitAsync();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var bookInDB = await bookRepo.GetOneAsync(b => b.Id == id);
            if (bookInDB is null)
                return NotFound();
            if (!bookInDB.BookImage.IsNullOrEmpty())
            {
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Imagebook", bookInDB.BookImage!);
                if (System.IO.File.Exists(oldPath))
                    System.IO.File.Delete(oldPath);
            }
            bookRepo.Delete(bookInDB);
            await bookRepo.CommitAsync();
            return NoContent();
        }
    }
}
