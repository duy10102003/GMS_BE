using Dapper;
using Microsoft.Extensions.Configuration;
using SWP.Core.Dtos;
using SWP.Core.Dtos.BookingDto;
using SWP.Core.Entities;
using SWP.Core.Interfaces.Repositories;
using MySqlConnector;
using System.Text;
using MISA.QLSX.Infrastructure.Repositories;

namespace SWP.Infrastructure.Repositories
{
    public class BookingRepo : BaseRepo<Booking>, IBookingRepo
    {
        private readonly string _connection;

        public BookingRepo(IConfiguration configuration) : base(configuration)
        {
            _connection = configuration.GetConnectionString("DefaultConnection")
                           ?? throw new ArgumentNullException("DefaultConnection");
        }

        public async Task<PagedResult<BookingListItemDto>> GetPagingAsync(BookingFilterDtoRequest filter)
        {
            var parameters = new DynamicParameters();
            var whereConditions = new List<string> { "b.is_deleted = 0" };

            if (filter.Status.HasValue)
            {
                whereConditions.Add("b.booking_status = @Status");
                parameters.Add("@Status", filter.Status.Value);
            }

            if (filter.CustomerId.HasValue)
            {
                whereConditions.Add("b.customer_id = @CustomerId");
                parameters.Add("@CustomerId", filter.CustomerId.Value);
            }

            // Column filters (simple mapping similar to ServiceTicketRepo)
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

                    var columnName = GetColumnNameForFilter(columnFilter.ColumnName);
                    if (string.IsNullOrEmpty(columnName))
                    {
                        continue; // skip unknown columns
                    }

                    var paramName = $"@FilterValue{filterIndex}";

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
                            parameters.Add(paramName, $"{columnFilter.Value}");
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

            var whereClause = whereConditions.Any()
                ? "WHERE " + string.Join(" AND ", whereConditions)
                : string.Empty;

            var orderBy = "ORDER BY b.created_date DESC";
            if (filter.ColumnSorts != null && filter.ColumnSorts.Any())
            {
                var sortParts = filter.ColumnSorts
                    .Where(s => !string.IsNullOrWhiteSpace(s.ColumnName) &&
                                !string.IsNullOrWhiteSpace(s.SortDirection))
                    .Select(s =>
                    {
                        var columnName = GetColumnNameForSort(s.ColumnName);
                        if (string.IsNullOrEmpty(columnName))
                        {
                            return null;
                        }
                        var direction = s.SortDirection.ToUpper() == "ASC" ? "ASC" : "DESC";
                        return $"{columnName} {direction}";
                    })
                    .Where(x => x != null)
                    .ToList();

                if (sortParts.Any())
                {
                    orderBy = "ORDER BY " + string.Join(", ", sortParts);
                }
            }

            var offset = (filter.Page - 1) * filter.PageSize;
            parameters.Add("@PageSize", filter.PageSize);
            parameters.Add("@Offset", offset);

            var baseSelect = @"
                FROM bookings b
                LEFT JOIN customer c ON b.customer_id = c.customer_id";

            var countSql = $@"
                SELECT COUNT(1)
                {baseSelect}
                {whereClause}";

            var dataSql = $@"
                SELECT
                    b.booking_id AS BookingId,
                    b.booking_time AS BookingTime,
                    b.booking_status AS BookingStatus,
                    b.vehicle_name AS VehicleName,
                    c.customer_name AS CustomerName,
                    c.customer_phone AS CustomerPhone
                {baseSelect}
                {whereClause}
                {orderBy}
                LIMIT @PageSize OFFSET @Offset";

            using var connection = new MySqlConnection(_connection);

            var total = await connection.QuerySingleAsync<int>(countSql, parameters);
            var items = await connection.QueryAsync<BookingListItemDto>(dataSql, parameters);

            return new PagedResult<BookingListItemDto>
            {
                Items = items,
                Total = total,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }

        public async Task<BookingDetailDto?> GetDetailAsync(int id)
        {
            var sql = @"
                SELECT 
                    b.booking_id AS BookingId,
                    b.booking_time AS BookingTime,
                    b.booking_status AS BookingStatus,
                    b.vehicle_name AS VehicleName,
                    b.note AS Note,
                    b.customer_id AS CustomerId,
                    b.created_date AS CreatedDate,
                    b.modified_date AS ModifiedDate,
                    c.customer_name AS CustomerName,
                    c.customer_phone AS CustomerPhone
                FROM bookings b
                LEFT JOIN customer c ON b.customer_id = c.customer_id
                WHERE b.booking_id = @Id AND b.is_deleted = 0";

            using var connection = new MySqlConnection(_connection);
            return await connection.QueryFirstOrDefaultAsync<BookingDetailDto>(sql, new { Id = id });
        }

        private string GetColumnNameForFilter(string columnName)
        {
            var columnMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "BookingTime", "b.booking_time" },
                { "BookingStatus", "b.booking_status" },
                { "VehicleName", "b.vehicle_name" },
                { "CustomerName", "c.customer_name" },
                { "CustomerPhone", "c.customer_phone" }
            };

            if (columnMap.TryGetValue(columnName, out var mapped))
            {
                return mapped;
            }

            // unknown column -> ignore
            return string.Empty;
        }

        private string GetColumnNameForSort(string columnName)
        {
            var columnMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "BookingTime", "b.booking_time" },
                { "BookingStatus", "b.booking_status" },
                { "VehicleName", "b.vehicle_name" },
                { "CustomerName", "c.customer_name" },
                { "CustomerPhone", "c.customer_phone" },
                { "CreatedDate", "b.created_date" },
                { "ModifiedDate", "b.modified_date" }
            };

            if (columnMap.TryGetValue(columnName, out var mapped))
            {
                return mapped;
            }

            // unknown column -> ignore
            return string.Empty;
        }
    }
}
