namespace BookStoreAPIs.Models
{
    public class Cart
    {
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
