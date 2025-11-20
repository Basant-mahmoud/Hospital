namespace Hospital.Application.Interfaces.Payment;
    public class PaymobOptions
    {
        public const string SectionName = "Paymob";

        public string ApiKey { get; set; } = null!;
        public int CardIntegrationId { get; set; }
        public int IframeId { get; set; }
        public string HmacSecret { get; set; } = null!;
    }

