using Application.Abstractions;
using Application.Commands.Players;
using Application.Common;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Matches
{
    public class CreateMatchCommandHandler
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateMatchCommandHandler> _logger;

        public CreateMatchCommandHandler(
            ITeamRepository teamRepository,
            IMatchRepository matchRepository,
            IUnitOfWork unitOfWork,
            ILogger<CreateMatchCommandHandler> logger)
        {
            _teamRepository = teamRepository;
            _matchRepository = matchRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateMatchCommand command)
        {
            // Validación de flujo (Application)
            if (command.HomeTeamId == command.AwayTeamId)
                return Result<Guid>.Failure("Un equipo no puede jugar contra sí mismo");

            // Validamos que ambos equipos existan
            var homeTeam = await _teamRepository.GetByIdAsync(command.HomeTeamId);
            var awayTeam = await _teamRepository.GetByIdAsync(command.AwayTeamId);

            if (homeTeam is null || awayTeam is null)
                return Result<Guid>.Failure("Uno o ambos equipos no existen");

            // Validamos que el equipo no tenga partido en horarios iguales
            var homeConflict = await _matchRepository
                .ExistsMatchAtSameTime(command.HomeTeamId, command.MatchDate);

            var awayConflict = await _matchRepository
                .ExistsMatchAtSameTime(command.AwayTeamId, command.MatchDate);

            if (homeConflict || awayConflict)
                return Result<Guid>.Failure("Uno de los equipos ya tiene partido en ese horario");

            // Validamos que no exista el mismo partido duplicado en la misma fecha
            var sameMatchExists = await _matchRepository
                .ExistsSameMatch(
                    command.HomeTeamId,
                    command.AwayTeamId,
                    command.MatchDate);

            if (sameMatchExists)
                return Result<Guid>.Failure("El partido ya está programado en esa fecha");


            // Creamos el Match (reglas internas protegidas en Domain)
            var match = new Match(
                command.HomeTeamId,
                command.AwayTeamId,
                command.MatchDate);

            await _matchRepository.AddAsync(match);
            _logger.LogInformation(
            "Partido creado {MatchId} {HomeTeamId} vs {AwayTeamId} {MatchDate}",
            match.Id,
            match.HomeTeamId,
            match.AwayTeamId,
            match.MatchDate);
            // Confirmamos transacción
            await _unitOfWork.CommitAsync();

            return Result<Guid>.Success(match.Id);
        }
    }
}
