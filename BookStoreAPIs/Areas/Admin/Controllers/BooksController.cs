
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

        public BooksController(IReposatory<Book> _bookRepo)
        {
            bookRepo = _bookRepo;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBookRequest createBook , CreateAuthorRequest createAuthor)
        {
            if(createBook.BookImage is not null && createBook.BookImage.Length > 0)
            {
                var imgMame = Guid.NewGuid().ToString() + Path.GetExtension(createBook.BookImage.FileName);
                var imgPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Imagebook", imgMame);
                using (var stream = System.IO.File.Create(imgPath))
                {
                    await createBook.BookImage.CopyToAsync(stream);
                }
                createBook.ImageName = imgMame;
            }
            var book = createBook.Adapt<Book>();
            await bookRepo.AddAsync(book);
            await bookRepo.CommitAsync();
            return Ok(new
            {
                Success = "Created Book Successfully"
            });
        }
    }
}
