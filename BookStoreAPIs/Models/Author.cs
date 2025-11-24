using BookStoreAPIs.DTOs.Response;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace BookStoreAPIs.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateOnly BirthDate { get; set; }
        public int Age { get; set; }
        public string AuthorImage { get; set; } = string.Empty;
        public List<BookResponseDTOs>? Books { get; set; }
    }
}
