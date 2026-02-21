using Application.Abstractions;
using Application.Common;
using Application.Queries.Players;
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
    public class PlayerQueryRepository : IPlayerQueryRepository
    {
        private readonly string _connectionString;

        public PlayerQueryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<PagedResult<PlayerResponse>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? name,
            Guid? teamId,
            string? sortBy,
            string? sortDirection)
        {
            using var connection = new SqlConnection(_connectionString);

            var offset = (pageNumber - 1) * pageSize;

            var validColumns = new[] { "Name", "Goals" };

            if (string.IsNullOrWhiteSpace(sortBy) || !validColumns.Contains(sortBy))
                sortBy = "Name";

            sortDirection = sortDirection?.ToUpper() == "DESC" ? "DESC" : "ASC";

            var whereConditions = new List<string>();
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(name))
            {
                whereConditions.Add("Name LIKE @Name");
                parameters.Add("Name", $"%{name}%");
            }

            if (teamId.HasValue)
            {
                whereConditions.Add("TeamId = @TeamId");
                parameters.Add("TeamId", teamId.Value);
            }

            var whereClause = whereConditions.Any()
                ? "WHERE " + string.Join(" AND ", whereConditions)
                : "";

            parameters.Add("Offset", offset);
            parameters.Add("PageSize", pageSize);

            var sql = $@"
                SELECT COUNT(*)
                FROM Players
                {whereClause};

                SELECT Id, Name, TeamId, Goals
                FROM Players
                {whereClause}
                ORDER BY {sortBy} {sortDirection}
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;
            ";
            //“Utilicé QueryMultiple para ejecutar ambas consultas en un solo roundtrip a la base de datos, reduciendo latencia y asegurando consistencia entre el total de registros y los datos paginados.”
            using var multi = await connection.QueryMultipleAsync(sql, parameters);

            var totalRecords = await multi.ReadFirstAsync<int>();
            var data = await multi.ReadAsync<PlayerResponse>();

            return new PagedResult<PlayerResponse>
            {
                Data = data,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize)
            };
        }
    }
}
