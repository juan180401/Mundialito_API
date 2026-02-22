using Application.Commands.Matches;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class MatchesController : ControllerBase
{
    private readonly CreateMatchCommandHandler _handler;
    private readonly RegisterMatchResultCommandHandler _handlerResult;

    public MatchesController(
        CreateMatchCommandHandler handler, 
        RegisterMatchResultCommandHandler handlerResult)
    {
        _handler = handler;
        _handlerResult = handlerResult;
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
}