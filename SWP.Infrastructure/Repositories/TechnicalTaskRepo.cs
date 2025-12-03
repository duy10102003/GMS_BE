using Dapper;
using Microsoft.Extensions.Configuration;
using SWP.Core.Dtos;
using SWP.Core.Dtos.TechnicalTaskDto;
using SWP.Core.Entities;
using SWP.Core.Interfaces.Repositories;
using MySqlConnector;
using MISA.QLSX.Infrastructure.Repositories;

namespace SWP.Infrastructure.Repositories
{
    /// <summary>
    /// Repository cho Technical Task
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public class TechnicalTaskRepo : BaseRepo<TechnicalTask>, ITechnicalTaskRepo
    {
        private readonly string _connection;

        public TechnicalTaskRepo(IConfiguration configuration) : base(configuration)
        {
            _connection = configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Lấy danh sách Technical Task có phân trang
        /// </summary>
        public async Task<PagedResult<TechnicalTaskListItemDto>> GetPagingAsync(TechnicalTaskFilterDtoRequest filter)
        {
            var parameters = new DynamicParameters();
            var whereConditions = new List<string>();

            // Base query
            var baseSelect = @"SELECT 
                    tt.technical_task_id AS TechnicalTaskId,
                    tt.service_ticket_id AS ServiceTicketId,
                    st.service_ticket_code AS ServiceTicketCode,
                    tt.description AS Description,
                    tt.assigned_to_technical AS AssignedToTechnical,
                    u1.full_name AS AssignedToTechnicalName,
                    tt.assigned_at AS AssignedAt,
                    tt.task_status AS TaskStatus,
                    tt.confirmed_by AS ConfirmedBy,
                    u2.full_name AS ConfirmedByName,
                    tt.confirmed_at AS ConfirmedAt,
                    st.service_ticket_status AS ServiceTicketStatus,
                    c.customer_name AS CustomerName,
                    v.vehicle_name AS VehicleName,
                    v.vehicle_license_plate AS VehicleLicensePlate
                FROM `technical_task` tt
                INNER JOIN `service_ticket` st ON tt.service_ticket_id = st.service_ticket_id
                LEFT JOIN `vehicle` v ON st.vehicle_id = v.vehicle_id
                LEFT JOIN `customer` c ON v.customer_id = c.customer_id
                LEFT JOIN `users` u1 ON tt.assigned_to_technical = u1.user_id
                LEFT JOIN `users` u2 ON tt.confirmed_by = u2.user_id
                WHERE tt.is_deleted = 0 AND st.is_deleted = 0";

            // Filter theo technical staff
            if (filter.AssignedToTechnical.HasValue)
            {
                whereConditions.Add("tt.assigned_to_technical = @AssignedToTechnical");
                parameters.Add("@AssignedToTechnical", filter.AssignedToTechnical.Value);
            }

            // Filter theo task status
            if (filter.TaskStatus.HasValue)
            {
                whereConditions.Add("tt.task_status = @TaskStatus");
                parameters.Add("@TaskStatus", filter.TaskStatus.Value);
            }

            // Filter theo service ticket status (chỉ lấy chưa hoàn thành)
            if (filter.ServiceTicketStatus.HasValue)
            {
                whereConditions.Add("st.service_ticket_status = @ServiceTicketStatus");
                parameters.Add("@ServiceTicketStatus", filter.ServiceTicketStatus.Value);
            }
            else
            {
                // Mặc định chỉ lấy service ticket chưa hoàn thành
                whereConditions.Add("st.service_ticket_status != 3"); // != Completed
            }

            // Build WHERE clause
            var whereClause = whereConditions.Any() 
                ? " AND " + string.Join(" AND ", whereConditions)
                : "";

            // Count query
            var countSql = $@"
                SELECT COUNT(1)
                FROM `technical_task` tt
                INNER JOIN `service_ticket` st ON tt.service_ticket_id = st.service_ticket_id
                WHERE tt.is_deleted = 0 AND st.is_deleted = 0{whereClause}";

            // Sort
            var orderBy = "ORDER BY tt.technical_task_id DESC";

            // Pagination
            var offset = (filter.Page - 1) * filter.PageSize;
            parameters.Add("@Offset", offset);
            parameters.Add("@PageSize", filter.PageSize);

            var dataSql = $"{baseSelect}{whereClause} {orderBy} LIMIT @PageSize OFFSET @Offset";

            using var connection = new MySqlConnection(_connection);
            
            var total = await connection.QuerySingleAsync<int>(countSql, parameters);
            var items = await connection.QueryAsync<TechnicalTaskListItemDto>(dataSql, parameters);

            return new PagedResult<TechnicalTaskListItemDto>
            {
                Items = items,
                Total = total,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }

        /// <summary>
        /// Lấy chi tiết Technical Task theo ID
        /// </summary>
        public async Task<TechnicalTaskDetailDto?> GetDetailAsync(int id)
        {
            // Lấy thông tin task
            var taskSql = @"
                SELECT 
                    tt.technical_task_id AS TechnicalTaskId,
                    tt.service_ticket_id AS ServiceTicketId,
                    st.service_ticket_code AS ServiceTicketCode,
                    tt.description AS Description,
                    tt.assigned_to_technical AS AssignedToTechnical,
                    u1.user_id AS UserId,
                    u1.full_name AS FullName,
                    u1.email AS Email,
                    u1.phone AS Phone,
                    tt.assigned_at AS AssignedAt,
                    tt.task_status AS TaskStatus,
                    tt.confirmed_by AS ConfirmedBy,
                    u2.user_id AS UserId,
                    u2.full_name AS FullName,
                    u2.email AS Email,
                    u2.phone AS Phone,
                    tt.confirmed_at AS ConfirmedAt,
                    st.service_ticket_status AS ServiceTicketStatus
                FROM `technical_task` tt
                INNER JOIN `service_ticket` st ON tt.service_ticket_id = st.service_ticket_id
                LEFT JOIN `users` u1 ON tt.assigned_to_technical = u1.user_id
                LEFT JOIN `users` u2 ON tt.confirmed_by = u2.user_id
                WHERE tt.technical_task_id = @Id AND tt.is_deleted = 0";

            // Lấy thông tin customer và vehicle
            var customerVehicleSql = @"
                SELECT 
                    c.customer_id AS CustomerId,
                    c.customer_name AS CustomerName,
                    c.customer_phone AS CustomerPhone,
                    c.customer_email AS CustomerEmail,
                    v.vehicle_id AS VehicleId,
                    v.vehicle_name AS VehicleName,
                    v.vehicle_license_plate AS VehicleLicensePlate,
                    v.current_km AS CurrentKm,
                    v.make AS Make,
                    v.model AS Model
                FROM `technical_task` tt
                INNER JOIN `service_ticket` st ON tt.service_ticket_id = st.service_ticket_id
                INNER JOIN `vehicle` v ON st.vehicle_id = v.vehicle_id
                LEFT JOIN `customer` c ON v.customer_id = c.customer_id
                WHERE tt.technical_task_id = @Id";

            // Lấy parts và services từ service ticket detail
            var partsServicesSql = @"
                SELECT 
                    std.service_ticket_detail_id AS ServiceTicketDetailId,
                    p.part_id AS PartId,
                    p.part_name AS PartName,
                    p.part_code AS PartCode,
                    p.part_price AS InventoryPrice,
                    p.part_quantity AS PartStock,
                    p.part_unit AS PartUnit,
                    std.quantity AS Quantity
                FROM `service_ticket_detail` std
                INNER JOIN `part` p ON std.part_id = p.part_id
                INNER JOIN `technical_task` tt ON std.service_ticket_id = tt.service_ticket_id
                WHERE tt.technical_task_id = @Id AND std.is_deleted = 0 AND std.part_id IS NOT NULL
                
                UNION ALL
                
                SELECT 
                    std.service_ticket_detail_id AS ServiceTicketDetailId,
                    NULL AS PartId,
                    NULL AS PartName,
                    NULL AS PartCode,
                    NULL AS InventoryPrice,
                    NULL AS PartStock,
                    NULL AS PartUnit,
                    std.quantity AS Quantity
                FROM `service_ticket_detail` std
                INNER JOIN `garage_service` gs ON std.garage_service_id = gs.garage_service_id
                INNER JOIN `technical_task` tt ON std.service_ticket_id = tt.service_ticket_id
                WHERE tt.technical_task_id = @Id AND std.is_deleted = 0 AND std.garage_service_id IS NOT NULL";

            using var connection = new MySqlConnection(_connection);
            
            // Lấy task detail (cần query riêng vì có nested objects)
            var task = await connection.QueryFirstOrDefaultAsync<dynamic>(taskSql, new { Id = id });
            if (task == null) return null;

            // TODO: Map to DTO properly - cần implement mapping logic
            // Tạm thời return null, sẽ implement sau
            return null;
        }

        /// <summary>
        /// Lấy Technical Task theo Service Ticket ID
        /// </summary>
        public async Task<TechnicalTask?> GetByServiceTicketIdAsync(int serviceTicketId)
        {
            var sql = "SELECT * FROM `technical_task` WHERE `service_ticket_id` = @ServiceTicketId AND `is_deleted` = 0";
            using var connection = new MySqlConnection(_connection);
            return await connection.QueryFirstOrDefaultAsync<TechnicalTask>(sql, new { ServiceTicketId = serviceTicketId });
        }
    }
}

