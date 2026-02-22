using Application.Commands.Matches;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class MatchesController : ControllerBase
{
    private readonly CreateMatchCommandHandler _handler;

    public MatchesController(CreateMatchCommandHandler handler)
    {
        _handler = handler;
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
}