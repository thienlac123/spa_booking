using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace DoAnSPA.Services
{
    public class GeminiChatService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;
        private readonly string _model;

        public GeminiChatService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _apiKey = config["Gemini:ApiKey"]
                      ?? throw new Exception("Missing Gemini:ApiKey");
            _model = config["Gemini:Model"] ?? "gemini-2.5-flash";
        }

        public async Task<string> AskAsync(string userMessage)
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";

            var payload = new
            {
                contents = new[]
                {
                    new {
                        role = "user",
                        parts = new[] { new { text = userMessage } }
                    }
                }
            };

            using var resp = await _http.PostAsJsonAsync(url, payload);
            resp.EnsureSuccessStatusCode();

            using var stream = await resp.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);

            // Lấy text đầu tiên: candidates[0].content.parts[0].text
            var root = doc.RootElement;
            var text = root
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return text ?? "";
        }
    }
}
