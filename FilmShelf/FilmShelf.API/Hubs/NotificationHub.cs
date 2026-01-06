using FilmShelf.API.VMs;
using Microsoft.AspNetCore.SignalR;

namespace FilmShelf.API.Hubs;

public class NotificationHub : Hub<INotificationHub>
{
    public async Task SendNotification(int userId, ReviewNotificationVM notification)
    {
        await Clients.User(userId.ToString())
            .ReceiveNotification(notification);
    }
}
