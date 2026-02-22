using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions
{
    public interface IMatchGoalRepository
    {
        Task AddAsync(MatchGoal matchGoal);
    }
}
