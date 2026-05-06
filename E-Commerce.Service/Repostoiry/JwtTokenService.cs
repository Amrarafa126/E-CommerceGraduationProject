using E_Commerce.Data.Identity;
using E_Commerce.Service.Interfase;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace E_Commerce.Service.Repostoiry
{
    public class JwtTokenService(IConfiguration config) : ITokenService
    {
        private readonly string _secret = config["JwtSettings:Secret"]
            ?? throw new InvalidOperationException("JWT Secret is not configured.");
        private readonly string _issuer = config["JwtSettings:Issuer"] ?? "B2BMarketplace";
        private readonly string _audience = config["JwtSettings:Audience"] ?? "B2BMarketplace";
        private readonly int _accessTokenExpireMinutes = int.Parse(config["JwtSettings:AccessTokenExpireMinutes"] ?? "60");

        public string GenerateAccessToken(User user, IEnumerable<string> roles)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier,     user.Id.ToString()),
            new(ClaimTypes.Email,              user.Email ?? string.Empty),
            new(ClaimTypes.GivenName,          user.FirstName ?? string.Empty),
            new(ClaimTypes.Surname,            user.LastName ?? string.Empty),

            new("owned_company_id",    user.OwnedCompanyId?.ToString()    ?? string.Empty),
        };

         
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_accessTokenExpireMinutes),
                signingCredentials: cred);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var parameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_secret)),
                ValidateLifetime = false,   // deliberately skip — token IS expired
            };

            try
            {
                var principal = new JwtSecurityTokenHandler()
                    .ValidateToken(token, parameters, out var raw);

                if (raw is not JwtSecurityToken jwt ||
                    !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                        StringComparison.OrdinalIgnoreCase))
                    return null;

                return principal;
            }
            catch { return null; }
        }
    }
}
