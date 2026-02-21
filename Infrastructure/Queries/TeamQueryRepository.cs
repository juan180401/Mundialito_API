using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Dapper;
using Application.Abstractions;
using Application.Common;
using Application.Queries.Teams;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Querys
{
    /// <summary>
    /// Implementación real del read side usando Dapper.
    /// NO usamos EF aquí.
    /// </summary>
    public class TeamQueryRepository : ITeamQueryRepository
    {
        private readonly string _connectionString;

        public TeamQueryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<PagedResult<TeamResponse>> GetPagedAsync(
            int pageNumber,
            int pageSize)
        {
            using var connection = new SqlConnection(_connectionString);

            var offset = (pageNumber - 1) * pageSize;

            var sql = @"
            SELECT COUNT(*) FROM Teams;

            SELECT Id, Name
            FROM Teams
            ORDER BY Name
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY;
        ";
            //“Utilicé QueryMultiple para ejecutar ambas consultas en un solo roundtrip a la base de datos, reduciendo latencia y asegurando consistencia entre el total de registros y los datos paginados.”
            using var multi = await connection.QueryMultipleAsync(sql, new
            {
                Offset = offset,
                PageSize = pageSize
            });

            var totalRecords = await multi.ReadFirstAsync<int>();
            var data = await multi.ReadAsync<TeamResponse>();

            return new PagedResult<TeamResponse>
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
