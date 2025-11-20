using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.PaymentDTOs
{
    public class PaymobPaymentStatusDto
    {
        public long OrderId { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public bool Success { get; set; }
    }
}
