using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common;
using Application.Queries.Teams;

namespace Application.Abstractions;

/// <summary>
/// Contrato para consultas de lectura usando Dapper.
/// Application define QUÉ necesita.
/// Infrastructure decide CÓMO hacerlo.
/// </summary>
public interface ITeamQueryRepository
{
    Task<PagedResult<TeamResponse>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search,
        string? sortBy,
        string? sortDirection);
}