using E_Commerce.Data.Helpers;
using E_Commerce.Service.Interfase;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;

namespace E_Commerce.Service.Repostoiry
{
    public class GoogleAuthService(IOptions<GoogleAuthSettings> options) : IGoogleAuthService
    {
        private readonly GoogleAuthSettings _settings = options.Value;

        public async Task<GoogleUserInfo> ValidateIdTokenAsync(string idToken)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _settings.ClientId }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            return new GoogleUserInfo(
                payload.Email,
                payload.GivenName,
                payload.FamilyName,
                payload.Picture);
        }
    }
}
