using FilmShelf.API.MappingExtensions;
using FilmShelf.API.VMs;
using FilmShelf.BAL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FilmShelf.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EvaluationController : ControllerBase
{
    private readonly IOfflineEvaluationService _evaluationService;
    private readonly ILogger<EvaluationController> _logger;

    public EvaluationController(
        IOfflineEvaluationService evaluationService,
        ILogger<EvaluationController> logger)
    {
        _evaluationService = evaluationService;
        _logger = logger;
    }

    /// <summary>
    /// Evaluate all recommendation methods using offline leave-one-out metrics.
    /// For each user with at least minReviews ratings, holds out their last-reviewed movie
    /// and checks if it appears in the top-K recommendations.
    /// Note: held-out items remain in the database, so recommenders may see them during training.
    /// </summary>
    [HttpGet("all")]
    [ProducesResponseType(typeof(List<EvaluationResultVM>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> EvaluateAll([FromQuery] EvaluationRequestVM request)
    {
        var dto = request.ToEvaluationRequestDTO();
        var results = await _evaluationService.EvaluateAllMethodsAsync(dto);
        return Ok(results.Select(r => r.ToEvaluationResultVM()).ToList());
    }

    /// <summary>
    /// Evaluate a single recommendation method using offline leave-one-out metrics.
    /// Supported methods: ml, content, user-cf, llm, embedding.
    /// Note: held-out items remain in the database, so recommenders may see them during training.
    /// </summary>
    [HttpGet("{method}")]
    [ProducesResponseType(typeof(EvaluationResultVM), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> EvaluateMethod(string method, [FromQuery] EvaluationRequestVM request)
    {
        var dto = request.ToEvaluationRequestDTO();
        var result = await _evaluationService.EvaluateMethodAsync(method, dto);
        return Ok(result.ToEvaluationResultVM());
    }
}
