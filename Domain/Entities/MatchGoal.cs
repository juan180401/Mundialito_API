using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class MatchGoal
    {
        public Guid Id { get; private set; }
        public Guid MatchId { get; private set; }
        public Guid PlayerId { get; private set; }
        public int Goals { get; private set; }

        private MatchGoal() { }

        public MatchGoal(Guid matchId, Guid playerId, int goals)
        {
            // Regla interna del dominio:
            // No tiene sentido registrar 0 o goles negativos
            if (goals <= 0)
                throw new ArgumentException("Los goles deben ser mayores a 0");

            Id = Guid.NewGuid();
            MatchId = matchId;
            PlayerId = playerId;
            Goals = goals;
        }
    }
}
