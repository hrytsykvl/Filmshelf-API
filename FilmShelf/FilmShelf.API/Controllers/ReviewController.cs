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
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [Authorize]
    public async Task<IActionResult> AddReview([FromBody] ReviewAddVM reviewAddVM)
    {
        var userId = UserClaimsHelper.GetUserId(User);

        var reviewAddDTO = reviewAddVM.ToReviewAddDTO(userId);
        var reviewId = await _reviewService.AddReviewAsync(reviewAddDTO);

        return RedirectToAction(
            nameof(RetrieveReviewById),
            new { id = reviewId });
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

        var reviewResponseId = await _reviewService
            .AddReviewResponseAsync(reviewResponseAddDTO);

        return RedirectToAction(
            nameof(RetrieveReviewResponseById),
            new { id = reviewResponseId });
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
