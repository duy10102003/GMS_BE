using Dapper;
using Microsoft.Extensions.Configuration;
using SWP.Core.Dtos;
using SWP.Core.Dtos.SeriveTicketDto;
using SWP.Core.Entities;
using SWP.Core.Interfaces.Repositories;
using MySqlConnector;
using System.Text;
using System.Collections.Generic;
using MISA.QLSX.Infrastructure.Repositories;

namespace SWP.Infrastructure.Repositories
{
    /// <summary>
    /// Repository cho Service Ticket
    /// Created by: DuyLC(01/12/2025)
    /// </summary>
    public class ServiceTicketRepo : BaseRepo<ServiceTicket>, IServiceTicketRepo
    {
        private readonly string _connection;

        public ServiceTicketRepo(IConfiguration configuration) : base(configuration)
        {
            _connection = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("DefaultConnection");
        }

        /// <summary>
        /// Lấy danh sách Service Ticket có phân trang
        /// </summary>
        public async Task<PagedResult<ServiceTicketListItemDto>> GetPagingAsync(ServiceTicketFilterDtoRequest filter)
        {
            var parameters = new DynamicParameters();
            var whereConditions = new List<string>();
            var joinClauses = new List<string>();

            // Base query - sử dụng subquery để lấy technical staff được assign
            var baseSelect = @"SELECT DISTINCT
                    st.service_ticket_id AS ServiceTicketId,
                    st.service_ticket_code AS ServiceTicketCode,
                    st.booking_id AS BookingId,
                    st.vehicle_id AS VehicleId,
                    v.vehicle_name AS VehicleName,
                    v.vehicle_license_plate AS VehicleLicensePlate,
                    c.customer_id AS CustomerId,
                    c.customer_name AS CustomerName,
                    c.customer_phone AS CustomerPhone,
                    st.created_by AS CreatedBy,
                    u1.full_name AS CreatedByName,
                    st.created_date AS CreatedDate,
                    st.modified_by AS ModifiedBy,
                    u2.full_name AS ModifiedByName,
                    st.modified_date AS ModifiedDate,
                    CAST(st.service_ticket_status AS UNSIGNED) AS ServiceTicketStatus,
                    st.initial_issue AS InitialIssue,
                    (SELECT tt.assigned_to_technical 
                     FROM technical_task tt 
                     WHERE tt.service_ticket_id = st.service_ticket_id 
                     LIMIT 1) AS AssignedToTechnical,
                    (SELECT u3.full_name 
                     FROM technical_task tt 
                     LEFT JOIN users u3 ON tt.assigned_to_technical = u3.user_id
                     WHERE tt.service_ticket_id = st.service_ticket_id 
                     LIMIT 1) AS AssignedToTechnicalName
                FROM service_ticket st
                LEFT JOIN vehicle v ON st.vehicle_id = v.vehicle_id
                LEFT JOIN customer c ON v.customer_id = c.customer_id
                LEFT JOIN users u1 ON st.created_by = u1.user_id
                LEFT JOIN users u2 ON st.modified_by = u2.user_id";

            // Xử lý ColumnFilters
            if (filter.ColumnFilters != null && filter.ColumnFilters.Any())
            {
                var filterIndex = 0;
                foreach (var columnFilter in filter.ColumnFilters)
                {
                    if (string.IsNullOrWhiteSpace(columnFilter.ColumnName) || 
                        string.IsNullOrWhiteSpace(columnFilter.Operator))
                    {
                        continue;
                    }

                    var paramName = $"@FilterValue{filterIndex}";
                    var columnName = GetColumnNameForFilter(columnFilter.ColumnName, "st", "");

                    switch (columnFilter.Operator.ToLower())
                    {
                        case "equals":
                            whereConditions.Add($"{columnName} = {paramName}");
                            parameters.Add(paramName, columnFilter.Value);
                            break;
                        case "not_equals":
                            whereConditions.Add($"{columnName} != {paramName}");
                            parameters.Add(paramName, columnFilter.Value);
                            break;
                        case "contains":
                            whereConditions.Add($"{columnName} LIKE {paramName}");
                            parameters.Add(paramName, $"%{columnFilter.Value}%");
                            break;
                        case "not_contains":
                            whereConditions.Add($"{columnName} NOT LIKE {paramName}");
                            parameters.Add(paramName, $"%{columnFilter.Value}%");
                            break;
                        case "starts_with":
                            whereConditions.Add($"{columnName} LIKE {paramName}");
                            parameters.Add(paramName, $"{columnFilter.Value}%");
                            break;
                        case "ends_with":
                            whereConditions.Add($"{columnName} LIKE {paramName}");
                            parameters.Add(paramName, $"%{columnFilter.Value}");
                            break;
                        case "empty":
                            whereConditions.Add($"({columnName} IS NULL OR {columnName} = '')");
                            break;
                        case "not_empty":
                            whereConditions.Add($"({columnName} IS NOT NULL AND {columnName} != '')");
                            break;
                        case "greater_than":
                            whereConditions.Add($"{columnName} > {paramName}");
                            parameters.Add(paramName, columnFilter.Value);
                            break;
                        case "less_than":
                            whereConditions.Add($"{columnName} < {paramName}");
                            parameters.Add(paramName, columnFilter.Value);
                            break;
                        case "greater_or_equal":
                            whereConditions.Add($"{columnName} >= {paramName}");
                            parameters.Add(paramName, columnFilter.Value);
                            break;
                        case "less_or_equal":
                            whereConditions.Add($"{columnName} <= {paramName}");
                            parameters.Add(paramName, columnFilter.Value);
                            break;
                    }
                    filterIndex++;
                }
            }

            // Build WHERE clause
            var whereClause = whereConditions.Any() 
                ? "WHERE " + string.Join(" AND ", whereConditions)
                : "";

            // Count query
            var countSql = $@"
                SELECT COUNT(DISTINCT st.service_ticket_id)
                FROM service_ticket st
                LEFT JOIN vehicle v ON st.vehicle_id = v.vehicle_id
                LEFT JOIN customer c ON v.customer_id = c.customer_id
                LEFT JOIN technical_task tt ON st.service_ticket_id = tt.service_ticket_id
                {whereClause}";

            // Sort
            var orderBy = "ORDER BY st.created_date DESC";
            if (filter.ColumnSorts != null && filter.ColumnSorts.Any())
            {
                var sortParts = filter.ColumnSorts
                    .Where(s => !string.IsNullOrWhiteSpace(s.ColumnName) && 
                               !string.IsNullOrWhiteSpace(s.SortDirection))
                    .Select(s => 
                    {
                        var columnName = GetColumnNameForSort(s.ColumnName, "st", "");
                        var direction = s.SortDirection.ToUpper() == "ASC" ? "ASC" : "DESC";
                        return $"{columnName} {direction}";
                    });
                
                if (sortParts.Any())
                {
                    orderBy = "ORDER BY " + string.Join(", ", sortParts);
                }
            }

            // Pagination
            var offset = (filter.Page - 1) * filter.PageSize;
            parameters.Add("@Offset", offset);
            parameters.Add("@PageSize", filter.PageSize);

            var dataSql = string.IsNullOrWhiteSpace(whereClause)
                ? $"{baseSelect} {orderBy} LIMIT @PageSize OFFSET @Offset"
                : $"{baseSelect} {whereClause} {orderBy} LIMIT @PageSize OFFSET @Offset";

            using var connection = new MySqlConnection(_connection);
            
            var total = await connection.QuerySingleAsync<int>(countSql, parameters);
            var items = await connection.QueryAsync<ServiceTicketListItemDto>(dataSql, parameters);

            return new PagedResult<ServiceTicketListItemDto>
            {
                Items = items,
                Total = total,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }

        /// <summary>
        /// Lấy chi tiết Service Ticket theo ID
        /// </summary>
        public async Task<ServiceTicketDetailDto?> GetDetailAsync(int id)
        {
            var sql = @"
                SELECT 
                    st.service_ticket_id AS ServiceTicketId,
                    st.service_ticket_code AS ServiceTicketCode,
                    st.booking_id AS BookingId,
                    st.created_date AS CreatedDate,
                    st.modified_date AS ModifiedDate,
                    CAST(st.service_ticket_status AS UNSIGNED) AS ServiceTicketStatus,
                    st.initial_issue AS InitialIssue
                FROM service_ticket st
                WHERE st.service_ticket_id = @Id AND st.is_deleted = 0";

            var vehicleSql = @"
                SELECT 
                    v.vehicle_id AS VehicleId,
                    v.vehicle_name AS VehicleName,
                    v.vehicle_license_plate AS VehicleLicensePlate,
                    v.make AS Make,
                    v.model AS Model,
                    v.current_km AS CurrentKm
                FROM vehicle v
                INNER JOIN service_ticket st ON v.vehicle_id = st.vehicle_id
                WHERE st.service_ticket_id = @Id AND v.is_deleted = 0";

            var customerSql = @"
                SELECT 
                    c.customer_id AS CustomerId,
                    c.customer_name AS CustomerName,
                    c.customer_phone AS CustomerPhone,
                    c.customer_email AS CustomerEmail
                FROM customer c
                INNER JOIN vehicle v ON c.customer_id = v.customer_id
                INNER JOIN service_ticket st ON v.vehicle_id = st.vehicle_id
                WHERE st.service_ticket_id = @Id AND c.is_deleted = 0";

            var createdBySql = @"
                SELECT 
                    u.user_id AS UserId,
                    u.full_name AS FullName,
                    u.email AS Email,
                    u.phone AS Phone
                FROM users u
                INNER JOIN service_ticket st ON u.user_id = st.created_by
                WHERE st.service_ticket_id = @Id AND u.is_deleted = 0";

            var modifiedBySql = @"
                SELECT 
                    u.user_id AS UserId,
                    u.full_name AS FullName,
                    u.email AS Email,
                    u.phone AS Phone
                FROM users u
                INNER JOIN service_ticket st ON u.user_id = st.modified_by
                WHERE st.service_ticket_id = @Id AND st.modified_by IS NOT NULL AND u.is_deleted = 0";

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
                FROM service_ticket_detail std
                INNER JOIN part p ON std.part_id = p.part_id
                LEFT JOIN part_category pc ON p.part_category_id = pc.part_category_id
                WHERE std.service_ticket_id = @Id AND std.is_deleted = 0 AND std.part_id IS NOT NULL AND p.is_deleted = 0";

            var servicesSql = @"
                SELECT 
                    std.service_ticket_detail_id AS ServiceTicketDetailId,
                    gs.garage_service_id AS GarageServiceId,
                    gs.garage_service_name AS GarageServiceName,
                    gs.garage_service_price AS GarageServicePrice
                FROM `service_ticket_detail` std
                INNER JOIN `garage_service` gs ON std.garage_service_id = gs.garage_service_id
                WHERE std.service_ticket_id = @Id AND std.is_deleted = 0 AND std.garage_service_id IS NOT NULL AND gs.is_deleted = 0";

            var tasksSql = @"
                SELECT 
                    tt.technical_task_id AS TechnicalTaskId,
                    tt.service_ticket_id AS ServiceTicketId,
                    st.service_ticket_code AS ServiceTicketCode,
                    tt.description AS Description,
                    tt.assigned_at AS AssignedAt,
                    CAST(tt.task_status AS UNSIGNED) AS TaskStatus,
                    tt.confirmed_at AS ConfirmedAt,
                    u.user_id AS AssignedToTechnicalUserId,
                    u.full_name AS AssignedToTechnicalFullName,
                    u.email AS AssignedToTechnicalEmail,
                    u.phone AS AssignedToTechnicalPhone,
                    u2.user_id AS ConfirmedByUserId,
                    u2.full_name AS ConfirmedByFullName,
                    u2.email AS ConfirmedByEmail,
                    u2.phone AS ConfirmedByPhone
                FROM technical_task tt
                LEFT JOIN service_ticket st ON tt.service_ticket_id = st.service_ticket_id
                LEFT JOIN users u ON tt.assigned_to_technical = u.user_id
                LEFT JOIN users u2 ON tt.confirmed_by = u2.user_id
                WHERE tt.service_ticket_id = @Id AND tt.is_deleted = 0";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);

            using var connection = new MySqlConnection(_connection);
            
            // Query vào dynamic object trước để xử lý type conversion
            var detailData = await connection.QueryFirstOrDefaultAsync(sql, parameters);
            if (detailData == null)
            {
                return null;
            }
            
            // Map thủ công để xử lý type conversion
            var detail = new ServiceTicketDetailDto
            {
                ServiceTicketId = detailData.ServiceTicketId,
                ServiceTicketCode = detailData.ServiceTicketCode,
                BookingId = detailData.BookingId,
                CreatedDate = detailData.CreatedDate,
                ModifiedDate = detailData.ModifiedDate,
                ServiceTicketStatus = detailData.ServiceTicketStatus != null ? (byte?)Convert.ToByte(detailData.ServiceTicketStatus) : null,
                InitialIssue = detailData.InitialIssue
            };

            // Map vehicle info
            var vehicleInfo = await connection.QueryFirstOrDefaultAsync<VehicleInfoDto>(vehicleSql, parameters);
            if (vehicleInfo != null) detail.Vehicle = vehicleInfo;

            // Map customer info
            var customerInfo = await connection.QueryFirstOrDefaultAsync<CustomerInfoDto>(customerSql, parameters);
            if (customerInfo != null) detail.Customer = customerInfo;

            // Map created by user
            var createdByUser = await connection.QueryFirstOrDefaultAsync<UserInfoDto>(createdBySql, parameters);
            if (createdByUser != null) detail.CreatedByUser = createdByUser;

            // Map modified by user
            var modifiedByUser = await connection.QueryFirstOrDefaultAsync<UserInfoDto>(modifiedBySql, parameters);
            if (modifiedByUser != null) detail.ModifiedByUser = modifiedByUser;

            // Map parts với supplier info
            var partsData = await connection.QueryAsync(partsSql, parameters);
            var parts = new List<ServiceTicketDetailItemDto>();
            foreach (var row in partsData)
            {
                var partInfo = new PartInfoDto
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
                    partInfo.PartCategory = new PartCategoryInfoDto
                    {
                        PartCategoryId = row.PartCategoryId,
                        PartCategoryName = row.PartCategoryName,
                        PartCategoryCode = row.PartCategoryCode
                    };
                }

                parts.Add(new ServiceTicketDetailItemDto
                {
                    ServiceTicketDetailId = row.ServiceTicketDetailId,
                    Part = partInfo,
                    Quantity = row.Quantity
                });
            }

            // Map garage services thủ công vì có nested object
            var servicesData = await connection.QueryAsync(servicesSql, parameters);
            var services = new List<ServiceTicketDetailServiceDto>();
            foreach (var row in servicesData)
            {
                services.Add(new ServiceTicketDetailServiceDto
                {
                    ServiceTicketDetailId = row.ServiceTicketDetailId,
                    GarageService = new GarageServiceInfoDto
                    {
                        GarageServiceId = row.GarageServiceId,
                        GarageServiceName = row.GarageServiceName,
                        GarageServicePrice = row.GarageServicePrice
                    }
                });
            }

            var tasksData = await connection.QueryAsync(tasksSql, parameters);
            var tasks = new List<TechnicalTaskDto>();
            foreach (var row in tasksData)
            {
                var task = new TechnicalTaskDto
                {
                    TechnicalTaskId = row.TechnicalTaskId,
                    ServiceTicketId = row.ServiceTicketId,
                    ServiceTicketCode = row.ServiceTicketCode,
                    Description = row.Description ?? string.Empty,
                    AssignedAt = row.AssignedAt,
                    TaskStatus = row.TaskStatus != null ? (byte?)Convert.ToByte(row.TaskStatus) : null,
                    ConfirmedAt = row.ConfirmedAt
                };

                if (row.AssignedToTechnicalUserId != null)
                {
                    task.AssignedToTechnical = new UserInfoDto
                    {
                        UserId = row.AssignedToTechnicalUserId,
                        FullName = row.AssignedToTechnicalFullName,
                        Email = row.AssignedToTechnicalEmail,
                        Phone = row.AssignedToTechnicalPhone
                    };
                }

                if (row.ConfirmedByUserId != null)
                {
                    task.ConfirmedBy = new UserInfoDto
                    {
                        UserId = row.ConfirmedByUserId,
                        FullName = row.ConfirmedByFullName,
                        Email = row.ConfirmedByEmail,
                        Phone = row.ConfirmedByPhone
                    };
                }

                tasks.Add(task);
            }

            detail.Parts = parts;
            detail.GarageServices = services.ToList();
            detail.TechnicalTasks = tasks;

            return detail;
        }

