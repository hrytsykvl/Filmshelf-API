using FilmShelf.BAL.Interfaces;
using FilmShelf.TMDbClient.Options;
using Microsoft.Extensions.Options;

namespace FilmShelf.API.Workers;

public class SyncBackgroundService : BackgroundService
{
    private readonly TmdbSettings _tmdbSettings;
    private readonly IServiceProvider _serviceProvider;

    public SyncBackgroundService(
        IOptions<TmdbSettings> tmdbSettings,
        IServiceProvider serviceProvider)
    {
        _tmdbSettings = tmdbSettings.Value;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var moviePageService = scope.ServiceProvider.GetRequiredService<IMoviePageService>();
                await moviePageService.FetchAndUpdateAsync();
            }

            var now = DateTime.Now;
            var nextScheduledTime = CalculateNextScheduledTime(now);
            var delay = nextScheduledTime - now;
            
            await Task.Delay(delay, stoppingToken);
        }
    }

    private DateTime CalculateNextScheduledTime(DateTime currentTime)
    {
        var scheduledTimeToday = DateTime.Today.Add(TimeSpan.Parse(_tmdbSettings.ScheduledTime));
        return currentTime < scheduledTimeToday 
            ? scheduledTimeToday 
            : scheduledTimeToday.AddDays(TimeSpan.FromHours(_tmdbSettings.IntervalInHours).TotalDays);
    }
}
