using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStoreAPIs.DataAccess.EntityTypeConfigration
{
    public class OTPUserConfigration : IEntityTypeConfiguration<OTPUser>
    {
        public void Configure(EntityTypeBuilder<OTPUser> builder)
        {
            builder.HasKey(k => new { k.OTP, k.UserId });
        }
    }
}
