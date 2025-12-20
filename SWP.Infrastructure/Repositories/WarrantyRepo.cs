using Dapper;
using Microsoft.Extensions.Configuration;
using MISA.QLSX.Infrastructure.Repositories;
using MySqlConnector;
using SWP.Core.Dtos;
using SWP.Core.Dtos.WarrantyDto;
using SWP.Core.Entities;
using SWP.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWP.Infrastructure.Repositories
{
    public class WarrantyRepo : BaseRepo<Warranty>, IWarrantyRepo
    {
        private readonly string _connection;
        public WarrantyRepo(IConfiguration configuration) : base(configuration)
        {
            _connection = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("DefaultConnection");

        }
        public async Task<int> AutoUpdateExpiredAsync()
        {
            const string sql = @"
        UPDATE warranty
        SET status = 1
        WHERE is_deleted = 0
          AND status = 0
          AND end_date < NOW();
    ";

            using var conn = new MySqlConnection(_connection);
            return await conn.ExecuteAsync(sql);
        }
        public async Task CreateWarrantyForServiceTicketAsync(int serviceTicketId)
        {
            using var conn = new MySqlConnection(_connection);
            await conn.OpenAsync();

            using var tran = conn.BeginTransaction();

            try
            {
                //  Kiểm tra service ticket đã hoàn thành & thanh toán

                // Lấy danh sách detail + warranty month
                var details = await conn.QueryAsync<(int DetailId, int WarrantyMonth)>(
                    @"
            SELECT 
                std.service_ticket_detail_id AS DetailId,
                p.warranty_month AS WarrantyMonth
            FROM service_ticket_detail std
            JOIN part p ON std.part_id = p.part_id
            WHERE std.service_ticket_id = @serviceTicketId
              AND std.is_deleted = 0
              AND std.part_id IS NOT NULL
              AND p.is_deleted = 0
              AND p.warranty_month > 0;
            ",
                    new { serviceTicketId },
                    tran
                );

                if (!details.Any())
                {
                    await tran.CommitAsync();
                    return;
                }

                // 3. Insert warranty
                foreach (var item in details)
                {
                    var startDate = DateTime.Now;
                    var endDate = startDate.AddMonths(item.WarrantyMonth);

                    await conn.ExecuteAsync(
                        @"
                INSERT INTO warranty
                (
                    service_ticket_detail_id,
                    start_date,
                    end_date,
                    status,
                    is_deleted
                )
                SELECT 
                    @ServiceTicketDetailId,
                    @StartDate,
                    @EndDate,
                    0,
                    0
                FROM DUAL
                WHERE NOT EXISTS
                (
                    SELECT 1 FROM warranty
                    WHERE service_ticket_detail_id = @ServiceTicketDetailId
                      AND is_deleted = 0
                );
                ",
                        new
                        {
                            ServiceTicketDetailId = item.DetailId,
                            StartDate = startDate,
                            EndDate = endDate
                        },
                        tran
                    );
                }

                await tran.CommitAsync();
            }
            catch
            {
                await tran.RollbackAsync();
                throw;
            }
        }

        public async Task<Warranty?> GetByServiceTicketDetailIdAsync(int serviceTicketDetailId)
        {
            var sql = @"
            SELECT *
            FROM warranty
            WHERE service_ticket_detail_id = @ServiceTicketDetailId
              AND is_deleted = 0";

            using var conn = new MySqlConnection(_connection);
            return await conn.QueryFirstOrDefaultAsync<Warranty>(
                sql,
                new { ServiceTicketDetailId = serviceTicketDetailId });
        }

        public async Task<PagedResult<WarrantyListItemDto>> GetPagingAsync(WarrantyFilterDtoRequest filter)
        {
            // await AutoUpdateExpiredAsync();


            var parameters = new DynamicParameters();
            var whereClause = BuildWhereClause(filter.ColumnFilters, parameters);
            var orderBy = BuildOrderByClause(filter.ColumnSorts);
            // var whereCoditions = new List<string>();
            var offset = (filter.Page - 1) * filter.PageSize;
            parameters.Add("@Offset", offset);
            parameters.Add("@PageSize", filter.PageSize);
            var whereConditions = new List<string>();

            if (!string.IsNullOrWhiteSpace(whereClause))
            {
                whereConditions.Add(whereClause);
            }

            if (!string.IsNullOrWhiteSpace(filter.KeyWord))
            {
                whereConditions.Add(@"
        AND (
            LOWER(p.part_name) LIKE @keyword
            OR LOWER(v.vehicle_license_plate) LIKE @keyword
            OR LOWER(c.customer_phone) LIKE @keyword
            OR LOWER(c.customer_name) LIKE @keyword
        )
    ");

                parameters.Add("@keyword", $"%{filter.KeyWord.Trim().ToLower()}%");
            }

            var finalWhere = string.Join(" ", whereConditions);

            var baseFrom = @"
        FROM warranty w
        INNER JOIN service_ticket_detail std ON w.service_ticket_detail_id = std.service_ticket_detail_id
        INNER JOIN part p ON std.part_id = p.part_id
        INNER JOIN service_ticket st ON std.service_ticket_id = st.service_ticket_id
        INNER JOIN vehicle v ON st.vehicle_id = v.vehicle_id
        INNER JOIN customer c ON v.customer_id = c.customer_id
        WHERE w.is_deleted = 0";

            var countSql = $@"
    SELECT COUNT(1)
    {baseFrom}
    {finalWhere}
";


          var dataSql = $@"
    SELECT 
        w.warranty_id AS WarrantyId,
        w.service_ticket_detail_id AS ServiceTicketDetailId,
        p.part_name AS PartName,
        p.part_code AS PartCode,
        v.vehicle_license_plate AS VehicleLicensePlate,
        c.customer_name AS CustomerName,
        c.customer_phone AS CustomerPhone,
        c.customer_email AS CustomerEmail,
        w.start_date AS StartDate,
        w.end_date AS EndDate,
        w.status AS Status
    {baseFrom}
    {finalWhere}
    {orderBy}
    LIMIT @PageSize OFFSET @Offset
";

            using var conn = new MySqlConnection(_connection);

            var total = await conn.QuerySingleAsync<int>(countSql, parameters);
            var items = await conn.QueryAsync<WarrantyListItemDto>(dataSql, parameters);

            return new PagedResult<WarrantyListItemDto>
            {
                Items = items,
                Total = total,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }

        public async Task<PagedResult<WarrantyListItemDto>> GetPagingByCustomerAsync(int customerId, WarrantyFilterDtoRequest filter)
        {
            //await AutoUpdateExpiredAsync();
            var parameters = new DynamicParameters();

            var whereClause = BuildWhereClause(filter.ColumnFilters, parameters);
            var orderBy = BuildOrderByClause(filter.ColumnSorts);

            var offset = (filter.Page - 1) * filter.PageSize;
            parameters.Add("@Offset", offset);
            parameters.Add("@PageSize", filter.PageSize);

            var baseFrom = @"
        FROM warranty w
        INNER JOIN service_ticket_detail std ON w.service_ticket_detail_id = std.service_ticket_detail_id
        INNER JOIN part p ON std.part_id = p.part_id
        INNER JOIN service_ticket st ON std.service_ticket_id = st.service_ticket_id
        INNER JOIN vehicle v ON st.vehicle_id = v.vehicle_id
        INNER JOIN customer c ON v.customer_id = c.customer_id
        WHERE 
            w.is_deleted = 0
            AND c.customer_id = @CustomerId";

            var countSql = $"SELECT COUNT(1) {baseFrom}{whereClause}";

            var dataSql = $@"
        SELECT 
            w.warranty_id AS WarrantyId,
            w.service_ticket_detail_id AS ServiceTicketDetailId,
            p.part_name AS PartName,
            p.part_code AS PartCode,
            v.vehicle_license_plate AS VehicleLicensePlate,
            c.customer_name AS CustomerName,
            c.customer_phone AS CustomerPhone,
            w.start_date AS StartDate,
            w.end_date AS EndDate,
            w.status AS Status
        {baseFrom}
        {whereClause}
        {orderBy}
        LIMIT @PageSize OFFSET @Offset";

            using var conn = new MySqlConnection(_connection);
            var customerId1 = await conn.ExecuteScalarAsync<int?>(
                "SELECT customer_id FROM customer WHERE user_id = @UserId AND is_deleted = 0 LIMIT 1",
                 new { UserId = customerId }
            );
            parameters.Add("@CustomerId", customerId1);
            var total = await conn.QuerySingleAsync<int>(countSql, parameters);
            var items = await conn.QueryAsync<WarrantyListItemDto>(dataSql, parameters);

            return new PagedResult<WarrantyListItemDto>
            {
                Items = items,
                Total = total,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }

        #region Helper
        private static readonly Dictionary<string, string> WarrantyColumnMap = new()
{
                { "PartName", "p.part_name" },
                { "PartCode", "p.part_code" },
                { "CustomerName", "c.customer_name" },
                { "CustomerPhone", "c.customer_phone" },
                { "VehicleLicensePlate", "v.vehicle_license_plate" },
                { "StartDate", "w.start_date" },
                { "EndDate", "w.end_date" },
                { "Status", "w.status" }
        };

        private string BuildWhereClause(
    List<ColumnFilterDto>? filters,
    DynamicParameters parameters)
        {
            if (filters == null || !filters.Any())
                return string.Empty;

            var conditions = new List<string>();
            var index = 0;

            foreach (var filter in filters)
            {
                if (!WarrantyColumnMap.TryGetValue(filter.ColumnName, out var column))
                    continue;

                var paramName = $"@Filter{index}";

                conditions.Add($"{column} LIKE {paramName}");
                parameters.Add(paramName, $"%{filter.Value}%");

                index++;
            }

            return " AND " + string.Join(" AND ", conditions);
        }

        private string BuildOrderByClause(List<ColumnSortDto>? sorts)
        {
            if (sorts == null || !sorts.Any())
                return " ORDER BY w.warranty_id DESC";

            var orders = new List<string>();

            foreach (var sort in sorts)
            {
                if (!WarrantyColumnMap.TryGetValue(sort.ColumnName, out var column))
                    continue;

                var dir = sort.SortDirection == "DESC" ? "DESC" : "ASC";
                orders.Add($"{column} {dir}");
            }

            return orders.Any()
                ? " ORDER BY " + string.Join(", ", orders)
                : " ORDER BY w.warranty_id DESC";
        }


        #endregion
    }
}
