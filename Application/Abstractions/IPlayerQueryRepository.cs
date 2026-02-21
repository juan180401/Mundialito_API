using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common;
using Application.Queries.Players;

namespace Application.Abstractions
{
    public interface IPlayerQueryRepository
    {
        Task<PagedResult<PlayerResponse>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? name,
            Guid? teamId,
            string? sortBy,
            string? sortDirection);
    }
}
