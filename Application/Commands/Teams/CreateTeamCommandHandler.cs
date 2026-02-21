using Application.Abstractions;
using Application.Common;
using Domain.Entities;

namespace Application.Commands.Teams;

public class CreateTeamCommandHandler
{
    private readonly ITeamRepository _teamRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTeamCommandHandler(
        ITeamRepository teamRepository,
        IUnitOfWork unitOfWork)
    {
        _teamRepository = teamRepository;
        _unitOfWork = unitOfWork;
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

        return Result<Guid>.Success(team.Id);
    }
}