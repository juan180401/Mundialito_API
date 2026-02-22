using Application.Queries.Standings;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Queries
{
    public class StandingQueryRepository
    {
        private readonly string _connectionString;

        public StandingQueryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<StandingResponse>> GetStandingsAsync()
        {
            using var connection = new SqlConnection(_connectionString);

            var sql = @"
        SELECT 
            t.Id AS TeamId,
            t.Name AS TeamName,

            COUNT(m.Id) AS Played,

            SUM(CASE 
                WHEN (m.HomeTeamId = t.Id AND m.HomeGoals > m.AwayGoals)
                  OR (m.AwayTeamId = t.Id AND m.AwayGoals > m.HomeGoals)
                THEN 1 ELSE 0 END) AS Won,

            SUM(CASE 
                WHEN m.HomeGoals = m.AwayGoals
                THEN 1 ELSE 0 END) AS Draw,

            SUM(CASE 
                WHEN (m.HomeTeamId = t.Id AND m.HomeGoals < m.AwayGoals)
                  OR (m.AwayTeamId = t.Id AND m.AwayGoals < m.HomeGoals)
                THEN 1 ELSE 0 END) AS Lost,

            SUM(CASE 
                WHEN m.HomeTeamId = t.Id THEN m.HomeGoals
                WHEN m.AwayTeamId = t.Id THEN m.AwayGoals
                ELSE 0 END) AS GoalsFor,

            SUM(CASE 
                WHEN m.HomeTeamId = t.Id THEN m.AwayGoals
                WHEN m.AwayTeamId = t.Id THEN m.HomeGoals
                ELSE 0 END) AS GoalsAgainst

        FROM Teams t
        LEFT JOIN Matches m 
            ON (m.HomeTeamId = t.Id OR m.AwayTeamId = t.Id)
            AND m.IsFinished = 1

        GROUP BY t.Id, t.Name
        ";

            var result = await connection.QueryAsync<StandingResponse>(sql);

            // Cálculos finales en memoria (diferencia y puntos)
            foreach (var team in result)
            {
                team.GoalDifference = team.GoalsFor - team.GoalsAgainst;
                team.Points = (team.Won * 3) + team.Draw;
            }

            return result
                .OrderByDescending(x => x.Points)
                .ThenByDescending(x => x.GoalDifference)
                .ThenByDescending(x => x.GoalsFor);
        }
    }
}
