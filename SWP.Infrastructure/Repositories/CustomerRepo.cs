using Dapper;
using Microsoft.Extensions.Configuration;
using SWP.Core.Dtos.CustomerDto;
using SWP.Core.Entities;
using SWP.Core.Interfaces.Repositories;
using MySqlConnector;
using MISA.QLSX.Infrastructure.Repositories;

namespace SWP.Infrastructure.Repositories
{
    /// <summary>
    /// Repository cho Customer
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public class CustomerRepo : BaseRepo<Customer>, ICustomerRepo
    {
        private readonly string _connection;

        public CustomerRepo(IConfiguration configuration) : base(configuration)
        {
            _connection = configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Tìm kiếm Customer cho select (với search keyword)
        /// </summary>
        public async Task<List<CustomerSelectDto>> SearchForSelectAsync(CustomerSearchRequest request)
        {
            var sql = @"
                SELECT 
                    c.customer_id AS CustomerId,
                    c.customer_name AS CustomerName,
                    c.customer_phone AS CustomerPhone,
                    c.customer_email AS CustomerEmail
                FROM `customer` c
                WHERE c.is_deleted = 0";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(request.SearchKeyword))
            {
                sql += " AND (c.customer_name LIKE @SearchKeyword OR c.customer_phone LIKE @SearchKeyword OR c.customer_email LIKE @SearchKeyword)";
                parameters.Add("@SearchKeyword", $"%{request.SearchKeyword.Trim()}%");
            }

            sql += " ORDER BY c.customer_name ASC LIMIT @Limit";
            parameters.Add("@Limit", request.Limit);

            using var connection = new MySqlConnection(_connection);
            var result = await connection.QueryAsync<CustomerSelectDto>(sql, parameters);
            return result.ToList();
        }

        /// <summary>
        /// Lấy thông tin customer theo userId.
        /// </summary>
        public async Task<Customer?> GetByUserIdAsync(int userId)
        {
            var sql = @"
                SELECT 
                    c.customer_id AS CustomerId,
                    c.customer_name AS CustomerName,
                    c.customer_phone AS CustomerPhone,
                    c.customer_email AS CustomerEmail,
                    c.user_id AS UserId,
                    c.is_deleted AS IsDeleted
                FROM `customer` c
                WHERE c.user_id = @UserId AND c.is_deleted = 0
                LIMIT 1";

            using var connection = new MySqlConnection(_connection);
            return await connection.QueryFirstOrDefaultAsync<Customer>(sql, new { UserId = userId });
        }
    }
}

