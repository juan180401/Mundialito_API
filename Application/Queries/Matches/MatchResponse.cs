using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Matches
{
    public class MatchResponse
    {
        public Guid MatchId { get; set; }
        public string HomeTeamName { get; set; } = string.Empty;
        public string AwayTeamName { get; set; } = string.Empty;
        public int HomeGoals { get; set; }
        public int AwayGoals { get; set; }
        public DateTime MatchDate { get; set; }
        public bool IsFinished { get; set; }
    }
}
