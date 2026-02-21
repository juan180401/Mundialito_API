using Application.Abstractions;
using Application.Commands.Players;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayersController : ControllerBase
{
    private readonly CreatePlayerCommandHandler _handler;
    private readonly IPlayerQueryRepository _queryRepository;

    public PlayersController(
        CreatePlayerCommandHandler handler,
        IPlayerQueryRepository queryRepository)
    {
        _handler = handler;
        _queryRepository = queryRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePlayerCommand command)
    {
        var result = await _handler.Handle(command);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return CreatedAtAction(nameof(Create), new { id = result.Value }, result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> Get(
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string? name = null,
    [FromQuery] Guid? teamId = null,
    [FromQuery] string? sortBy = "Name",
    [FromQuery] string? sortDirection = "ASC")
    {
        var result = await _queryRepository.GetPagedAsync(
            pageNumber,
            pageSize,
            name,
            teamId,
            sortBy,
            sortDirection);

        return Ok(result);
    }
}