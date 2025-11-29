namespace BookStoreAPIs.DTOs.Request
{
    public class UpdatePasswordRequest
    {
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

    }
}
