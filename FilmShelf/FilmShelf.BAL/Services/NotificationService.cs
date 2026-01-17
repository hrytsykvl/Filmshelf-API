using FilmShelf.BAL.DTOs;
using FilmShelf.BAL.Interfaces;
using FilmShelf.BAL.MappingExtensions;
using FilmShelf.DAL.Entities;
using FilmShelf.DAL.Identity;
using FilmShelf.DAL.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace FilmShelf.BAL.Services;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public NotificationService(
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<int> CreateReviewNotificationAsync(int userId, int reviewResponseId)
    {
        var reviewNotification = new ReviewNotification
        {
            UserId = userId,
            ReviewResponseId = reviewResponseId,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        await _unitOfWork.NotificationRepository
            .CreateNotificationAsync(reviewNotification);
        await _unitOfWork.SaveAsync();
        return reviewNotification.Id;
    }

    public async Task AddNotificationsForAllUsersAsync()
    {
        var today = DateTime.UtcNow.Date;
        var notificationsExist = await _unitOfWork.NotificationRepository
            .AnyAsync(n => n.CreatedAt.Date == today);

        if (notificationsExist)
        {
            return;
        }

        var notifications = _userManager.Users
            .Select(u => u.Id)
            .Select(userId => new Notification
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            })
            .ToList();

        await _unitOfWork.NotificationRepository
            .CreateNotificationsAsync(notifications);
        await _unitOfWork.SaveAsync();
    }

    public async Task DeleteNotificationAsync(int userId, int notificationId)
    {
        var notification = await _unitOfWork.NotificationRepository
            .GetNotificationById(notificationId);

        if (notification == null) return;

        _unitOfWork.NotificationRepository
            .DeleteNotification(notification);
        await _unitOfWork.SaveAsync();
    }

    public async Task<List<NotificationDTO>> GetAllNotificationsAsync(int userId)
    {
        var notifications = await _unitOfWork.NotificationRepository
            .GetAllNotificationsAsync(userId);

        return notifications.Select(n => n is ReviewNotification reviewNotification
            ? reviewNotification.ToNotificationDTO()
            : n.ToNotificationDTO())
            .ToList();
    }

    public async Task<NotificationDTO?> GetReviewNotificationAsync(int notificationId)
    {
        var reviewNotification = await _unitOfWork.NotificationRepository
            .GetReviewNotificationById(notificationId);

        return reviewNotification?.ToNotificationDTO();
    }

    public async Task<int> GetUnreadNotificationsCountAsync(int userId)
    {
        return await _unitOfWork.NotificationRepository
            .GetUnreadNotificationsCountAsync(userId);
    }

    public async Task MarkNotificationAsReadAsync(int notificationId)
    {
        var notification = await _unitOfWork.NotificationRepository
            .GetNotificationById(notificationId);

        if (notification == null) return;

        notification.IsRead = true;

        await _unitOfWork.SaveAsync();
    }

    public async Task DeleteAllNotificationsAsync(int userId)
    {
        _unitOfWork.NotificationRepository
            .DeleteAllNotifications(userId);
        await _unitOfWork.SaveAsync();
    }
}
