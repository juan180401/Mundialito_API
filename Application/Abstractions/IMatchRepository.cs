using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Abstractions
{
    public interface IMatchRepository
    {
        Task AddAsync(Match match);
        Task<Match?> GetByIdAsync(Guid id);
        Task<bool> ExistsMatchAtSameTime(Guid teamId, DateTime matchDate);
        Task<bool> ExistsSameMatch(Guid homeTeamId, Guid awayTeamId, DateTime matchDate);
    }
}