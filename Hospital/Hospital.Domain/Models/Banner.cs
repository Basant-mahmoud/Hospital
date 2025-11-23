using Hospital.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Domain.Models
{
    public class Banner
    {
        [Key] public int BannerId { get; set; }
        [Required, StringLength(150)] public string Title { get; set; } = null!;
        [Url, StringLength(300)] public string? ImageURL { get; set; }
        [Url, StringLength(300)] public string? LinkURL { get; set; }
        public BannerType? Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
       
    }
}
