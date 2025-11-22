using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStoreAPIs.DataAccess.EntityTypeConfigration
{
    public class CategoryConfigration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}
