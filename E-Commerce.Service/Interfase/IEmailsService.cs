namespace E_Commerce.Service.Interfase
{
    public interface IEmailsService
    {
        Task<string> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true);
    }
}
