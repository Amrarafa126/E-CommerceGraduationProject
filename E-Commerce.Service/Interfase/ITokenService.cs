using E_Commerce.Data.Identity;
using System.Security.Claims;


namespace E_Commerce.Service.Interfase
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
