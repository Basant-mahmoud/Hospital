using Hospital.Application.DTO.ChatBot;
using Hospital.Application.Interfaces.Services;
using Hospital.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryService _memory;

        public ChatController(IHttpClientFactory httpClientFactory, IMemoryService memory)
        {
            _httpClient = httpClientFactory.CreateClient("PythonRagClient");
            _memory = memory;
        }


        [HttpPost("ask")]
        public async Task<ActionResult<ChatResponse>> Ask(ChatRequest request)
        {
            _memory.AddMessage(request.SessionId, request.Question);

            var pythonResponse = await _httpClient.GetFromJsonAsync<ChatResponse>(
                $"ask?session_id={request.SessionId}&question={Uri.EscapeDataString(request.Question)}"
            );

            return pythonResponse!;
        }
    }
}
