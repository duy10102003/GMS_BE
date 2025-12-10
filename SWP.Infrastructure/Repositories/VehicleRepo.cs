using Dapper;
using Microsoft.Extensions.Configuration;
using SWP.Core.Dtos.VehicleDto;
using SWP.Core.Entities;
using SWP.Core.Interfaces.Repositories;
using MySqlConnector;
using MISA.QLSX.Infrastructure.Repositories;

namespace SWP.Infrastructure.Repositories
{
    /// <summary>
    /// Repository cho Vehicle
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public class VehicleRepo : BaseRepo<Vehicle>, IVehicleRepo
    {
        private readonly string _connection;

        public VehicleRepo(IConfiguration configuration) : base(configuration)
        {
            _connection = configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Tìm kiếm Vehicle cho select (với search keyword và filter theo customer)
        /// </summary>
        public async Task<List<VehicleSelectDto>> SearchForSelectAsync(VehicleSearchRequest request)
        {
            var sql = @"
                SELECT 
                    v.vehicle_id AS VehicleId,
                    v.vehicle_name AS VehicleName,
                    v.vehicle_license_plate AS VehicleLicensePlate,
                    v.current_km AS CurrentKm,
                    v.make AS Make,
                    v.model AS Model,
                    v.customer_id AS CustomerId,
                    c.customer_name AS CustomerName
                FROM `vehicle` v
                LEFT JOIN `customer` c ON v.customer_id = c.customer_id
                WHERE v.is_deleted = 0";

            var parameters = new DynamicParameters();

            if (request.CustomerId.HasValue)
            {
                sql += " AND v.customer_id = @CustomerId";
                parameters.Add("@CustomerId", request.CustomerId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchKeyword))
            {
                sql += " AND (v.vehicle_name LIKE @SearchKeyword OR v.vehicle_license_plate LIKE @SearchKeyword)";
                parameters.Add("@SearchKeyword", $"%{request.SearchKeyword.Trim()}%");
            }

            sql += " ORDER BY v.vehicle_name ASC LIMIT @Limit";
            parameters.Add("@Limit", request.Limit);

            using var connection = new MySqlConnection(_connection);
            var result = await connection.QueryAsync<VehicleSelectDto>(sql, parameters);
            return result.ToList();
        }
    }
}


