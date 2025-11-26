using BookStoreAPIs.DTOs.Request;
using BookStoreAPIs.DTOs.Response;
using Mapster;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;
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
        public async Task<IActionResult> Create([FromForm] CreateBookFullRequest bookFullRequest)
        {



            // check if author is exsit in db or not
            Author author;
            var authorInDb = await authorRepo.GetOneAsync(a => a.Name.ToLower() == bookFullRequest.CreateAuthor.Name.ToLower());
            // if exist assign its id to book author id 
            if (authorInDb is not null)
                author = authorInDb;
            else
            {
                string authorImageName = string.Empty;

                if (bookFullRequest.CreateAuthor.Image is not null && bookFullRequest.CreateAuthor.Image.Length > 0)
                {
                    authorImageName = Guid.NewGuid().ToString() + Path.GetExtension(bookFullRequest.CreateAuthor.Image.FileName);
                    var authorImagePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\AuthorImage", authorImageName);
                    using (var stream = System.IO.File.Create(authorImagePath))
                    {
                        bookFullRequest.CreateAuthor.Image.CopyTo(stream);
                    }
                }
                author = new Author()
                {
                    Name = bookFullRequest.CreateAuthor.Name.TrimMoreThanOneSpace(),
                    BirthDate = bookFullRequest.CreateAuthor.BirthDate,
                    Age = DateTime.Now.Year - bookFullRequest.CreateAuthor.BirthDate.Year,
                    AuthorImage = authorImageName
                };
            }


            // check if category is exist in db or not 
            Category category;
            var categoryInDb = await categoryRepo.GetOneAsync(c => c.CategoryName.ToLower() == bookFullRequest.CreateBook.CategoryName.ToLower());
            // if exist assign its id to book category id
            if (categoryInDb is not null)
                category = categoryInDb;
            else
            {
                // else create new category and assign its id to book category id
                category = new Category()
                {
                    CategoryName = bookFullRequest.CreateBook.CategoryName.Trim()
                };
            }

            // save book image to wwwroot folder 
            Book book;
            string imgMame = string.Empty;
            if (bookFullRequest.CreateBook.BookImage is not null && bookFullRequest.CreateBook.BookImage.Length > 0)
            {
                imgMame = Guid.NewGuid().ToString() + Path.GetExtension(bookFullRequest.CreateBook.BookImage.FileName);
                var imgPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Imagebook", imgMame);
                using (var stream = System.IO.File.Create(imgPath))
                {
                    await bookFullRequest.CreateBook.BookImage.CopyToAsync(stream);
                }
            }
            book = new Book()
            {
                Title = bookFullRequest.CreateBook.Title,
                Description = bookFullRequest.CreateBook.Description,
                Created = bookFullRequest.CreateBook.Created,
                Price = bookFullRequest.CreateBook.Price,
                Discount = bookFullRequest.CreateBook.Discount,
                PriceAfterDiscount = bookFullRequest.CreateBook.Price - (bookFullRequest.CreateBook.Price * ((decimal)bookFullRequest.CreateBook.Discount / 100)),
                BookImage = imgMame,
                Author = author,
                Category = category,
            };
            author.Books.Add(book);
            category.Books.Add(book);
            await bookRepo.AddAsync(book);
            await bookRepo.CommitAsync();

            return Ok(new
            {
                Success = "Created Book Successfully"
            });
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var booksInDB = await bookRepo.GetAllAsync();
            var books = booksInDB.Adapt<List<BookResponseDTOs>>();
            return Ok(books);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var bookInDB = await bookRepo.GetOneAsync(b => b.Id == id);
            if (bookInDB is null)
                return NotFound();
            var book = bookInDB.Adapt<BookResponseDTOs>();
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
            {
                bookInDB.Price = editeBook.Price;
                bookInDB.PriceAfterDiscount = editeBook.Price - (editeBook.Price * ((decimal)editeBook.Discount / 100));
            }
            if (editeBook.Discount != default(double))
            {
                bookInDB.Discount = editeBook.Discount;
                bookInDB.PriceAfterDiscount = editeBook.Price - (editeBook.Price * ((decimal)editeBook.Discount / 100));
            }
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
