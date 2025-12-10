using Dapper;
using Microsoft.Extensions.Configuration;
using SWP.Core.Dtos.UserDto;
using SWP.Core.Entities;
using SWP.Core.Interfaces.Repositories;
using MySqlConnector;
using MISA.QLSX.Infrastructure.Repositories;

namespace SWP.Infrastructure.Repositories
{
    /// <summary>
    /// Repository cho User
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public class UserRepo : BaseRepo<User>, IUserRepo
    {
        private readonly string _connection;

        public UserRepo(IConfiguration configuration) : base(configuration)
        {
            _connection = configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Lấy danh sách Technical Staff với trạng thái rảnh/không rảnh
        /// </summary>
        public async Task<List<TechnicalStaffSelectDto>> GetTechnicalStaffWithAvailabilityAsync(string roleName)
        {
            var sql = @"
                SELECT 
                    u.user_id AS UserId,
                    u.full_name AS FullName,
                    u.email AS Email,
                    u.phone AS Phone,
                    u.role_id AS RoleId,
                    r.role_name AS RoleName,
                    COUNT(CASE 
                        WHEN st.service_ticket_status IN (0, 1, 2) AND tt.is_deleted = 0 THEN 1 
                        ELSE NULL 
                    END) AS CurrentTaskCount
                FROM `users` u
                INNER JOIN `role` r ON u.role_id = r.role_id
                LEFT JOIN `technical_task` tt ON u.user_id = tt.assigned_to_technical
                LEFT JOIN `service_ticket` st ON tt.service_ticket_id = st.service_ticket_id
                WHERE u.is_deleted = 0 
                    AND r.is_deleted = 0
                    AND r.role_name = @RoleName
                GROUP BY u.user_id, u.full_name, u.email, u.phone, u.role_id, r.role_name
                ORDER BY u.full_name ASC";

            var parameters = new DynamicParameters();
            parameters.Add("@RoleName", roleName);

            using var connection = new MySqlConnection(_connection);
            var result = await connection.QueryAsync<TechnicalStaffSelectDto>(sql, parameters);

            // Tính toán IsAvailable: rảnh khi CurrentTaskCount = 0
            var staffList = result.ToList();
            foreach (var staff in staffList)
            {
                staff.IsAvailable = (staff.CurrentTaskCount ?? 0) == 0;
            }

            return staffList;
        }
    }
}


