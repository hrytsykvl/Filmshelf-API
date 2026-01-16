using FilmShelf.DAL.Data;
using FilmShelf.DAL.Entities;
using FilmShelf.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FilmShelf.DAL.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly FilmsDbContext _context;

    public NotificationRepository(FilmsDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AnyAsync(Expression<Func<Notification, bool>> predicate)
    {
        return await _context.Notifications.AnyAsync(predicate);
    }

    public async Task CreateNotificationAsync(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
    }

    public async Task CreateNotificationsAsync(List<Notification> notifications)
    {
        await _context.Notifications.AddRangeAsync(notifications);
    }

    public void DeleteNotification(Notification notification)
    {
        _context.Notifications.Remove(notification);
    }

    public async Task<List<Notification>> GetAllNotificationsAsync(int userId)
    {
        var notifications = new List<Notification>();

        var reviewNotifications = await _context.ReviewNotifications
            .Where(n => n.UserId == userId)
            .Include(n => n.ReviewResponse)
                .ThenInclude(r => r.User)
            .Include(n => n.ReviewResponse)
                .ThenInclude(r => r.Review)
                .ThenInclude(r => r.Movie)
            .ToListAsync();

        notifications.AddRange(reviewNotifications);

        var otherNotifications = await _context.Notifications
            .Where(n => n.UserId == userId && !(n is ReviewNotification))
            .ToListAsync();

        notifications.AddRange(otherNotifications);

        return notifications;
    }

    public async Task<ReviewNotification?> GetReviewNotificationById(int notificationId)
    {
        return await _context.ReviewNotifications
            .Where(n => n.Id == notificationId)
            .Include(n => n.ReviewResponse)
                .ThenInclude(r => r.User)
            .Include(n => n.ReviewResponse)
                .ThenInclude(r => r.Review)
                .ThenInclude(r => r.Movie)
            .FirstOrDefaultAsync();
    }

    public async Task<Notification?> GetNotificationById(int notificationId)
    {
        return await _context.Notifications
            .Where(n => n.Id == notificationId)
            .FirstOrDefaultAsync();
    }

    public async Task<int> GetUnreadNotificationsCountAsync(int userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .CountAsync();
    }

    public void DeleteAllNotifications(int userId)
    {
        _context.Notifications
            .RemoveRange(_context.Notifications.Where(n => n.UserId == userId));
    }
}
