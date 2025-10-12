namespace MediaLabAPI.DTOs.OpenAI
{
    public class OpenAiRequestDto
    {
        public string Model { get; set; } = "gpt-4o-mini"; // default model
        public string Prompt { get; set; } = string.Empty; // testo inviato dall'utente
        public int MaxTokens { get; set; } = 100;          // default limite token
    }
}
