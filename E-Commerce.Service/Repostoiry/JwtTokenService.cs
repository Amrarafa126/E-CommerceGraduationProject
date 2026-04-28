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
        private readonly string _secret = config["JwtSettings:secret"]
            ?? throw new InvalidOperationException("JWT Secret is not configured.");
        private readonly string _issuer = config["JwtSettings:issuer"] ?? "B2BMarketplace";
        private readonly string _audience = config["JwtSettings:audience"] ?? "B2BMarketplace";
        private readonly int _expiryMinutes = int.Parse(config["JwtSettings:ExpiryMinutes"] ?? "60");

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
            new(ClaimTypes.GivenName,          user.FirstName),
            new(ClaimTypes.Surname,            user.LastName),

            // Custom domain claims — read by ICurrentUserService
            new("owned_company_id",    user.OwnedCompanyId?.ToString()    ?? string.Empty),
        };

            // One ClaimTypes.Role per Identity role
            // Supports multi-role scenarios (e.g., Seller is also an Admin)
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_expiryMinutes),
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

        /// <summary>
        /// Validates signature of an expired token and returns its ClaimsPrincipal.
        /// Used during token refresh — lifetime validation is intentionally skipped.
        /// </summary>
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
