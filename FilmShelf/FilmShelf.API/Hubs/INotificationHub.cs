using FilmShelf.API.VMs;

namespace FilmShelf.API.Hubs;

public interface INotificationHub
{
    Task ReceiveNotification(ReviewNotificationVM notification);
}
