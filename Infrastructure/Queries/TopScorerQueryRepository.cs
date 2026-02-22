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
            int pageSize)
        {
            using var connection = new SqlConnection(_connectionString);

            var sql = @"
                SELECT COUNT(*) 
                FROM Players 
                WHERE Goals > 0;

                SELECT 
                    p.Id AS PlayerId,
                    p.Name AS PlayerName,
                    t.Name AS TeamName,
                    p.Goals
                FROM Players p
                INNER JOIN Teams t ON t.Id = p.TeamId
                WHERE p.Goals > 0
                ORDER BY p.Goals DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            ";

            var multi = await connection.QueryMultipleAsync(sql, new
            {
                Offset = (pageNumber - 1) * pageSize,
                PageSize = pageSize
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
