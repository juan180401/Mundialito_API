using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Queries.Goleadores;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Queries
{
    public class TopScorerQueryRepository
    {
        private readonly string _connectionString;

        public TopScorerQueryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Obtiene goleadores ordenados por goles descendente
        public async Task<IEnumerable<TopScorerResponse>> GetTopScorersAsync()
        {
            using var connection = new SqlConnection(_connectionString);

            var sql = @"
                SELECT 
                    p.Id AS PlayerId,
                    p.Name AS PlayerName,
                    t.Name AS TeamName,
                    p.Goals
                FROM Players p
                INNER JOIN Teams t ON t.Id = p.TeamId
                WHERE p.Goals > 0
                ORDER BY p.Goals DESC
            ";

            var result = await connection.QueryAsync<TopScorerResponse>(sql);

            return result;
        }
    }
}
