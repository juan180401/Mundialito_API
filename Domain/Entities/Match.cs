using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Match
    {
        public Guid Id { get; private set; }
        public Guid HomeTeamId { get; private set; }
        public Guid AwayTeamId { get; private set; }
        public int HomeGoals { get; private set; }
        public int AwayGoals { get; private set; }
        public DateTime MatchDate { get; private set; }        
        public bool IsFinished { get; private set; }

        private Match() { }

        public Match(Guid homeTeamId, Guid awayTeamId, DateTime matchDate)
        {
            if (homeTeamId == awayTeamId)
                throw new ArgumentException("Un equipo no puede jugar contra sí mismo");

            Id = Guid.NewGuid();
            HomeTeamId = homeTeamId;
            AwayTeamId = awayTeamId;
            MatchDate = matchDate;
            IsFinished = false;
        }
        public void SetResult(int homeGoals, int awayGoals)
        {
            if (IsFinished)
                throw new InvalidOperationException("El partido ya está finalizado");

            if (homeGoals < 0 || awayGoals < 0)
                throw new ArgumentException("Los goles no pueden ser negativos");

            HomeGoals = homeGoals;
            AwayGoals = awayGoals;
            IsFinished = true;
        }
    }
}
