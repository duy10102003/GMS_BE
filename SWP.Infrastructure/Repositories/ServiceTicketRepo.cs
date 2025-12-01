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
            var baseSelect = @"
                SELECT DISTINCT
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
                    st.service_ticket_status AS ServiceTicketStatus,
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
                    var columnName = GetColumnNameForFilter(columnFilter.ColumnName);

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
                        var columnName = GetColumnNameForSort(s.ColumnName);
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

            var dataSql = $@"
                {baseSelect}
                {whereClause}
                {orderBy}
                LIMIT @PageSize OFFSET @Offset";

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
        public async Task<ServiceTicketDetailDto?> GetDetailAsync(Guid id)
        {
            var sql = @"
                SELECT 
                    st.service_ticket_id AS ServiceTicketId,
                    st.service_ticket_code AS ServiceTicketCode,
                    st.booking_id AS BookingId,
                    st.vehicle_id AS VehicleId,
                    v.vehicle_name AS VehicleName,
                    v.vehicle_license_plate AS VehicleLicensePlate,
                    v.make AS VehicleMake,
                    v.model AS VehicleModel,
                    v.current_km AS VehicleCurrentKm,
                    c.customer_id AS CustomerId,
                    c.customer_name AS CustomerName,
                    c.customer_phone AS CustomerPhone,
                    c.customer_email AS CustomerEmail,
                    st.created_by AS CreatedBy,
                    u1.full_name AS CreatedByName,
                    st.created_date AS CreatedDate,
                    st.modified_by AS ModifiedBy,
                    u2.full_name AS ModifiedByName,
                    st.modified_date AS ModifiedDate,
                    st.service_ticket_status AS ServiceTicketStatus,
                    st.initial_issue AS InitialIssue
                FROM service_ticket st
                LEFT JOIN vehicle v ON st.vehicle_id = v.vehicle_id
                LEFT JOIN customer c ON v.customer_id = c.customer_id
                LEFT JOIN users u1 ON st.created_by = u1.user_id
                LEFT JOIN users u2 ON st.modified_by = u2.user_id
                WHERE st.service_ticket_id = @Id";

            var partsSql = @"
                SELECT 
                    std.service_ticket_detail_id AS ServiceTicketDetailId,
                    std.part_id AS PartId,
                    p.part_name AS PartName,
                    p.part_code AS PartCode,
                    p.part_price AS PartPrice,
                    std.quantity AS Quantity,
                    p.part_unit AS PartUnit
                FROM service_ticket_detail std
                INNER JOIN part p ON std.part_id = p.part_id
                WHERE std.service_ticket_id = @Id";

            var tasksSql = @"
                SELECT 
                    tt.technical_task_id AS TechnicalTaskId,
                    tt.description AS Description,
                    tt.assigned_to_technical AS AssignedToTechnical,
                    u.full_name AS AssignedToTechnicalName,
                    tt.assigned_at AS AssignedAt,
                    tt.task_status AS TaskStatus,
                    tt.confirmed_by AS ConfirmedBy,
                    u2.full_name AS ConfirmedByName,
                    tt.confirmed_at AS ConfirmedAt
                FROM technical_task tt
                LEFT JOIN users u ON tt.assigned_to_technical = u.user_id
                LEFT JOIN users u2 ON tt.confirmed_by = u2.user_id
                WHERE tt.service_ticket_id = @Id";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);

            using var connection = new MySqlConnection(_connection);
            
            var detail = await connection.QueryFirstOrDefaultAsync<ServiceTicketDetailDto>(sql, parameters);
            if (detail == null)
            {
                return null;
            }

            var parts = await connection.QueryAsync<ServiceTicketDetailItemDto>(partsSql, parameters);
            var tasks = await connection.QueryAsync<TechnicalTaskDto>(tasksSql, parameters);

            detail.Parts = parts.ToList();
            detail.TechnicalTasks = tasks.ToList();

            return detail;
        }

        /// <summary>
        /// Kiểm tra mã Service Ticket đã tồn tại chưa
        /// </summary>
        public async Task<bool> CheckCodeExistsAsync(string code, Guid? excludeId = null)
        {
            var sql = "SELECT COUNT(1) FROM service_ticket WHERE service_ticket_code = @Code";
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
        /// Lấy tên cột để filter (map từ tên property sang tên cột database)
        /// </summary>
        private string GetColumnNameForFilter(string columnName)
        {
            // Map các tên property sang tên cột database
            var columnMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "ServiceTicketCode", "st.service_ticket_code" },
                { "ServiceTicketStatus", "st.service_ticket_status" },
                { "CustomerName", "c.customer_name" },
                { "CustomerPhone", "c.customer_phone" },
                { "VehicleName", "v.vehicle_name" },
                { "VehicleLicensePlate", "v.vehicle_license_plate" },
                { "CreatedByName", "u1.full_name" },
                { "CreatedDate", "st.created_date" },
                { "AssignedToTechnicalName", "(SELECT u3.full_name FROM technical_task tt LEFT JOIN users u3 ON tt.assigned_to_technical = u3.user_id WHERE tt.service_ticket_id = st.service_ticket_id LIMIT 1)" }
            };

            if (columnMap.TryGetValue(columnName, out var mappedColumn))
            {
                return mappedColumn;
            }

            // Nếu không có trong map, thử prefix với st.
            return $"st.{columnName.ToLower()}";
        }

        /// <summary>
        /// Lấy tên cột để sort (map từ tên property sang tên cột database)
        /// </summary>
        private string GetColumnNameForSort(string columnName)
        {
            // Map các tên property sang tên cột database
            var columnMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "ServiceTicketCode", "st.service_ticket_code" },
                { "ServiceTicketStatus", "st.service_ticket_status" },
                { "CustomerName", "c.customer_name" },
                { "CustomerPhone", "c.customer_phone" },
                { "VehicleName", "v.vehicle_name" },
                { "VehicleLicensePlate", "v.vehicle_license_plate" },
                { "CreatedByName", "u1.full_name" },
                { "CreatedDate", "st.created_date" },
                { "ModifiedDate", "st.modified_date" }
            };

            if (columnMap.TryGetValue(columnName, out var mappedColumn))
            {
                return mappedColumn;
            }

            // Nếu không có trong map, thử prefix với st.
            return $"st.{columnName.ToLower()}";
        }
    }
}