        /// <summary>
        /// Kiểm tra mã Service Ticket đã tồn tại chưa
        /// </summary>
        public async Task<bool> CheckCodeExistsAsync(string code, int? excludeId = null)
        {
            var sql = "SELECT COUNT(1) FROM service_ticket WHERE service_ticket_code = @Code AND is_deleted = 0";
            var parameters = new DynamicParameters();
            parameters.Add("@Code", code);

            if (excludeId.HasValue)
            {
                sql += " AND service_ticket_id != @ExcludeId";
                parameters.Add("@ExcludeId", excludeId.Value);
            }

            using var connection = new MySqlConnection(_connection);
            var count = await connection.QuerySingleAsync<int>(sql, parameters);
            return count > 0;
        }

        /// <summary>
        /// Lấy danh sách tasks của Mechanic có phân trang
        /// </summary>
        public async Task<PagedResult<MechanicTaskDto>> GetMechanicTasksAsync(int mechanicId, ServiceTicketFilterDtoRequest filter)
        {
            var parameters = new DynamicParameters();
            var whereConditions = new List<string>();

            // Base query
            var baseSelect = @"SELECT DISTINCT
                    tt.technical_task_id AS TechnicalTaskId,
                    tt.service_ticket_id AS ServiceTicketId,
                    st.service_ticket_code AS ServiceTicketCode,
                    tt.description AS Description,
                    tt.assigned_at AS AssignedAt,
                    CAST(tt.task_status AS UNSIGNED) AS TaskStatus,
                    tt.confirmed_at AS ConfirmedAt
                FROM technical_task tt
                INNER JOIN service_ticket st ON tt.service_ticket_id = st.service_ticket_id
                WHERE tt.assigned_to_technical = @MechanicId AND tt.is_deleted = 0 AND st.is_deleted = 0";

            parameters.Add("@MechanicId", mechanicId);

            // Xử lý ColumnFilters
            if (filter.ColumnFilters != null && filter.ColumnFilters.Any())
            {
                var filterIndex = 0;
                foreach (var columnFilter in filter.ColumnFilters)
                {
                    if (string.IsNullOrWhiteSpace(columnFilter.ColumnName) || 
                        string.IsNullOrWhiteSpace(columnFilter.Operator))
                    {
                        continue;
                    }

                    var paramName = $"@FilterValue{filterIndex}";
                    var columnName = GetColumnNameForFilter(columnFilter.ColumnName, "tt", "st");

                    switch (columnFilter.Operator.ToLower())
                    {
                        case "equals":
                            whereConditions.Add($"{columnName} = {paramName}");
                            parameters.Add(paramName, columnFilter.Value);
                            break;
                        case "not_equals":
                            whereConditions.Add($"{columnName} != {paramName}");
                            parameters.Add(paramName, columnFilter.Value);
                            break;
                        case "contains":
                            whereConditions.Add($"{columnName} LIKE {paramName}");
                            parameters.Add(paramName, $"%{columnFilter.Value}%");
                            break;
                        case "not_contains":
                            whereConditions.Add($"{columnName} NOT LIKE {paramName}");
                            parameters.Add(paramName, $"%{columnFilter.Value}%");
                            break;
                        case "starts_with":
                            whereConditions.Add($"{columnName} LIKE {paramName}");
                            parameters.Add(paramName, $"{columnFilter.Value}%");
                            break;
                        case "ends_with":
                            whereConditions.Add($"{columnName} LIKE {paramName}");
                            parameters.Add(paramName, $"%{columnFilter.Value}");
                            break;
                        case "empty":
                            whereConditions.Add($"({columnName} IS NULL OR {columnName} = '')");
                            break;
                        case "not_empty":
                            whereConditions.Add($"({columnName} IS NOT NULL AND {columnName} != '')");
                            break;
                        case "greater_than":
                            whereConditions.Add($"{columnName} > {paramName}");
                            parameters.Add(paramName, columnFilter.Value);
                            break;
                        case "less_than":
                            whereConditions.Add($"{columnName} < {paramName}");
                            parameters.Add(paramName, columnFilter.Value);
                            break;
                        case "greater_or_equal":
                            whereConditions.Add($"{columnName} >= {paramName}");
                            parameters.Add(paramName, columnFilter.Value);
                            break;
                        case "less_or_equal":
                            whereConditions.Add($"{columnName} <= {paramName}");
                            parameters.Add(paramName, columnFilter.Value);
                            break;
                    }
                    filterIndex++;
                }
            }

            // Build WHERE clause
            var whereClause = whereConditions.Any() 
                ? " AND " + string.Join(" AND ", whereConditions)
                : "";

            // Count query
            var countSql = $@"
                SELECT COUNT(DISTINCT tt.technical_task_id)
                FROM technical_task tt
                INNER JOIN service_ticket st ON tt.service_ticket_id = st.service_ticket_id
                WHERE tt.assigned_to_technical = @MechanicId AND tt.is_deleted = 0 AND st.is_deleted = 0
                {whereClause}";

            // Sort
            var orderBy = "ORDER BY tt.assigned_at DESC";
            if (filter.ColumnSorts != null && filter.ColumnSorts.Any())
            {
                var sortParts = filter.ColumnSorts
                    .Where(s => !string.IsNullOrWhiteSpace(s.ColumnName) && 
                               !string.IsNullOrWhiteSpace(s.SortDirection))
                    .Select(s => 
                    {
                        var columnName = GetColumnNameForSort(s.ColumnName, "tt", "st");
                        var direction = s.SortDirection.ToUpper() == "ASC" ? "ASC" : "DESC";
                        return $"{columnName} {direction}";
                    });
                
                if (sortParts.Any())
                {
                    orderBy = "ORDER BY " + string.Join(", ", sortParts);
                }
            }

            // Pagination
            var offset = (filter.Page - 1) * filter.PageSize;
            parameters.Add("@Offset", offset);
            parameters.Add("@PageSize", filter.PageSize);

            var dataSql = string.IsNullOrWhiteSpace(whereClause)
                ? $"{baseSelect} {orderBy} LIMIT @PageSize OFFSET @Offset"
                : $"{baseSelect} {whereClause} {orderBy} LIMIT @PageSize OFFSET @Offset";

            using var connection = new MySqlConnection(_connection);
            
            var total = await connection.QuerySingleAsync<int>(countSql, parameters);
            var items = await connection.QueryAsync<MechanicTaskDto>(dataSql, parameters);

            // Load thêm thông tin cho mỗi task
            var tasksList = items.ToList();
            foreach (var task in tasksList)
            {
                var taskDetail = await GetMechanicTaskDetailAsync(task.TechnicalTaskId, mechanicId);
                if (taskDetail != null)
                {
                    task.ServiceTicket = taskDetail.ServiceTicket;
                    task.Parts = taskDetail.Parts;
                    task.GarageServices = taskDetail.GarageServices;
                }
            }

            return new PagedResult<MechanicTaskDto>
            {
                Items = tasksList,
                Total = total,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }

        /// <summary>
        /// Lấy chi tiết task của Mechanic
        /// </summary>
        public async Task<MechanicTaskDto?> GetMechanicTaskDetailAsync(int technicalTaskId, int mechanicId)
        {
            var taskSql = @"
                SELECT 
                    tt.technical_task_id AS TechnicalTaskId,
                    tt.service_ticket_id AS ServiceTicketId,
                    st.service_ticket_code AS ServiceTicketCode,
                    tt.description AS Description,
                    tt.assigned_at AS AssignedAt,
                    CAST(tt.task_status AS UNSIGNED) AS TaskStatus,
                    tt.confirmed_at AS ConfirmedAt
                FROM technical_task tt
                INNER JOIN service_ticket st ON tt.service_ticket_id = st.service_ticket_id
                WHERE tt.technical_task_id = @TaskId AND tt.assigned_to_technical = @MechanicId 
                AND tt.is_deleted = 0 AND st.is_deleted = 0";

            var serviceTicketSql = @"
                SELECT 
                    st.service_ticket_id AS ServiceTicketId,
                    st.service_ticket_code AS ServiceTicketCode,
                    CAST(st.service_ticket_status AS UNSIGNED) AS ServiceTicketStatus,
                    st.initial_issue AS InitialIssue,
                    v.vehicle_id AS VehicleId,
                    v.vehicle_name AS VehicleName,
                    v.vehicle_license_plate AS VehicleLicensePlate,
                    v.make AS Make,
                    v.model AS Model,
                    v.current_km AS CurrentKm,
                    c.customer_id AS CustomerId,
                    c.customer_name AS CustomerName,
                    c.customer_phone AS CustomerPhone,
                    c.customer_email AS CustomerEmail
                FROM service_ticket st
                LEFT JOIN vehicle v ON st.vehicle_id = v.vehicle_id
                LEFT JOIN customer c ON v.customer_id = c.customer_id
                WHERE st.service_ticket_id = @ServiceTicketId AND st.is_deleted = 0";

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
                FROM service_ticket_detail std
                INNER JOIN part p ON std.part_id = p.part_id
                LEFT JOIN part_category pc ON p.part_category_id = pc.part_category_id
                WHERE std.service_ticket_id = @ServiceTicketId AND std.is_deleted = 0 
                AND std.part_id IS NOT NULL AND p.is_deleted = 0";

            var servicesSql = @"
                SELECT 
                    std.service_ticket_detail_id AS ServiceTicketDetailId,
                    gs.garage_service_id AS GarageServiceId,
                    gs.garage_service_name AS GarageServiceName,
                    gs.garage_service_price AS GarageServicePrice
                FROM `service_ticket_detail` std
                INNER JOIN `garage_service` gs ON std.garage_service_id = gs.garage_service_id
                WHERE std.service_ticket_id = @ServiceTicketId AND std.is_deleted = 0 
                AND std.garage_service_id IS NOT NULL AND gs.is_deleted = 0";

            var parameters = new DynamicParameters();
            parameters.Add("@TaskId", technicalTaskId);
            parameters.Add("@MechanicId", mechanicId);

            using var connection = new MySqlConnection(_connection);
            
            var task = await connection.QueryFirstOrDefaultAsync<MechanicTaskDto>(taskSql, parameters);
            if (task == null)
            {
                return null;
            }

            parameters.Add("@ServiceTicketId", task.ServiceTicketId);

            // Load service ticket info
            var serviceTicketData = await connection.QueryFirstOrDefaultAsync(serviceTicketSql, parameters);
            if (serviceTicketData != null)
            {
                task.ServiceTicket = new ServiceTicketInfoDto
                {
                    ServiceTicketId = serviceTicketData.ServiceTicketId,
                    ServiceTicketCode = serviceTicketData.ServiceTicketCode,
                    ServiceTicketStatus = serviceTicketData.ServiceTicketStatus,
                    InitialIssue = serviceTicketData.InitialIssue,
                    Vehicle = new VehicleInfoDto
                    {
                        VehicleId = serviceTicketData.VehicleId,
                        VehicleName = serviceTicketData.VehicleName ?? string.Empty,
                        VehicleLicensePlate = serviceTicketData.VehicleLicensePlate ?? string.Empty,
                        Make = serviceTicketData.Make,
                        Model = serviceTicketData.Model,
                        CurrentKm = serviceTicketData.CurrentKm
                    },
                    Customer = new CustomerInfoDto
                    {
                        CustomerId = serviceTicketData.CustomerId,
                        CustomerName = serviceTicketData.CustomerName,
                        CustomerPhone = serviceTicketData.CustomerPhone ?? string.Empty,
                        CustomerEmail = serviceTicketData.CustomerEmail
                    }
                };
            }

            // Load parts
            var partsData = await connection.QueryAsync(partsSql, parameters);
            var parts = new List<ServiceTicketDetailItemDto>();
            foreach (var row in partsData)
            {
                var partInfo = new PartInfoDto
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
                    partInfo.PartCategory = new PartCategoryInfoDto
                    {
                        PartCategoryId = row.PartCategoryId,
                        PartCategoryName = row.PartCategoryName,
                        PartCategoryCode = row.PartCategoryCode
                    };
                }

                parts.Add(new ServiceTicketDetailItemDto
                {
                    ServiceTicketDetailId = row.ServiceTicketDetailId,
                    Part = partInfo,
                    Quantity = row.Quantity
                });
            }

            // Map garage services thủ công vì có nested object
            var servicesData = await connection.QueryAsync(servicesSql, parameters);
            var services = new List<ServiceTicketDetailServiceDto>();
            foreach (var row in servicesData)
            {
                services.Add(new ServiceTicketDetailServiceDto
                {
                    ServiceTicketDetailId = row.ServiceTicketDetailId,
                    GarageService = new GarageServiceInfoDto
                    {
                        GarageServiceId = row.GarageServiceId,
                        GarageServiceName = row.GarageServiceName,
                        GarageServicePrice = row.GarageServicePrice
                    }
                });
            }

            task.Parts = parts;
            task.GarageServices = services;

            return task;
        }

