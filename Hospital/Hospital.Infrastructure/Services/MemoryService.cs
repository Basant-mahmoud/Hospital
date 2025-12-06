using Hospital.Application.Interfaces.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Infrastructure.Services
{
    public class MemoryService : IMemoryService
    {
        private readonly ConcurrentDictionary<string, List<string>> _memory = new();

        public void AddMessage(string sessionId, string message)
        {
            _memory.AddOrUpdate(
                sessionId,
                new List<string> { message },
                (_, existing) =>
                {
                    existing.Add(message);

                    // limit to last 10 messages
                    if (existing.Count > 10)
                        existing.RemoveAt(0);

                    return existing;
                });
        }

        public List<string> GetMessages(string sessionId)
        {
            return _memory.TryGetValue(sessionId, out var messages)
                ? messages
                : new List<string>();
        }
    }
}
