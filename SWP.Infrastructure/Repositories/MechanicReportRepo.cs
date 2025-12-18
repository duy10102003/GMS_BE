using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using SWP.Core.Dtos.MechanicReportDto;
using SWP.Core.Interfaces.Repositories;

namespace SWP.Infrastructure.Repositories
{
    /// <summary>
    /// Repository cho báo cáo mechanic
    /// </summary>
    public class MechanicReportRepo : IMechanicReportRepo
    {
        private readonly string _connection;

        public MechanicReportRepo(IConfiguration configuration)
        {
            _connection = configuration.GetConnectionString("DefaultConnection")
                          ?? throw new ArgumentNullException("DefaultConnection");
        }

        public async Task<MechanicReportSummaryDto?> GetSummaryAsync(int mechanicId)
        {
            const string sql = @"
                SELECT 
                    u.user_id AS MechanicId,
                    u.full_name AS MechanicName,
                    COUNT(tt.technical_task_id) AS TotalTasks,
                    SUM(CASE WHEN tt.task_status = 0 THEN 1 ELSE 0 END) AS PendingTasks,
                    SUM(CASE WHEN tt.task_status = 1 THEN 1 ELSE 0 END) AS InProgressTasks,
                    SUM(CASE WHEN tt.task_status = 2 THEN 1 ELSE 0 END) AS AdjustedTasks,
                    SUM(CASE WHEN tt.task_status = 3 THEN 1 ELSE 0 END) AS CompletedTasks,
                    COUNT(DISTINCT tt.service_ticket_id) AS TotalServiceTickets,
                    COUNT(DISTINCT v.vehicle_id) AS TotalVehicles,
                    COUNT(DISTINCT c.customer_id) AS TotalCustomers
                FROM technical_task tt
                INNER JOIN service_ticket st ON tt.service_ticket_id = st.service_ticket_id
                    AND st.is_deleted = 0
                LEFT JOIN vehicle v ON st.vehicle_id = v.vehicle_id
                    AND v.is_deleted = 0
                LEFT JOIN customer c ON v.customer_id = c.customer_id
                    AND c.is_deleted = 0
                LEFT JOIN users u ON tt.assigned_to_technical = u.user_id
                WHERE tt.is_deleted = 0
                  AND tt.assigned_to_technical = @MechanicId
                GROUP BY u.user_id, u.full_name;";

            using var connection = new MySqlConnection(_connection);
            var result = await connection.QueryFirstOrDefaultAsync<MechanicReportSummaryDto>(
                sql,
                new { MechanicId = mechanicId });

            return result;
        }
    }
}

