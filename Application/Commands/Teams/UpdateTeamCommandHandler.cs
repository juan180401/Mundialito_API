using Application.Abstractions;
using Application.Common;
using Domain;

namespace Application.Commands.Teams;

public class UpdateTeamCommandHandler
{
    private readonly ITeamRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTeamCommandHandler(
        ITeamRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateTeamCommand command)
    {
        var team = await _repository.GetByIdAsync(command.Id);

        if (team is null)
            return Result.Failure("Equipo no encontrado");

        team.UpdateName(command.Name);

        await _unitOfWork.CommitAsync();

        return Result.Success();
    }
}