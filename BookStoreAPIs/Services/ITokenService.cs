using System.Security.Claims;

namespace BookStoreAPIs.Services
{
    public interface ITokenService
    {
        string GetAccessToken(IEnumerable<Claim> claims);
        string GetRefreshToken();
        ClaimsPrincipal ExtractClimFromToken(string token);
    }
}
