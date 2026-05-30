namespace FilmShelf.BAL.Options;

public class AzureOpenAiSettings
{
    public string Endpoint { get; set; } = null!;
    public string DeploymentName { get; set; } = null!;
    public string ChatDeploymentName { get; set; } = "gpt-5.3-chat";
    public string ApiKey { get; set; } = null!;
}
