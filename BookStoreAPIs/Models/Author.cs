using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace BookStoreAPIs.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string BirthDate { get; set; } = string.Empty;
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;
    }
}
