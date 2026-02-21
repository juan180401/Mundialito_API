using Domain;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions
{
    public interface IPlayerRepository
    {
        Task AddAsync(Player player);
        Task<Player?> GetByIdAsync(Guid id);
        void Remove(Player player);
    }
}
