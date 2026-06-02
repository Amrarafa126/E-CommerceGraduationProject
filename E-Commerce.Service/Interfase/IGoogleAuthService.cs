namespace E_Commerce.Service.Interfase
{
    public record GoogleUserInfo(string Email, string? FirstName, string? LastName, string? PictureUrl);

    public interface IGoogleAuthService
    {
        Task<GoogleUserInfo> ValidateIdTokenAsync(string idToken);
    }
}
