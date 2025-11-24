namespace BookStoreAPIs.DTOs.Response
{
    public class CategoryResponse
    {
        public string CategoryName { get; set; } = string.Empty;
        public List<CategoryDTOs> Books { get; set; } = new List<CategoryDTOs>();
    }
}
