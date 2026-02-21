using Application.Abstractions;
using Application.Common;
using Domain.Entities;

namespace Application.Commands.Teams;

/// <summary>
/// Operación de escritura (Write Side en CQRS) para crear un nuevo equipo.
/// </summary>
public class CreateTeamCommand
{
    public string Name { get; set; }
}