        /// <summary>
        /// Lấy tên cột để filter (map từ tên property sang tên cột database)
        /// </summary>
        private string GetColumnNameForFilter(string columnName, string tablePrefix = "st", string tablePrefix2 = "")
        {
            // Map các tên property sang tên cột database
            var columnMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "ServiceTicketCode", $"{tablePrefix}.service_ticket_code" },
                { "ServiceTicketStatus", $"{tablePrefix}.service_ticket_status" },
                { "CustomerName", "c.customer_name" },
                { "CustomerPhone", "c.customer_phone" },
                { "VehicleName", "v.vehicle_name" },
                { "VehicleLicensePlate", "v.vehicle_license_plate" },
                { "CreatedByName", "u1.full_name" },
                { "CreatedDate", $"{tablePrefix}.created_date" },
                { "TaskStatus", "tt.task_status" },
                { "AssignedAt", "tt.assigned_at" },
                { "AssignedToTechnicalName", "(SELECT u3.full_name FROM technical_task tt LEFT JOIN users u3 ON tt.assigned_to_technical = u3.user_id WHERE tt.service_ticket_id = st.service_ticket_id LIMIT 1)" }
            };

            if (columnMap.TryGetValue(columnName, out var mappedColumn))
            {
                return mappedColumn;
            }

