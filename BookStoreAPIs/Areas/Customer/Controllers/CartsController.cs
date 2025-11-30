using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BookStoreAPIs.Areas.Customer.Controllers
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Customer")]
    [Authorize]
    public class CartsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IReposatory<Book> bookRepo;
        private readonly IReposatory<Cart> cartRepo;

        public CartsController(UserManager<ApplicationUser> _userManager, IReposatory<Book> _bookRepo, IReposatory<Cart> _cartRepo)
        {
            userManager = _userManager;
            bookRepo = _bookRepo;
            cartRepo = _cartRepo;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> AddToCart(int id)
        {
            // Get the current user
            var user = await userManager.GetUserAsync(User);
            // Check if the book exists in the database 
            var book = await bookRepo.GetOneAsync(b => b.Id == id, includes: [a => a.Author]);
            // Check if the book is already in the user's cart if yes increase the quantity by 1
            var bookInCart = await cartRepo.GetOneAsync(c => c.BookId == id && c.UserId == user!.Id);
            if (bookInCart is not null)
            {
                bookInCart.Quantity += 1;
                cartRepo.Update(bookInCart);
            }
            else // If the book is not in the cart, add it
            {

                if (book is null) return BadRequest(new { error = "Book Is Not Found" });
                var cart = new Cart
                {
                    BookId = id,
                    UserId = user!.Id,
                    Quantity = 1,
                    Price = book.PriceAfterDiscount
                };
                await cartRepo.AddAsync(cart);
            }
            // Save changes to the database
            await cartRepo.CommitAsync();
            return Ok(new { message = $"Book with ID {id} added to cart for user {user!.UserName}" });
        }
        [HttpGet("GetCart")]
        public async Task<IActionResult> GetCart()
        {
            // Get the current user
            var user = await userManager.GetUserAsync(User);
            // Get all cart items for the user with book and author details
            var cartsInDb = await cartRepo.GetAllAsync(c => c.UserId == user!.Id, includes: [b => b.Book, a => a.Book.Author]);
            // Map cart items to CartResponse DTOs and return them to the client
            var carts = cartsInDb.Select(c => new CartResponse
            {
                AuthorName = c.Book.Author.Name,
                Title = c.Book.Title,
                Description = c.Book.Description,
                BookImage = c.Book.BookImage,
                Quantity = c.Quantity,
                Price = c.Book.Price,
                PriceAfterDiscount = c.Book.PriceAfterDiscount,
                Discount = c.Book.Discount,
            });
            return Ok(carts);
        }
        [HttpPut("{id}/Incerese")]
        public async Task<IActionResult> Incerese(int id)
        {
            var user = await userManager.GetUserAsync(User);
            var cartItem = await cartRepo.GetOneAsync(c => c.BookId == id && c.UserId == user!.Id);
            if(cartItem is null) return BadRequest(new { error = "Cart Item Not Found" });
            cartItem.Quantity += 1;
            cartRepo.Update(cartItem);
            await cartRepo.CommitAsync();
            return Ok();
        }
        [HttpPut("{id}/Decerese")]
        public async Task<IActionResult> Decerese(int id)
        {
            var user = await userManager.GetUserAsync(User);
            var cartItem = await cartRepo.GetOneAsync(c => c.BookId == id && c.UserId == user!.Id);
            if(cartItem is null) return BadRequest(new { error = "Cart Item Not Found" });
            cartItem.Quantity -= 1;
            cartRepo.Update(cartItem);
            await cartRepo.CommitAsync();
            return Ok();
        }
        [HttpDelete("{id}/Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await userManager.GetUserAsync(User);
            var cartItem = await cartRepo.GetOneAsync(c => c.BookId == id && c.UserId == user!.Id);
            if(cartItem is null) return BadRequest(new { error = "Cart Item Not Found" });
            cartRepo.Delete(cartItem);
            await cartRepo.CommitAsync();
            return NoContent();
        }
    }
}
