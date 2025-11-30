namespace BookStoreAPIs.DTOs.Response
{
    public class CartResponse
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? BookImage { get; set; }
        public string? AuthorName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal PriceAfterDiscount { get; set; }
        public double Discount { get; set; }
        public decimal TotalPrice => Quantity * PriceAfterDiscount;

    }
}
