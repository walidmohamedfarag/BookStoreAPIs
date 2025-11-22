namespace BookStoreAPIs.DTOs
{
    public class CreateAuthorRequest
    {
        public string Name { get; set; } = string.Empty;
        public DateOnly BirthDate { get; set; }
        public IFormFile? Image { get; set; }

    }
}
