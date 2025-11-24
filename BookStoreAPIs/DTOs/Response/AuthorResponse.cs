namespace BookStoreAPIs.DTOs.Response
{
    public class AuthorResponse
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string AuthorImage { get; set; } = string.Empty;
        public List<BookDTOs> Books { get; set; } = new();

    }
}
