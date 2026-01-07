using FilmShelf.DAL.Data;
using FilmShelf.DAL.Entities;
using FilmShelf.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FilmShelf.DAL.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly FilmsDbContext _context;

    public NotificationRepository(FilmsDbContext context)
    {
        _context = context;
    }

    public async Task CreateNotificationAsync(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
    }

    public void DeleteNotification(Notification notification)
    {
        _context.Notifications.Remove(notification);
    }

    public async Task<List<ReviewNotification>> GetReviewNotificationsAsync(int userId)
    {
        return await _context.ReviewNotifications
            .Where(n => n.UserId == userId)
            .Include(n => n.ReviewResponse)
                .ThenInclude(r => r.User)
            .Include(n => n.ReviewResponse)
                .ThenInclude(r => r.Review)
                .ThenInclude(r => r.Movie)
            .ToListAsync();
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
