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
            if (filter.TaskStatus.HasValue && filter.TaskStatus.Value != 10)
            {
                whereConditions.Add("tt.task_status = @TaskStatus");
                parameters.Add("@TaskStatus", filter.TaskStatus.Value);
            }
            if(filter.TaskStatus.HasValue && filter.TaskStatus.Value == 10)
            {
                whereConditions.Add("tt.task_status IN (0,1,2,3)");
            }

            // Filter theo service ticket status (chỉ lấy chưa hoàn thành)
            if (filter.ServiceTicketStatus.HasValue && filter.TaskStatus.Value != 10)
            {
                whereConditions.Add("st.service_ticket_status = @ServiceTicketStatus");
                parameters.Add("@ServiceTicketStatus", filter.ServiceTicketStatus.Value);
            }
            else if (filter.ServiceTicketStatus.HasValue && filter.ServiceTicketStatus.Value == 10)
            {
                whereConditions.Add("st.service_ticket_status IN (0,1,2,3,4,5)");
            }
            else
            {
                // Mặc định chỉ lấy service ticket chưa hoàn thành
                whereConditions.Add("st.service_ticket_status != 3 && st.service_ticket_status != 5 "); // != Completed
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

            // Phan trang
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
                    tt.assigned_at AS AssignedAt,
                    CAST(tt.task_status AS UNSIGNED) AS TaskStatus,
                    tt.confirmed_at AS ConfirmedAt,
                    CAST(st.service_ticket_status AS UNSIGNED) AS ServiceTicketStatus,
                    u1.user_id AS AssignedToTechnicalUserId,
                    u1.full_name AS AssignedToTechnicalFullName,
                    u1.email AS AssignedToTechnicalEmail,
                    u1.phone AS AssignedToTechnicalPhone,
                    u2.user_id AS ConfirmedByUserId,
                    u2.full_name AS ConfirmedByFullName,
                    u2.email AS ConfirmedByEmail,
                    u2.phone AS ConfirmedByPhone
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
                WHERE tt.technical_task_id = @Id AND v.is_deleted = 0";

            // Lấy parts từ service ticket detail
            var partsSql = @"
                SELECT 
                    std.service_ticket_detail_id AS ServiceTicketDetailId,
                    std.quantity AS Quantity,
                    p.part_id AS PartId,
                    p.part_name AS PartName,
                    p.part_code AS PartCode,
                    p.part_quantity AS PartQuantity,
                    p.part_unit AS PartUnit,
                    p.part_price AS PartPrice,
                    pc.part_category_id AS PartCategoryId,
                    pc.part_category_name AS PartCategoryName,
                    pc.part_category_code AS PartCategoryCode
                FROM `service_ticket_detail` std
                INNER JOIN `part` p ON std.part_id = p.part_id
                LEFT JOIN `part_category` pc ON p.part_category_id = pc.part_category_id
                INNER JOIN `technical_task` tt ON std.service_ticket_id = tt.service_ticket_id
                WHERE tt.technical_task_id = @Id AND std.is_deleted = 0 AND std.part_id IS NOT NULL AND p.is_deleted = 0";

            // Lấy garage services từ service ticket detail
            var servicesSql = @"
                SELECT 
                    std.service_ticket_detail_id AS ServiceTicketDetailId,
                    gs.garage_service_id AS GarageServiceId,
                    gs.garage_service_name AS GarageServiceName,
                    gs.garage_service_price AS GarageServicePrice
                FROM `service_ticket_detail` std
                INNER JOIN `garage_service` gs ON std.garage_service_id = gs.garage_service_id
                INNER JOIN `technical_task` tt ON std.service_ticket_id = tt.service_ticket_id
                WHERE tt.technical_task_id = @Id AND std.is_deleted = 0 AND std.garage_service_id IS NOT NULL AND gs.is_deleted = 0";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);

            using var connection = new MySqlConnection(_connection);
            
            // Lấy task detail
            var taskData = await connection.QueryFirstOrDefaultAsync(taskSql, parameters);
            if (taskData == null) return null;

            // Map task info
            var detail = new TechnicalTaskDetailDto
            {
                TechnicalTaskId = taskData.TechnicalTaskId,
                ServiceTicketId = taskData.ServiceTicketId,
                ServiceTicketCode = taskData.ServiceTicketCode,
                Description = taskData.Description ?? string.Empty,
                AssignedAt = taskData.AssignedAt,
                TaskStatus = taskData.TaskStatus != null ? (byte?)Convert.ToByte(taskData.TaskStatus) : null,
                ConfirmedAt = taskData.ConfirmedAt,
                ServiceTicketStatus = taskData.ServiceTicketStatus != null ? (byte?)Convert.ToByte(taskData.ServiceTicketStatus) : null
            };

            // Map AssignedToTechnical
            if (taskData.AssignedToTechnicalUserId != null)
            {
                detail.AssignedToTechnical = new SWP.Core.Dtos.SeriveTicketDto.UserInfoDto
                {
                    UserId = taskData.AssignedToTechnicalUserId,
                    FullName = taskData.AssignedToTechnicalFullName,
                    Email = taskData.AssignedToTechnicalEmail,
                    Phone = taskData.AssignedToTechnicalPhone
                };
            }

            // Map ConfirmedBy
            if (taskData.ConfirmedByUserId != null)
            {
                detail.ConfirmedBy = new SWP.Core.Dtos.SeriveTicketDto.UserInfoDto
                {
                    UserId = taskData.ConfirmedByUserId,
                    FullName = taskData.ConfirmedByFullName,
                    Email = taskData.ConfirmedByEmail,
                    Phone = taskData.ConfirmedByPhone
                };
            }

            // Map customer and vehicle
            var customerVehicleData = await connection.QueryFirstOrDefaultAsync(customerVehicleSql, parameters);
            if (customerVehicleData != null)
            {
                detail.Customer = new SWP.Core.Dtos.SeriveTicketDto.CustomerInfoDto
                {
                    CustomerId = customerVehicleData.CustomerId,
                    CustomerName = customerVehicleData.CustomerName,
                    CustomerPhone = customerVehicleData.CustomerPhone,
                    CustomerEmail = customerVehicleData.CustomerEmail
                };

                detail.Vehicle = new SWP.Core.Dtos.SeriveTicketDto.VehicleInfoDto
                {
                    VehicleId = customerVehicleData.VehicleId,
                    VehicleName = customerVehicleData.VehicleName,
                    VehicleLicensePlate = customerVehicleData.VehicleLicensePlate,
                    CurrentKm = customerVehicleData.CurrentKm,
                    Make = customerVehicleData.Make,
                    Model = customerVehicleData.Model
                };
            }

            // Map parts
            var partsData = await connection.QueryAsync(partsSql, parameters);
            var parts = new List<SWP.Core.Dtos.SeriveTicketDto.ServiceTicketDetailItemDto>();
            foreach (var row in partsData)
            {
                var partInfo = new SWP.Core.Dtos.SeriveTicketDto.PartInfoDto
                {
                    PartId = row.PartId,
                    PartName = row.PartName ?? string.Empty,
                    PartCode = row.PartCode ?? string.Empty,
                    PartPrice = row.PartPrice,
                    PartQuantity = row.PartQuantity,
                    PartUnit = row.PartUnit ?? string.Empty
                };

                if (row.PartCategoryId != null)
                {
                    partInfo.PartCategory = new SWP.Core.Dtos.SeriveTicketDto.PartCategoryInfoDto
                    {
                        PartCategoryId = row.PartCategoryId,
                        PartCategoryName = row.PartCategoryName,
                        PartCategoryCode = row.PartCategoryCode
                    };
                }

                parts.Add(new SWP.Core.Dtos.SeriveTicketDto.ServiceTicketDetailItemDto
                {
                    ServiceTicketDetailId = row.ServiceTicketDetailId,
                    Part = partInfo,
                    Quantity = row.Quantity
                });
            }
            detail.Parts = parts;

            // Map garage services
            var servicesData = await connection.QueryAsync(servicesSql, parameters);
            var services = new List<SWP.Core.Dtos.SeriveTicketDto.ServiceTicketDetailServiceDto>();
            foreach (var row in servicesData)
            {
                services.Add(new SWP.Core.Dtos.SeriveTicketDto.ServiceTicketDetailServiceDto
                {
                    ServiceTicketDetailId = row.ServiceTicketDetailId,
                    GarageService = new SWP.Core.Dtos.SeriveTicketDto.GarageServiceInfoDto
                    {
                        GarageServiceId = row.GarageServiceId,
                        GarageServiceName = row.GarageServiceName,
                        GarageServicePrice = row.GarageServicePrice
                    }
                });
            }
            detail.GarageServices = services;

            return detail;
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

