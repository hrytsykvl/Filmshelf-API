namespace FilmShelf.BAL.Interfaces;

public interface IAzureEmbeddingService
{
    Task<float[]> GetEmbeddingAsync(string text);
}
