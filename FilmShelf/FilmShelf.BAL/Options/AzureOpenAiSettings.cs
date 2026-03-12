namespace FilmShelf.BAL.Options;

public class AzureOpenAiSettings
{
    public string Endpoint { get; set; } = null!;
    public string DeploymentName { get; set; } = null!;
    public string ApiKey { get; set; } = null!;
}
