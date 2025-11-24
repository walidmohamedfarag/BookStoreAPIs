namespace BookStoreAPIs.DTOs.Request
{
    public class CreateBookFullRequest
    {
        public CreateBookRequest CreateBook { get; set; } = null!;
        public CreateAuthorRequest CreateAuthor { get; set; } = null!;
    }
}
