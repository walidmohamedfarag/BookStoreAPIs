using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BookStoreAPIs.Areas.Admin.Controllers
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Admin")]
    public class AuthorsController : ControllerBase
    {
        private readonly IReposatory<Author> authorReposatory;

        public AuthorsController(IReposatory<Author> _authorReposatory)
        {
            authorReposatory = _authorReposatory;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var authorsInDB = await authorReposatory.GetAllAsync(includes: [a => a.Books]);
            if(authorsInDB is null)
                return NotFound();
            var authors = authorsInDB.Adapt<List<AuthorResponse>>();
            return Ok(authors);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var authorInDB = await authorReposatory.GetOneAsync(a=>a.Id == id , includes: [a=>a.Books]);
            if (authorInDB is null)
                return NotFound();
            var author = authorInDB.Adapt<AuthorResponse>();
            return Ok(author);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var authorInDB = await authorReposatory.GetOneAsync(a => a.Id == id, includes: [a => a.Books]);
            if(authorInDB is null)
                return NotFound();
            // delete author image from wwwroot folder
            var pathImage = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\AuthorImage", authorInDB.AuthorImage);
            if (System.IO.File.Exists(pathImage))
                System.IO.File.Delete(pathImage);
            authorReposatory.Delete(authorInDB);
            await authorReposatory.CommitAsync();
            return NoContent();
        }
    }
}
