using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common;
using Application.Queries.Matches;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Queries
{
    public class MatchQueryRepository
    {
        private readonly string _connectionString;

        public MatchQueryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<PagedResult<MatchResponse>> GetMatchesAsync(
            int pageNumber,
            int pageSize,
            string? sortBy,
            string? sortDirection,
            Guid? teamId,
            DateTime? date,
            bool? isFinished)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            using var connection = new SqlConnection(_connectionString);

            // Columnas válidas (anti SQL injection)
            var validSortColumns = new Dictionary<string, string>
        {
            { "date", "m.MatchDate" },
            { "home", "ht.Name" },
            { "away", "at.Name" },
            { "finished", "m.IsFinished" }
        };

            var sortColumn = validSortColumns.ContainsKey(sortBy?.ToLower() ?? "")
                ? validSortColumns[sortBy!.ToLower()]
                : "m.MatchDate";

            var direction = sortDirection?.ToLower() == "asc" ? "ASC" : "DESC";

            var whereClauses = new List<string>();
            var parameters = new DynamicParameters();

            if (teamId.HasValue)
            {
                whereClauses.Add("(m.HomeTeamId = @TeamId OR m.AwayTeamId = @TeamId)");
                parameters.Add("TeamId", teamId);
            }

            if (date.HasValue)
            {
                whereClauses.Add("CAST(m.MatchDate AS DATE) = @MatchDate");
                parameters.Add("MatchDate", date.Value.Date);
            }

            if (isFinished.HasValue)
            {
                whereClauses.Add("m.IsFinished = @IsFinished");
                parameters.Add("IsFinished", isFinished);
            }

            var whereSql = whereClauses.Any()
                ? "WHERE " + string.Join(" AND ", whereClauses)
                : "";

            parameters.Add("Offset", (pageNumber - 1) * pageSize);
            parameters.Add("PageSize", pageSize);

            var sql = $@"
                SELECT COUNT(*)
                FROM Matches m
                {whereSql};

                SELECT 
                    m.Id AS MatchId,
                    m.HomeTeamId,
                    m.AwayTeamId,
                    ht.Name AS HomeTeamName,
                    at.Name AS AwayTeamName,
                    m.HomeGoals,
                    m.AwayGoals,
                    m.MatchDate,
                    m.IsFinished
                FROM Matches m
                INNER JOIN Teams ht ON ht.Id = m.HomeTeamId
                INNER JOIN Teams at ON at.Id = m.AwayTeamId
                {whereSql}
                ORDER BY {sortColumn} {direction}
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            ";

            var multi = await connection.QueryMultipleAsync(sql, parameters);

            var totalRecords = await multi.ReadSingleAsync<int>();
            var data = (await multi.ReadAsync<MatchResponse>()).ToList();

            return new PagedResult<MatchResponse>
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
