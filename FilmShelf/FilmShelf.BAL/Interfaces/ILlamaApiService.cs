namespace FilmShelf.BAL.Interfaces;

public interface ILlamaApiService
{
    Task<string> SendMessageAsync(string systemPrompt, string userMessage);
}