            // Nếu không có trong map, thử prefix với table prefix
            return $"{tablePrefix}.{columnName.ToLower()}";
        }

        /// <summary>
        /// Lấy tên cột để sort (map từ tên property sang tên cột database)
        /// </summary>
        private string GetColumnNameForSort(string columnName, string tablePrefix = "st", string tablePrefix2 = "")
        {
            // Map các tên property sang tên cột database
            var columnMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "ServiceTicketCode", $"{tablePrefix}.service_ticket_code" },
                { "ServiceTicketStatus", $"{tablePrefix}.service_ticket_status" },
                { "CustomerName", "c.customer_name" },
                { "CustomerPhone", "c.customer_phone" },
                { "VehicleName", "v.vehicle_name" },
                { "VehicleLicensePlate", "v.vehicle_license_plate" },
                { "CreatedByName", "u1.full_name" },
                { "CreatedDate", $"{tablePrefix}.created_date" },
                { "ModifiedDate", $"{tablePrefix}.modified_date" },
                { "TaskStatus", "tt.task_status" },
                { "AssignedAt", "tt.assigned_at" }
            };

            if (columnMap.TryGetValue(columnName, out var mappedColumn))
            {
                return mappedColumn;
            }

            // Nếu không có trong map, thử prefix với table prefix
            return $"{tablePrefix}.{columnName.ToLower()}";
        }

        /// <summary>
        /// Lấy danh sách Service Ticket Detail theo Service Ticket ID
        /// </summary>
        public async Task<List<ServiceTicketDetail>> GetServiceTicketDetailsAsync(int serviceTicketId)
        {
            var sql = "SELECT * FROM `service_ticket_detail` WHERE `service_ticket_id` = @ServiceTicketId AND `is_deleted` = 0";
            using var connection = new MySqlConnection(_connection);
            var result = await connection.QueryAsync<ServiceTicketDetail>(sql, new { ServiceTicketId = serviceTicketId });
            return result.ToList();
        }
    }
}

