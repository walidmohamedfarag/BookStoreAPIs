namespace BookStoreAPIs.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Author { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public decimal Price { get; set; }
        public double Discount { get; set; }
    }
}
