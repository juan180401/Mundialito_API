using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Players;
public class CreatePlayerCommand
{
    public string Name { get; set; } = string.Empty;
    public Guid TeamId { get; set; }
}