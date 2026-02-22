using Application.Common;
using Application.Queries.Goleadores;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Queries
{
    public class TopScorerQueryRepository
    {
        private readonly string _connectionString;

        public TopScorerQueryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Obtiene goleadores paginados y ordenados por goles descendente
        public async Task<PagedResult<TopScorerResponse>> GetTopScorersAsync(
            int pageNumber,
            int pageSize,
            string? sortBy,
            string? sortDirection,
            Guid? teamId)
        {
                if (pageNumber <= 0) pageNumber = 1;
                if (pageSize <= 0) pageSize = 10;
                if (pageSize > 100) pageSize = 100;

                using var connection = new SqlConnection(_connectionString);

                // Columnas permitidas para evitar SQL Injection
                var validSortColumns = new Dictionary<string, string>
                {
                    { "goals", "p.Goals" },
                    { "name", "p.Name" },
                    { "team", "t.Name" }
                };

                var sortColumn = validSortColumns.ContainsKey(sortBy?.ToLower() ?? "")
                    ? validSortColumns[sortBy!.ToLower()]
                    : "p.Goals";

                var direction = sortDirection?.ToLower() == "asc" ? "ASC" : "DESC";

                var whereClause = "WHERE p.Goals > 0";

                if (teamId.HasValue)
                    whereClause += " AND p.TeamId = @TeamId";

                var sql = $@"
                    SELECT COUNT(*) 
                    FROM Players p
                    {whereClause};

                    SELECT 
                        p.Id AS PlayerId,
                        p.Name AS PlayerName,
                        t.Name AS TeamName,
                        p.Goals
                    FROM Players p
                    INNER JOIN Teams t ON t.Id = p.TeamId
                    {whereClause}
                    ORDER BY {sortColumn} {direction}
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
                ";

                var multi = await connection.QueryMultipleAsync(sql, new
                {
                    Offset = (pageNumber - 1) * pageSize,
                    PageSize = pageSize,
                    TeamId = teamId
                });

                var totalRecords = await multi.ReadSingleAsync<int>();
                var data = (await multi.ReadAsync<TopScorerResponse>()).ToList();

                return new PagedResult<TopScorerResponse>
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
