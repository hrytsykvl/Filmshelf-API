namespace FilmShelf.BAL.Options;

public class AzureSearchSettings
{
    public string Endpoint { get; set; } = null!;
    public string IndexName { get; set; } = "movies";
    public int VectorDimensions { get; set; } = 1536;
    public string AdminApiKey { get; set; } = null!;
    public string QueryApiKey { get; set; } = null!;
}
