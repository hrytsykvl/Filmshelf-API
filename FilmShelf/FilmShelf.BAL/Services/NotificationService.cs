using FilmShelf.BAL.DTOs;
using FilmShelf.BAL.Interfaces;
using FilmShelf.BAL.MappingExtensions;
using FilmShelf.DAL.Entities;
using FilmShelf.DAL.Interfaces;

namespace FilmShelf.BAL.Services;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;

    public NotificationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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

    public async Task DeleteNotificationAsync(int userId, int notificationId)
    {
        var notification = await _unitOfWork.NotificationRepository
            .GetReviewNotificationById(notificationId);

        if (notification == null) return;

        _unitOfWork.NotificationRepository
            .DeleteNotification(notification);
        await _unitOfWork.SaveAsync();
    }

    public async Task<List<ReviewNotificationDTO>> GetReviewNotificationsAsync(int userId)
    {
        var reviewNotifications = await _unitOfWork.NotificationRepository
            .GetReviewNotificationsAsync(userId);

        return reviewNotifications
            .Select(rn => rn.ToReviewNotificationDTO())
            .ToList();
    }

    public async Task<ReviewNotificationDTO?> GetReviewNotificationAsync(int notificationId)
    {
        var reviewNotification = await _unitOfWork.NotificationRepository
            .GetReviewNotificationById(notificationId);

        if (reviewNotification == null) return null;

        return reviewNotification.ToReviewNotificationDTO();
    }

    public async Task<int> GetUnreadNotificationsCountAsync(int userId)
    {
        return await _unitOfWork.NotificationRepository
            .GetUnreadNotificationsCountAsync(userId);
    }

    public async Task MarkNotificationAsReadAsync(int notificationId)
    {
        var notification = await _unitOfWork.NotificationRepository
            .GetReviewNotificationById(notificationId);

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
