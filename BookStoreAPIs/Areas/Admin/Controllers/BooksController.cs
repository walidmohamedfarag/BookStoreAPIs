using BookStoreAPIs.DTOs;
using BookStoreAPIs.Reposatories;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Create(CreateBookRequest createBook)
        {

            return Ok(new
            {
                Success = "Created Book Successfully"
            });
        }
    }
}
