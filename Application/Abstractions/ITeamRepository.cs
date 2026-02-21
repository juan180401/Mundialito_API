using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Abstractions
{
    public interface ITeamRepository
    {
        Task AddAsync(Team team);
        Task<Team?> GetByIdAsync(Guid id);

        void Remove(Team team);
    }
}
