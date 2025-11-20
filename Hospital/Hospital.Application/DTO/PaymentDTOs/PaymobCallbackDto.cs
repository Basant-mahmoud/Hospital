using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.PaymentDTOs
{
    public class PaymobCallbackDto
    {
        public string PaymentId { get; set; }
        public string OrderId { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string TransactionTime { get; set; }
        // Add other fields according to Paymob docs
    }
}
