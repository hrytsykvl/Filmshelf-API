using FilmShelf.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FilmShelf.DAL.Interfaces;

public interface INotificationRepository
{
    Task<bool> AnyAsync(Expression<Func<Notification, bool>> predicate);
    Task CreateNotificationAsync(Notification notification);
    Task CreateNotificationsAsync(List<Notification> notifications);
    Task<List<Notification>> GetAllNotificationsAsync(int userId);
    Task<ReviewNotification?> GetReviewNotificationById(int notificationId);
    Task<Notification?> GetNotificationById(int notificationId);
    Task<int> GetUnreadNotificationsCountAsync(int userId);
    void DeleteNotification(Notification notification);
    void DeleteAllNotifications(int userId);
}
