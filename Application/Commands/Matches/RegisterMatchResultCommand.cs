using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Matches;

public class RegisterMatchResultCommand
{
    public Guid MatchId { get; set; }
    public int HomeGoals { get; set; }
    public int AwayGoals { get; set; }

    // Lista de goleadores con cantidad de goles por jugador
    public List<GoalDto> Scorers { get; set; } = new();
}

public class GoalDto
{
    public Guid PlayerId { get; set; }
    public int Goals { get; set; }
}
