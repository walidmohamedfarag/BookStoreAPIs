namespace BookStoreAPIs.DTOs.Response
{
    public class CategoryResponse
    {
        public string CategoryName { get; set; } = string.Empty;
        public List<BookDTOs> Books { get; set; } = new List<BookDTOs>();
    }
}
