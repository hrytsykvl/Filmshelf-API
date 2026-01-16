using FilmShelf.API.MappingExtensions;
using FilmShelf.API.VMs;
using FilmShelf.BAL.DTOs;
using FilmShelf.BAL.Helpers;
using FilmShelf.BAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FilmShelf.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<NotificationVM>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RetrieveAllNotifications()
    {
        var userId = UserClaimsHelper.GetUserId(User);

        var notificationsDTO = await _notificationService
            .GetAllNotificationsAsync(userId);

        var notificationsVM = notificationsDTO
            .Select(dto => dto.ToNotificationVM())
            .ToList();

        return Ok(notificationsVM);
    }

    [HttpGet("unread")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUnreadNotificationsCount()
    {
        var userId = UserClaimsHelper.GetUserId(User);

        var unreadNotificationsCount = await _notificationService
            .GetUnreadNotificationsCountAsync(userId);

        return Ok(unreadNotificationsCount);
    }

    [HttpPut("{id}/read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarkNotificationAsRead(NotificationRequestVM notificationRequestVM)
    {
        await _notificationService
            .MarkNotificationAsReadAsync(notificationRequestVM.Id);

        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteNotification(NotificationRequestVM notificationRequestVM)
    {
        var userId = UserClaimsHelper.GetUserId(User);
        await _notificationService
            .DeleteNotificationAsync(userId, notificationRequestVM.Id);

        return NoContent();
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAllNotifications()
    {
        var userId = UserClaimsHelper.GetUserId(User);
        await _notificationService
            .DeleteAllNotificationsAsync(userId);

        return NoContent();
    }
}
