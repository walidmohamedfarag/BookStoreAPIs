namespace BookStoreAPIs.DTOs.Request
{
    public class TokenApiRequest
    {
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
    }
}
