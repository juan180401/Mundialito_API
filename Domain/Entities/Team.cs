using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Team
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }

        private readonly List<Player> _players = new();
        public IReadOnlyCollection<Player> Players => _players;

        private Team() { }

        public Team(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }

        public void UpdateName(string name)
        {
            // Validación básica
            // Se lanza excepción solo para proteger la invariancia interna de la entidad.            
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Nombre inválido");

            Name = name;
        }
    }
}
