namespace BookStoreAPIs.DTOs
{
    public class CreateAuthorRequest
    {
        public string Name { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public int BookId { get; set; }

    }
}
