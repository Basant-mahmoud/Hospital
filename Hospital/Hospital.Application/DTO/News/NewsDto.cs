using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.News
{
    public class NewsDto
    {
        public int NewsId { get; set; }
        [Required, StringLength(150)] public string Title { get; set; } = null!;
        public string? Description { get; set; }
        [Url, StringLength(300)] public string? ImageURL { get; set; }
        public DateTime Date { get; set; }
        public int? BranchId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
