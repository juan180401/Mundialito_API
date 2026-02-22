using Application.Common;
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

        public async Task<PagedResult<StandingResponse>> GetStandingsAsync(
            int pageNumber,
            int pageSize,
            string? sortBy,
            string? sortDirection)  
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            using var connection = new SqlConnection(_connectionString);

            var validSortColumns = new Dictionary<string, string>
            {
                { "points", "Points" },
                { "goaldifference", "GoalDifference" },
                { "goalsfor", "GoalsFor" },
                { "team", "TeamName" }
            };

            var sortColumn = validSortColumns.ContainsKey(sortBy?.ToLower() ?? "")
                ? validSortColumns[sortBy!.ToLower()]
                : "Points";

            var direction = sortDirection?.ToLower() == "asc" ? "ASC" : "DESC";

            var sql = $@"
            WITH Standings AS
            (
                SELECT 
                    t.Id AS TeamId,
                    t.Name AS TeamName,

                    COUNT(m.Id) AS Played,

                    SUM(CASE 
                        WHEN (m.HomeTeamId = t.Id AND m.HomeGoals > m.AwayGoals)
                          OR (m.AwayTeamId = t.Id AND m.AwayGoals > m.HomeGoals)
                        THEN 1 ELSE 0 END) AS Won,

                    SUM(CASE 
                        WHEN m.HomeGoals = m.AwayGoals AND m.IsFinished = 1
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
            )
            SELECT COUNT(*) FROM Standings;

            WITH Standings AS
            (
                SELECT 
                    t.Id AS TeamId,
                    t.Name AS TeamName,

                    COUNT(m.Id) AS Played,

                    SUM(CASE 
                        WHEN (m.HomeTeamId = t.Id AND m.HomeGoals > m.AwayGoals)
                          OR (m.AwayTeamId = t.Id AND m.AwayGoals > m.HomeGoals)
                        THEN 1 ELSE 0 END) AS Won,

                    SUM(CASE 
                        WHEN m.HomeGoals = m.AwayGoals AND m.IsFinished = 1
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
            )
            SELECT *,
                   (GoalsFor - GoalsAgainst) AS GoalDifference,
                   ((Won * 3) + Draw) AS Points
            FROM Standings
            ORDER BY {sortColumn} {direction}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            ";

            var multi = await connection.QueryMultipleAsync(sql, new
            {
                Offset = (pageNumber - 1) * pageSize,
                PageSize = pageSize
            });

            var totalRecords = await multi.ReadSingleAsync<int>();
            var data = (await multi.ReadAsync<StandingResponse>()).ToList();

            return new PagedResult<StandingResponse>
            {
                Data = data,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
            };
        }
    }
}
