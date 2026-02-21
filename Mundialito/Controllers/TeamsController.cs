using Application.Commands.Teams;
using Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace Mundialito.Controllers;

/// <summary>
/// Este controller expone endpoints HTTP.
/// Es la puerta de entrada desde el mundo exterior.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TeamsController : ControllerBase
{
    private readonly CreateTeamCommandHandler _handler;

    /// <summary>
    /// Inyectamos el handler.
    /// El controller NO debe usar DbContext directo.
    /// Solo orquesta.
    /// </summary>
    public TeamsController(CreateTeamCommandHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Endpoint para crear un equipo.
    /// HTTP POST → creación.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(CreateTeamCommand command)
    {
        var result = await _handler.Handle(command);

        // Si algo falló devolvemos 400
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        // Si todo salió bien devolvemos 201 Created
        return CreatedAtAction(nameof(Create), new { id = result.Value }, result.Value);
    }
}