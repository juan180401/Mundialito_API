using Application.Abstractions;
using Application.Common;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Teams;

public class CreateTeamCommandHandler
{
    private readonly ITeamRepository _teamRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateTeamCommandHandler> _logger;

    public CreateTeamCommandHandler(
        ITeamRepository teamRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateTeamCommandHandler> logger)
    {
        _teamRepository = teamRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Creación del equipo.    
    /// </summary>
    public async Task<Result<Guid>> Handle(CreateTeamCommand command)
    {
        // Validación
        if (string.IsNullOrWhiteSpace(command.Name))
        {
            return Result<Guid>.Failure("El nombre del equipo es obligatorio");
        }

        var team = new Team(command.Name);

        await _teamRepository.AddAsync(team);

        // Centralización
        await _unitOfWork.CommitAsync();

        _logger.LogInformation(
        "Equipo creado {TeamId} {TeamName}",
        team.Id,
        team.Name);

        return Result<Guid>.Success(team.Id);
    }
}