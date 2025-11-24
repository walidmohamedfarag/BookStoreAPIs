namespace BookStoreAPIs.DTOs.Response
{
    public class CategoryResponse
    {
        public string CategoryName { get; set; } = string.Empty;
        public List<BookResponseDTOs> Books { get; set; } = new List<BookResponseDTOs>();
    }
}
