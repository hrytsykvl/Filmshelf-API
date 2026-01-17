using FilmShelf.API.VMs;
using Microsoft.AspNetCore.SignalR;

namespace FilmShelf.API.Hubs;

public class NotificationHub : Hub<INotificationHub>
{
    public async Task SendNotification(int userId, NotificationVM notification)
    {
        await Clients.User(userId.ToString())
            .ReceiveNotification(notification);
    }

    public async Task SendMovieNotification(MovieNotificationVM movieNotification)
    {
        await Clients.All
            .ReceiveMovieNotification(movieNotification);
    }
}
