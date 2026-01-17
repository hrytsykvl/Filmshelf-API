using FilmShelf.API.VMs;

namespace FilmShelf.API.Hubs;

public interface INotificationHub
{
    Task ReceiveNotification(NotificationVM notification);
    Task ReceiveMovieNotification(MovieNotificationVM movieNotification);
}
