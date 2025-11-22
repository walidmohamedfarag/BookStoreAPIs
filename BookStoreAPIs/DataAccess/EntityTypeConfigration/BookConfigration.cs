using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStoreAPIs.DataAccess.EntityTypeConfigration
{
    public class BookConfigration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.HasKey(key => key.Id);
            builder.Property(prop => prop.Title)
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(prop=>prop.Description)
                .HasMaxLength(500);
        }
    }
}
