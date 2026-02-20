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
        public DateTime MatchDate { get; private set; }
        public bool IsPlayed { get; private set; }

        private Match() { }

        public Match(Guid homeTeamId, Guid awayTeamId, DateTime matchDate)
        {
            Id = Guid.NewGuid();
            HomeTeamId = homeTeamId;
            AwayTeamId = awayTeamId;
            MatchDate = matchDate;
            IsPlayed = false;
        }

        public void MarkAsPlayed()
        {
            IsPlayed = true;
        }
    }
}
