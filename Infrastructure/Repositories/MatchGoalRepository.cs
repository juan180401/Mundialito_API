using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories
{
    public class MatchGoalRepository : IMatchGoalRepository
    {
        private readonly AppDbContext _context;

        public MatchGoalRepository(AppDbContext context)
        {
            _context = context;
        }

        // Agrega registro histórico de goles por partido
        public async Task AddAsync(MatchGoal matchGoal)
        {
            await _context.MatchGoals.AddAsync(matchGoal);
        }
    }
}
