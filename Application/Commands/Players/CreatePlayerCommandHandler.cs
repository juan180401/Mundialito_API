using Application.Abstractions;
using Application.Common;
using Domain;
using Domain.Entities;

namespace Application.Commands.Players;

public class CreatePlayerCommandHandler
{
    private readonly ITeamRepository _teamRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePlayerCommandHandler(
        ITeamRepository teamRepository,
        IPlayerRepository playerRepository,
        IUnitOfWork unitOfWork)
    {
        _teamRepository = teamRepository;
        _playerRepository = playerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreatePlayerCommand command)
    {
        // Validación de flujo
        if (string.IsNullOrWhiteSpace(command.Name))
            return Result<Guid>.Failure("Nombre inválido");

        // Coordinación entre entidades
        var team = await _teamRepository.GetByIdAsync(command.TeamId);

        if (team is null)
            return Result<Guid>.Failure("El equipo no existe");

        var player = new Player(command.Name, command.TeamId);

        await _playerRepository.AddAsync(player);

        await _unitOfWork.CommitAsync();

        return Result<Guid>.Success(player.Id);
    }
}
