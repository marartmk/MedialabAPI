using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediaLabAPI.DTOs.OpenAI;
using MediaLabAPI.Services;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MediaLabAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OpenAiController : ControllerBase
    {
        private readonly IApiKeyService _apiKeyService;
        private readonly HttpClient _httpClient;

        public OpenAiController(IApiKeyService apiKeyService, IHttpClientFactory httpClientFactory)
        {
            _apiKeyService = apiKeyService;
            _httpClient = httpClientFactory.CreateClient();
        }

        // Endpoint esistente per la chat (chiave usata solo lato server)
        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] OpenAiRequestDto request)
        {
            var apiKey = await _apiKeyService.GetKeyAsync("OpenAI");
            if (string.IsNullOrEmpty(apiKey))
                return StatusCode(500, "API Key non configurata o inattiva");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var payload = new
            {
                model = request.Model,
                messages = new[]
                {
                    new { role = "user", content = request.Prompt }
                },
                max_tokens = request.MaxTokens
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);

            var result = await response.Content.ReadAsStringAsync();
            return Content(result, "application/json");
        }

        // Nuovo endpoint per dare al FE la chiave
        [HttpGet("get-key")]
        public async Task<IActionResult> GetKey()
        {
            var apiKey = await _apiKeyService.GetKeyAsync("OpenAI");
            if (string.IsNullOrEmpty(apiKey))
                return NotFound(new { Success = false, Message = "API Key non configurata o inattiva" });

            return Ok(new { Success = true, ApiKey = apiKey });
        }
    }
}
