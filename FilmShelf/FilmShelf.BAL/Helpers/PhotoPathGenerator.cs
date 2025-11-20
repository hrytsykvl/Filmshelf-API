using FilmShelf.TMDbClient.Options;

namespace FilmShelf.BAL.Helpers;

public static class PhotoPathGenerator
{
    private static TmdbSettings _tmdbSettings = null!;

    public static void Initialize(TmdbSettings tmdbSettings)
    {
        _tmdbSettings = tmdbSettings;
    }

    public static string GeneratePosterPath(string posterPath)
    {
        return $"{_tmdbSettings.Images.BaseUrl}{_tmdbSettings.Images.DefaultPosterSize}{posterPath}";
    }
}