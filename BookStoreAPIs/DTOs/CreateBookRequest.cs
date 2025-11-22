namespace BookStoreAPIs.DTOs
{
    public class CreateBookRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime Created { get; set; }
        public decimal Price { get; set; }
        public double Discount { get; set; }
        public IFormFile? BookImage { get; set; } 

    }
}
