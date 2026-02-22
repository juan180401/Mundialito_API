using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Goleadores
{
    public class TopScorerResponse
    {
        public Guid PlayerId { get; set; }
        public string PlayerName { get; set; }
        public string TeamName { get; set; }
        public int Goals { get; set; }
    }
}
