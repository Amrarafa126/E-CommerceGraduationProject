namespace E_Commerce.Service.Shipping
{
    public class BostaOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://api.bosta.co";
        public bool IsTestMode { get; set; } = true;
    }
}
