using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class MatchRepository : IMatchRepository
    {
        private readonly AppDbContext _context;

        public MatchRepository(AppDbContext context)
        {
            _context = context;
        }

        // Agrega un nuevo partido al contexto (no guarda todavía)
        public async Task AddAsync(Match match)
        {
            await _context.Matches.AddAsync(match);
        }

        // Busca un partido por Id
        public async Task<Match?> GetByIdAsync(Guid id)
        {
            return await _context.Matches
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        //Verificar si equipo no tiene otro partido al mismo tiempo
        public async Task<bool> ExistsMatchAtSameTime(Guid teamId, DateTime matchDate)
        {
            return await _context.Matches.AnyAsync(m =>
                (m.HomeTeamId == teamId || m.AwayTeamId == teamId)
                && m.MatchDate == matchDate);
        }

        // Valida que no exista el mismo enfrentamiento en la misma fecha
        public async Task<bool> ExistsSameMatch(
            Guid homeTeamId,
            Guid awayTeamId,
            DateTime matchDate)
        {
            return await _context.Matches.AnyAsync(m =>
            (
                (m.HomeTeamId == homeTeamId && m.AwayTeamId == awayTeamId) ||
                (m.HomeTeamId == awayTeamId && m.AwayTeamId == homeTeamId)
            )
            && m.MatchDate.Date == matchDate.Date);
        }
    }
}
