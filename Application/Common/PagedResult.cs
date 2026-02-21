using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common;

/// <summary>
/// Respuesta estándar paginada.
/// Esto es obligatorio según la prueba.
/// </summary>
public class PagedResult<T>
{
    public IEnumerable<T> Data { get; set; } = new List<T>();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }
}
