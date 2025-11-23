using Hospital.Domain.Enum;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.Domain.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        // FK to Appointment (dependent side of 1:1)
        [Required]
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(3)]
        public string Currency { get; set; } = "EGP";

        // Paymob identifiers
        public long? PaymobOrderId { get; set; }
        public long? PaymobTransactionId { get; set; }

        [StringLength(100)]
        public string? PaymobMerchantOrderId { get; set; }

        [Required]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        // Optional raw JSON/body from gateway for debugging
        public string? GatewayResponse { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
