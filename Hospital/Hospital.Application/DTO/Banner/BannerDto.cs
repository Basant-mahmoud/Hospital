using Hospital.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.Banner
{
    public class BannerDto
    {
        public int BannerId { get; set; }
        public string Title { get; set; } = null!;
        public string? ImageURL { get; set; }
        public string? LinkURL { get; set; }

        [EnumDataType(typeof(BannerType))]
        public BannerType? Type { get; set; }
    }
}
