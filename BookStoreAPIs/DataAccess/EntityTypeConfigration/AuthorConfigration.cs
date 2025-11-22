using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStoreAPIs.DataAccess.EntityTypeConfigration
{
    public class AuthorConfigration : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder.HasKey(key => key.Id);
            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(p => p.BirthDate)
                .IsRequired();
        }
    }
}
