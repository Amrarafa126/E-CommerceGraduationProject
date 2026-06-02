namespace E_Commerce.Service.Payment
{
    public class PaymobOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public string HmacSecret { get; set; } = string.Empty;
        public int IntegrationIdCard { get; set; }
        public int IntegrationIdWallet { get; set; }
        public string IframeId { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://accept.paymob.com";
        public bool IsTestMode { get; set; } = true;
    }
}
