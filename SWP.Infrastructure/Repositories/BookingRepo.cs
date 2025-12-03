using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Collections.Generic;
using System.Linq;
using SWP.Core.Dtos;
using SWP.Core.Dtos.BookingDto;
using SWP.Core.Entities;
using SWP.Core.Interfaces.Repositories;
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
            var where = new List<string> { "b.is_deleted = 0" };
            var parameters = new DynamicParameters();

            if (filter.Status.HasValue)
            {
                where.Add("b.booking_status = @Status");
                parameters.Add("@Status", filter.Status.Value);
            }

            if (filter.CustomerId.HasValue)
            {
                where.Add("b.customer_id = @CustomerId");
                parameters.Add("@CustomerId", filter.CustomerId.Value);
            }

            if (filter.ColumnFilters != null && filter.ColumnFilters.Any())
            {
                var idx = 0;
                foreach (var cf in filter.ColumnFilters)
                {
                    if (string.IsNullOrWhiteSpace(cf.ColumnName) || string.IsNullOrWhiteSpace(cf.Operator))
                        continue;

                    var param = $"@f{idx}";
                    switch (cf.ColumnName.ToLower())
                    {
                        case "customername":
                            AppendFilter(where, cf.Operator, "c.customer_name", param, cf.Value, parameters);
                            break;
                        case "customerphone":
                            AppendFilter(where, cf.Operator, "c.customer_phone", param, cf.Value, parameters);
                            break;
                        case "vehiclename":
                            AppendFilter(where, cf.Operator, "b.vehicle_name", param, cf.Value, parameters);
                            break;
                    }
                    idx++;
                }
            }

            var whereClause = where.Any() ? "WHERE " + string.Join(" AND ", where) : string.Empty;

            var countSql = $@"
                SELECT COUNT(*)
                FROM bookings b
                LEFT JOIN customer c ON b.customer_id = c.customer_id
                {whereClause}";

            var orderBy = "ORDER BY b.created_date DESC";
            if (filter.ColumnSorts != null && filter.ColumnSorts.Any())
            {
                var sort = filter.ColumnSorts
                    .Where(s => !string.IsNullOrWhiteSpace(s.ColumnName))
                    .Select(s =>
                    {
                        var dir = s.SortDirection?.ToUpper() == "ASC" ? "ASC" : "DESC";
                        return s.ColumnName.ToLower() switch
                        {
                            "customername" => $"c.customer_name {dir}",
                            "customerphone" => $"c.customer_phone {dir}",
                            "vehiclename" => $"b.vehicle_name {dir}",
                            "bookingtime" => $"b.booking_time {dir}",
                            "bookingstatus" => $"b.booking_status {dir}",
                            _ => $"b.created_date {dir}"
                        };
                    });
                if (sort.Any())
                {
                    orderBy = "ORDER BY " + string.Join(", ", sort);
                }
            }

            parameters.Add("@Offset", (filter.Page - 1) * filter.PageSize);
            parameters.Add("@PageSize", filter.PageSize);

            var dataSql = $@"
                SELECT 
                    b.booking_id AS BookingId,
                    b.booking_time AS BookingTime,
                    b.booking_status AS BookingStatus,
                    b.vehicle_name AS VehicleName,
                    c.customer_name AS CustomerName,
                    c.customer_phone AS CustomerPhone
                FROM bookings b
                LEFT JOIN customer c ON b.customer_id = c.customer_id
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
                    c.customer_name AS CustomerName,
                    c.customer_phone AS CustomerPhone,
                    b.created_date AS CreatedDate,
                    b.modified_date AS ModifiedDate
                FROM bookings b
                LEFT JOIN customer c ON b.customer_id = c.customer_id
                WHERE b.booking_id = @Id AND b.is_deleted = 0";

            using var connection = new MySqlConnection(_connection);
            return await connection.QueryFirstOrDefaultAsync<BookingDetailDto>(sql, new { Id = id });
        }

        private static void AppendFilter(List<string> where, string op, string column, string paramName, object? value, DynamicParameters parameters)
        {
            switch (op.ToLower())
            {
                case "contains":
                    where.Add($"{column} LIKE {paramName}");
                    parameters.Add(paramName, $"%{value}%");
                    break;
                case "equals":
                    where.Add($"{column} = {paramName}");
                    parameters.Add(paramName, value);
                    break;
                case "not_equals":
                    where.Add($"{column} != {paramName}");
                    parameters.Add(paramName, value);
                    break;
                case "starts_with":
                    where.Add($"{column} LIKE {paramName}");
                    parameters.Add(paramName, $"{value}%");
                    break;
                case "ends_with":
                    where.Add($"{column} LIKE {paramName}");
                    parameters.Add(paramName, $"%{value}");
                    break;
                case "empty":
                    where.Add($"({column} IS NULL OR {column} = '')");
                    break;
                case "not_empty":
                    where.Add($"({column} IS NOT NULL AND {column} != '')");
                    break;
            }
        }
    }
}
