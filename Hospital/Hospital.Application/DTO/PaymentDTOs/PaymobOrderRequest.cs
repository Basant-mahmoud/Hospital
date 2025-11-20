using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.PaymentDTOs
{
    public class PaymobOrderRequest
    {
        public string auth_token { get; set; } = null!;
        public string delivery_needed { get; set; } = "false";
        public int amount_cents { get; set; }
        public string currency { get; set; } = "EGP";
        public string merchant_order_id { get; set; } = null!;
        public object[] items { get; set; } = Array.Empty<object>();
    }
}
