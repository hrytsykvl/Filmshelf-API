using FilmShelf.API.Hubs;
using FilmShelf.API.MappingExtensions;
using FilmShelf.API.VMs;
using FilmShelf.BAL.DTOs;
using FilmShelf.BAL.Helpers;
using FilmShelf.BAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace FilmShelf.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;
    private readonly INotificationService _notificationService;
    private readonly IHubContext<NotificationHub, INotificationHub> _hubContext;

    public ReviewController(
        IReviewService reviewService,
        IHubContext<NotificationHub, INotificationHub> hubContext,
        INotificationService notificationService)
    {
        _reviewService = reviewService;
        _hubContext = hubContext;
        _notificationService = notificationService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [Authorize]
    public async Task<IActionResult> AddReview([FromBody] ReviewAddVM reviewAddVM)
    {
        var userId = UserClaimsHelper.GetUserId(User);

        var reviewAddDTO = reviewAddVM.ToReviewAddDTO(userId);
        var createdReviewDTO = await _reviewService.AddReviewAsync(reviewAddDTO);

        return CreatedAtAction(
            nameof(RetrieveReviewById),
            new { id = createdReviewDTO.Id },
            createdReviewDTO.ToReviewVM());
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ReviewVM), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RetrieveReviewById(ReviewRequestVM reviewRequestVM)
    {
        var reviewDTO = await _reviewService
            .GetReviewByIdAsync(reviewRequestVM.Id);

        if (reviewDTO == null)
        {
            return NotFound();
        }

        var reviewVM = reviewDTO.ToReviewVM();

        return Ok(reviewVM);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [Authorize]
    public async Task<IActionResult> DeleteReview(ReviewRequestVM reviewRequestVM)
    {
        await _reviewService
            .DeleteReviewAsync(reviewRequestVM.Id);

        return NoContent();
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [Authorize]
    public async Task<IActionResult> UpdateReview(
        ReviewRequestVM reviewRequestVM,
        [FromBody] UpdateReviewRequestVM updateReviewRequestVM)
    {
        await _reviewService.UpdateReviewAsync(
            reviewRequestVM.Id,
            updateReviewRequestVM.Content,
            updateReviewRequestVM.Rating);

        return NoContent();
    }

    [HttpPost("{id}/responses")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [Authorize]
    public async Task<IActionResult> AddReviewResponse(
        ReviewRequestVM reviewRequestVM,
        [FromBody] ReviewResponseAddVM reviewResponseAddVM)
    {
        var userId = UserClaimsHelper.GetUserId(User);

        var reviewResponseAddDTO = reviewResponseAddVM
            .ToReviewResponseAddDTO(reviewRequestVM.Id, userId);

        var createdReviewResponseDTO = await _reviewService
            .AddReviewResponseAsync(reviewResponseAddDTO);

        var notificationId = await _notificationService
            .CreateReviewNotificationAsync(
            createdReviewResponseDTO.ReceiverId,
            createdReviewResponseDTO.Id);

        var notification = await _notificationService
            .GetReviewNotificationAsync(notificationId);

        await _hubContext.Clients
            .User(createdReviewResponseDTO.ReceiverId.ToString())
            .ReceiveNotification(notification?.ToReviewNotificationVM()!);

        return CreatedAtAction(
            nameof(RetrieveReviewResponseById),
            new { id = createdReviewResponseDTO.Id },
            createdReviewResponseDTO.ToReviewResponseVM());
    }

    [HttpGet("responses/{id}")]
    [ProducesResponseType(typeof(ReviewResponseVM), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RetrieveReviewResponseById(
        ReviewResponseRequestVM reviewResponseRequestVM)
    {
        var reviewResponseDTO = await _reviewService
            .GetReviewResponseByIdAsync(reviewResponseRequestVM.Id);

        if (reviewResponseDTO == null)
        {
            return NotFound();
        }

        var reviewResponseVM = reviewResponseDTO.ToReviewResponseVM();

        return Ok(reviewResponseVM);
    }

    [HttpGet("{id}/responses")]
    [ProducesResponseType(typeof(IEnumerable<ReviewResponseVM>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RetrieveResponsesForReview(ReviewRequestVM reviewRequestVM)
    {
        var reviewResponseDTOs = await _reviewService
            .GetReviewResponsesByReviewIdAsync(reviewRequestVM.Id);

        var reviewResponseVMs = reviewResponseDTOs
            .Select(r => r.ToReviewResponseVM());

        return Ok(reviewResponseVMs);
    }

    [HttpDelete("responses/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [Authorize]
    public async Task<IActionResult> RemoveReviewResponse(
        ReviewResponseRequestVM reviewResponseRequestVM)
    {
        var userId = UserClaimsHelper.GetUserId(User);

        await _reviewService
            .DeleteReviewResponseAsync(reviewResponseRequestVM.Id, userId);

        return NoContent();
    }
}
