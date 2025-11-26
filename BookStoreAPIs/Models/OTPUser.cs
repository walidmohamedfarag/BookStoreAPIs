namespace BookStoreAPIs.Models
{
    public class OTPUser
    {
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; }
        public string OTP { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
