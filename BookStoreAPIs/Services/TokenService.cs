using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BookStoreAPIs.Services
{
    public class TokenService : ITokenService
    {

        public string GetAccessToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ghkjjhkjkhjhjkjkjhjkhjhkkjkhjhnnmnmjkdsdsdsds"));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken
                (
                issuer: "https://localhost:7139",
                audience: "https://localhost:7139",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credential
                );
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            return jwtToken;
        }

        public string GetRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        public ClaimsPrincipal ExtractClimFromToken(string token)
        {
            var tokenValidation = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ghkjjhkjkhjhjkjkjhjkhjhkkjkhjhnnmnmjkdsdsdsds"))
            };
            var tokenHandeler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var claims = tokenHandeler.ValidateToken(token, tokenValidation, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken is null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid Token");
            return claims;
        }
    }
}
