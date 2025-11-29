namespace BookStoreAPIs.DTOs.Request
{
    public class UpdateProfileRequest
    {
        public string FisrtName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}
