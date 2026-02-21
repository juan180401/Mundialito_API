using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Teams;

/// <summary>
/// Representa la intención de obtener equipos.
/// En lectura solo pedimos datos.
/// </summary>
public class GetTeamsQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}