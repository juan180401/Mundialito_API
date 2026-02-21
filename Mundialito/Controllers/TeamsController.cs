using Application.Abstractions;
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
    private readonly ITeamQueryRepository _queryRepository;
    private readonly UpdateTeamCommandHandler _updateHandler;
    private readonly DeleteTeamCommandHandler _deleteHandler;

    /// <summary>
    /// Inyectamos el handler.
    /// El controller NO debe usar DbContext directo.
    /// Solo orquesta.
    /// </summary>
    public TeamsController(
    CreateTeamCommandHandler handler,
    UpdateTeamCommandHandler updateHandler,
    DeleteTeamCommandHandler deleteHandler,
    ITeamQueryRepository queryRepository)
    {
        _handler = handler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _queryRepository = queryRepository;
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

    [HttpGet]
    public async Task<IActionResult> Get(
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
    {
        var result = await _queryRepository.GetPagedAsync(pageNumber, pageSize);

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateTeamCommand command)
    {
        // Forzamos que el Id de la ruta sea el que se use (idempotencia básica)
        if (id != command.Id)
            return BadRequest("El id de la ruta no coincide con el del cuerpo");

        var result = await _updateHandler.Handle(command);

        if (!result.IsSuccess)
            return NotFound(result.Error);

        return NoContent(); // 204 correcto para PUT exitoso
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteTeamCommand { Id = id };

        var result = await _deleteHandler.Handle(command);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return NoContent();
    }
}