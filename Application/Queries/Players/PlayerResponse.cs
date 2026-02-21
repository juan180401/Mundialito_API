using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Players;
public class PlayerResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid TeamId { get; set; }
    public int Goals { get; set; }
}
