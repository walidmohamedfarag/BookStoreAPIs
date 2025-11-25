namespace BookStoreAPIs.DTOs.Response
{
    public class BookResponse
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime Created { get; set; }
        public decimal Price { get; set; }
        public double Discount { get; set; }
        public decimal PriceAfterDiscount { get; set; }
        public string? BookImage { get; set; }
        public GetAuthorResponse Author { get; set; }
        public GetCategoryResponse Category { get; set; }

    }
}
