using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.PaymentDTOs
{
    public class PaymobCallbackDto
    {
        
        [JsonPropertyName("paymentId")]
        public string PaymentId { get; set; }

        [JsonPropertyName("orderId")]
        public string OrderId { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("transactionTime")]
        public string TransactionTime { get; set; }
    }
}
