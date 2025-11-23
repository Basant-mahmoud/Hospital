using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Domain.Models
{
    public class PasswordResetCode
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string Code { get; set; }
        public DateTime ExpireAt { get; set; }
    }
}
