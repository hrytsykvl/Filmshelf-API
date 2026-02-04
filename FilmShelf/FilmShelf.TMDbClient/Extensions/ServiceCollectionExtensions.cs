using FilmShelf.TMDbClient.Interfaces;
using FilmShelf.TMDbClient.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FilmShelf.TMDbClient.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddTMDbClient(this IServiceCollection services)
    {
        services.AddTransient<IMovieApiIntegrationService, MovieApiIntegrationService>();
    }
}
