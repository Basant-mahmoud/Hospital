using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.PaymentDTOs
{
    public class BillingData
    {
        public string first_name { get; set; } = "";
        public string last_name { get; set; } = "";
        public string email { get; set; } = "";
        public string phone_number { get; set; } = "";
        public string apartment { get; set; } = "NA";
        public string floor { get; set; } = "NA";
        public string street { get; set; } = "NA";
        public string building { get; set; } = "NA";
        public string shipping_method { get; set; } = "NA";
        public string postal_code { get; set; } = "00000";
        public string city { get; set; } = "Cairo";
        public string country { get; set; } = "EG";
        public string state { get; set; } = "Cairo";
    }
}
