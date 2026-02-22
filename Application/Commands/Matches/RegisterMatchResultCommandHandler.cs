using Application.Abstractions;
using Application.Common;
using Domain.Entities;

namespace Application.Commands.Matches;

public class RegisterMatchResultCommandHandler
{
    private readonly IMatchRepository _matchRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly IMatchGoalRepository _matchGoalRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterMatchResultCommandHandler(
        IMatchRepository matchRepository,
        IPlayerRepository playerRepository,
        IMatchGoalRepository matchGoalRepository,
        IUnitOfWork unitOfWork)
    {
        _matchRepository = matchRepository;
        _playerRepository = playerRepository;
        _matchGoalRepository = matchGoalRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RegisterMatchResultCommand command)
    {
        // Validar que el partido exista
        var match = await _matchRepository.GetByIdAsync(command.MatchId);

        if (match is null)
            return Result.Failure("El partido no existe");

        // Validar que no esté finalizado
        if (match.IsFinished)
            return Result.Failure("El partido ya está finalizado");

        // Validar goles negativos
        if (command.HomeGoals < 0 || command.AwayGoals < 0)
            return Result.Failure("Los goles no pueden ser negativos");

        // Validar que la suma de goles individuales coincida con el resultado global
        var totalScorerGoals = command.Scorers.Sum(s => s.Goals);

        if (totalScorerGoals != command.HomeGoals + command.AwayGoals)
        {
            return Result.Failure("La suma de goles individuales no coincide con el resultado del partido");
        }

        // Procesar cada goleador
        foreach (var scorer in command.Scorers)
        {
            if (scorer.Goals <= 0)
                return Result.Failure("Cantidad de goles inválida");

            var player = await _playerRepository.GetByIdAsync(scorer.PlayerId);

            if (player is null)
                return Result.Failure("Uno de los jugadores no existe");

            // Validar que el jugador pertenezca a alguno de los equipos del partido
            if (player.TeamId != match.HomeTeamId &&
                player.TeamId != match.AwayTeamId)
            {
                return Result.Failure("El jugador no pertenece a este partido");
            }

            // Registrar detalle histórico del gol
            var matchGoal = new MatchGoal(
                command.MatchId,
                scorer.PlayerId,
                scorer.Goals);

            await _matchGoalRepository.AddAsync(matchGoal);

            // Incrementar goles acumulados del jugador
            for (int i = 0; i < scorer.Goals; i++)
                player.AddGoal();
        }

        // 5️⃣ Establecer resultado del partido (regla interna en Domain)
        match.SetResult(command.HomeGoals, command.AwayGoals);

        // 6️⃣ Confirmar transacción completa
        await _unitOfWork.CommitAsync();

        return Result.Success();
    }
}