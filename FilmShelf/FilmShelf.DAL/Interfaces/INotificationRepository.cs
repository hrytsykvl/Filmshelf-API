using FilmShelf.DAL.Entities;

namespace FilmShelf.DAL.Interfaces;

public interface INotificationRepository
{
    Task CreateNotificationAsync(Notification notification);
    Task<List<ReviewNotification>> GetReviewNotificationsAsync(int userId);
    Task<ReviewNotification?> GetReviewNotificationById(int userId);
    Task<int> GetUnreadNotificationsCountAsync(int userId);
    void DeleteNotification(Notification notification);
    void DeleteAllNotifications(int userId);
}
