using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Player
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public Guid TeamId { get; private set; }

        private Player() { }

        public Player(string name, Guid teamId)
        {
            Id = Guid.NewGuid();
            Name = name;
            TeamId = teamId;
        }
    }
}
