namespace BookStoreAPIs.DTOs.Response
{
    public class BookResponseDTOs
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime Created { get; set; }
        public decimal Price { get; set; }
        public double Discount { get; set; }
        public string? BookImage { get; set; }

    }
}
