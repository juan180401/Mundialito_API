using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Teams
{
    public class UpdateTeamCommand
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
