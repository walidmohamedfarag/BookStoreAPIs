using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace BookStoreAPIs.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public int BookId { get; set; }
        public string AuthorImage { get; set; } = string.Empty;
        public Book Book { get; set; } = null!;
    }
}
