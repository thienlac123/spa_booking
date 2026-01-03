namespace DoAnSPA.Models
{
    public class ChatAiRequest
    {
        public string Message { get; set; } = "";
    }

    public class ChatAiResponse
    {
        public bool Success { get; set; }
        public string Reply { get; set; } = "";
        public string? Error { get; set; }
    }
}
