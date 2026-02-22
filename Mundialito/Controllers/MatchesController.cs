using Application.Commands.Matches;
using Infrastructure.Queries;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class MatchesController : ControllerBase
{
    private readonly CreateMatchCommandHandler _handler;
    private readonly RegisterMatchResultCommandHandler _handlerResult;
    private readonly MatchQueryRepository _matchQueryRepository;

    public MatchesController(
        CreateMatchCommandHandler handler, 
        RegisterMatchResultCommandHandler handlerResult,
        MatchQueryRepository matchQueryRepository)
    {
        _handler = handler;
        _handlerResult = handlerResult;
        _matchQueryRepository = matchQueryRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateMatchCommand command)
    {
        var result = await _handler.Handle(command);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return CreatedAtAction(nameof(Create),
            new { id = result.Value },
            result.Value);
    }

    [HttpPost("{id}/result")]
    public async Task<IActionResult> RegisterResult(
    Guid id,
    [FromBody] RegisterMatchResultCommand command)
    {
        // Aseguramos que el id de la ruta sea el mismo del cuerpo
        if (id != command.MatchId)
            return BadRequest("El Id no coincide");

        var result = await _handlerResult.Handle(command);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetMatches(
    int pageNumber = 1,
    int pageSize = 10,
    string? sortBy = "date",
    string? sortDirection = "desc",
    Guid? teamId = null,
    DateTime? date = null,
    bool? isFinished = null)
    {
        var result = await _matchQueryRepository.GetMatchesAsync(
            pageNumber,
            pageSize,
            sortBy,
            sortDirection,
            teamId,
            date,
            isFinished);

        return Ok(result);
    }
}