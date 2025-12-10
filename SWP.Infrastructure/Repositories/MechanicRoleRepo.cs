using Dapper;
using Microsoft.Extensions.Configuration;
using SWP.Core.Dtos.MechanicRoleDto;
using SWP.Core.Entities;
using SWP.Core.Interfaces.Repositories;
using MySqlConnector;
using MISA.QLSX.Infrastructure.Repositories;

namespace SWP.Infrastructure.Repositories
{
    public class MechanicRoleRepo : BaseRepo<MechanicRole>, IMechanicRoleRepo
    {
        private readonly string _connection;

        public MechanicRoleRepo(IConfiguration configuration) : base(configuration)
        {
            _connection = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<MechanicRoleDto>> GetAllRolesAsync()
        {
            const string sql = @"SELECT 
                    mechanic_role_id AS MechanicRoleId,
                    mechanic_role_name AS MechanicRoleName,
                    mechanic_role_description AS MechanicRoleDescription
                FROM mechanic_role
                WHERE is_deleted = 0";

            using var connection = new MySqlConnection(_connection);
            var result = await connection.QueryAsync<MechanicRoleDto>(sql);
            return result.ToList();
        }

        public async Task<List<MechanicRoleAssignmentDto>> GetAssignmentsByUserAsync(int userId)
        {
            const string sql = @"
                SELECT 
                    mrp.mechanic_role_permission_id AS MechanicRolePermissionId,
                    mrp.mechanic_role_id AS MechanicRoleId,
                    mr.mechanic_role_name AS MechanicRoleName,
                    mrp.user_id AS UserId,
                    mrp.year_exp AS YearExp
                FROM mechanic_role_permission mrp
                INNER JOIN mechanic_role mr ON mrp.mechanic_role_id = mr.mechanic_role_id
                WHERE mrp.is_deleted = 0 AND mr.is_deleted = 0 AND mrp.user_id = @UserId
            ";
            using var connection = new MySqlConnection(_connection);
            var result = await connection.QueryAsync<MechanicRoleAssignmentDto>(sql, new { UserId = userId });
            return result.ToList();
        }

        public async Task<int> AssignRoleAsync(AssignMechanicRoleRequest request)
        {
            const string getSql = @"SELECT mechanic_role_permission_id, is_deleted 
                                    FROM mechanic_role_permission 
                                    WHERE user_id = @UserId AND mechanic_role_id = @MechanicRoleId
                                    LIMIT 1";

            const string insertSql = @"INSERT INTO mechanic_role_permission (mechanic_role_id, user_id, year_exp, is_deleted)
                                       VALUES (@MechanicRoleId, @UserId, @YearExp, 0);";

            const string updateSql = @"UPDATE mechanic_role_permission 
                                       SET year_exp = @YearExp, is_deleted = 0
                                       WHERE mechanic_role_permission_id = @MechanicRolePermissionId;";

            using var connection = new MySqlConnection(_connection);
            var existing = await connection.QueryFirstOrDefaultAsync<(int mechanic_role_permission_id, int is_deleted)>(
                getSql, new { request.UserId, request.MechanicRoleId });

            if (existing.mechanic_role_permission_id != 0)
            {
                return await connection.ExecuteAsync(updateSql, new
                {
                    request.YearExp,
                    MechanicRolePermissionId = existing.mechanic_role_permission_id
                });
            }

            return await connection.ExecuteAsync(insertSql, request);
        }

        public async Task<int> RemoveAssignmentAsync(int userId, int mechanicRoleId)
        {
            const string sql = @"UPDATE mechanic_role_permission
                                 SET is_deleted = 1
                                 WHERE user_id = @UserId AND mechanic_role_id = @MechanicRoleId";
            using var connection = new MySqlConnection(_connection);
            return await connection.ExecuteAsync(sql, new { UserId = userId, MechanicRoleId = mechanicRoleId });
        }
    }
}
