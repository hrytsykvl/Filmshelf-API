using FilmShelf.API.MappingExtensions;
using FilmShelf.API.VMs;
using FilmShelf.BAL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FilmShelf.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ActorController : ControllerBase
{
    private readonly IActorService _actorService;

    public ActorController(IActorService actorService)
    {
        _actorService = actorService;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ActorDetailsResponseVM), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActorDetailsById(ActorRequestVM actorRequestVM)
    {
        var actor = await _actorService.GetActorDetailsAsync(actorRequestVM.Id, actorRequestVM.Language);

        if (actor == null)
        {
            return NotFound();
        }

        var actorVM = actor.ToActorDetailsResponseVM();

        return Ok(actorVM);
    }
}
