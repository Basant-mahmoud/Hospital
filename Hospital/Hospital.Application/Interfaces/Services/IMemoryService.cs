using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.Interfaces.Services
{
    public interface IMemoryService
    {
        void AddMessage(string sessionId, string message);
        List<string> GetMessages(string sessionId);
    }
}
