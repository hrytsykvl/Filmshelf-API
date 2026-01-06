using FilmShelf.BAL.DTOs;

namespace FilmShelf.BAL.Interfaces;

public interface INotificationService
{
    Task<int> CreateReviewNotificationAsync(int userId, int reviewResponseId);
    Task MarkNotificationAsReadAsync(int notificationId);
    Task<List<ReviewNotificationDTO>> GetReviewNotificationsAsync(int userId);
    Task<ReviewNotificationDTO?> GetReviewNotificationAsync(int notificationId);
    Task<int> GetUnreadNotificationsCountAsync(int userId);
    Task DeleteNotificationAsync(int userId, int notificationId);
    Task DeleteAllNotificationsAsync(int userId);
}
