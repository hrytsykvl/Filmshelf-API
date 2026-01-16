using FilmShelf.API.Hubs;
using FilmShelf.API.MappingExtensions;
using FilmShelf.API.VMs;
using FilmShelf.BAL.Interfaces;
using FilmShelf.BAL.Options;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace FilmShelf.API.Workers;

public class DailyNotificationService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly NotificationSettings _notificationSettings;

    public DailyNotificationService(
        IServiceScopeFactory scopeFactory,
        IOptions<NotificationSettings> notificationSettings)
    {
        _scopeFactory = scopeFactory;
        _notificationSettings = notificationSettings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(
            TimeSpan.FromMinutes(_notificationSettings.DelayInMinutes),
            stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var movieService = scope.ServiceProvider.GetRequiredService<IMovieService>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            var notificationHubContext = scope.ServiceProvider
                    .GetRequiredService<IHubContext<NotificationHub, INotificationHub>>();

            var popularMovies = await movieService.GetPopularMoviesAsync();

            await notificationService
                .AddNotificationsForAllUsersAsync();

            var notification = new MovieNotificationVM
            {
                PopularMovies = popularMovies
                .Select(m => m.ToPopularMovieVM())
                .ToList()
            };

            await notificationHubContext.Clients.All
                    .ReceiveMovieNotification(notification);

            var now = DateTime.Now;
            var nextScheduledTime = CalculateNextScheduledTime(now);
            var delay = nextScheduledTime - now;

            await Task.Delay(delay, stoppingToken);
        }
    }

    private DateTime CalculateNextScheduledTime(DateTime currentTime)
    {
        var scheduledTimeToday = DateTime.Today
            .Add(TimeSpan.FromHours(_notificationSettings.NotificationHour));

        return currentTime < scheduledTimeToday
            ? scheduledTimeToday
            : scheduledTimeToday.AddDays(_notificationSettings.NotificationIntervalDays);
    }
}
