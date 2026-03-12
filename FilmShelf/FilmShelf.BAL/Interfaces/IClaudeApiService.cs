namespace FilmShelf.BAL.Interfaces;

public interface IClaudeApiService
{
    Task<string> SendMessageAsync(string systemPrompt, string userMessage);
}
