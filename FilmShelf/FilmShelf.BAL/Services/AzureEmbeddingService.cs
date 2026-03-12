using Azure;
using Azure.AI.OpenAI;
using FilmShelf.BAL.Interfaces;
using FilmShelf.BAL.Options;
using Microsoft.Extensions.Options;
using OpenAI.Embeddings;

namespace FilmShelf.BAL.Services;

public class AzureEmbeddingService : IAzureEmbeddingService
{
    private readonly EmbeddingClient _embeddingClient;

    public AzureEmbeddingService(IOptions<AzureOpenAiSettings> options)
    {
        var settings = options.Value;
        var client = new AzureOpenAIClient(new Uri(settings.Endpoint), new AzureKeyCredential(settings.ApiKey));
        _embeddingClient = client.GetEmbeddingClient(settings.DeploymentName);
    }

    public async Task<float[]> GetEmbeddingAsync(string text)
    {
        var response = await _embeddingClient.GenerateEmbeddingAsync(text);
        return response.Value.ToFloats().ToArray();
    }
}
