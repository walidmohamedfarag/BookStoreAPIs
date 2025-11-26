namespace BookStoreAPIs.DTOs.Request
{
    public class UpdateProfileRequest
    {
        public string FisrtName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}
