namespace FilmShelf.BAL.Options;

public class ClaudeSettings
{
    public string ApiKey { get; set; } = null!;
    public string Model { get; set; } = "claude-opus-4-6";
    public int MaxTokens { get; set; } = 4096;
}
