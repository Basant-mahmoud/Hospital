using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.ChatBot
{
    public class ChatRequest
    {
        public string SessionId { get; set; } = string.Empty;
        public string Question { get; set; } = string.Empty;
    }
}
