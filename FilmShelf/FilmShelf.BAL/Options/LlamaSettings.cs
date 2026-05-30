namespace FilmShelf.BAL.Options;

public class LlamaSettings
{
    public string BaseUrl { get; set; } = "http://localhost:11434";
    public string Model { get; set; } = "llama3.2";
    public bool UseAzureOpenAi { get; set; } = false;
}
