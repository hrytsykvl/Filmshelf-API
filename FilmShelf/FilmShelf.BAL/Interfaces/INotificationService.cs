using FilmShelf.BAL.DTOs;
using FilmShelf.DAL.Entities;

namespace FilmShelf.BAL.Interfaces;

public interface INotificationService
{
    Task<int> CreateReviewNotificationAsync(int userId, int reviewResponseId);
    Task AddNotificationsForAllUsersAsync();
    Task MarkNotificationAsReadAsync(int notificationId);
    Task<List<NotificationDTO>> GetAllNotificationsAsync(int userId);
    Task<NotificationDTO?> GetReviewNotificationAsync(int notificationId);
    Task<int> GetUnreadNotificationsCountAsync(int userId);
    Task DeleteNotificationAsync(int userId, int notificationId);
    Task DeleteAllNotificationsAsync(int userId);
}
