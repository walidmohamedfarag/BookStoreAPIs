using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStoreAPIs.DataAccess.EntityTypeConfigration
{
    public class CartConfigration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.HasKey(key => new { key.BookId , key.UserId});
        }
    }
}
