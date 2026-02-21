using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Teams;

/// <summary>
/// DTO de salida para lectura.
/// No devolvemos la entidad Domain directamente.
/// </summary>
public class TeamResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}