using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.PaymentDTOs
{
    public class PaymobPaymentKeyRequest
    {
        public string auth_token { get; set; } = null!;
        public int amount_cents { get; set; }
        public string currency { get; set; } = "EGP";
        public long order_id { get; set; }
        public int integration_id { get; set; }
        public BillingData billing_data { get; set; } = null!;
    }
}
