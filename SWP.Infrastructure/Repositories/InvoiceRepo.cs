using Dapper;
using Microsoft.Extensions.Configuration;
using SWP.Core.Dtos;
using SWP.Core.Dtos.InvoiceDto;
using SWP.Core.Entities;
using SWP.Core.Interfaces.Repositories;
using MySqlConnector;
using MISA.QLSX.Infrastructure.Repositories;

namespace SWP.Infrastructure.Repositories
{
    /// <summary>
    /// Repository cho Invoice
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public class InvoiceRepo : BaseRepo<Invoice>, IInvoiceRepo
    {
        private readonly string _connection;

        public InvoiceRepo(IConfiguration configuration) : base(configuration)
        {
            _connection = configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Lấy danh sách Invoice có phân trang
        /// </summary>
        public async Task<PagedResult<InvoiceListItemDto>> GetPagingAsync(InvoiceFilterDtoRequest filter)
        {
            var parameters = new DynamicParameters();
            var whereConditions = new List<string>();

            // Base query
            var baseSelect = @"SELECT 
                    i.invoice_id AS InvoiceId,
                    i.service_ticket_id AS ServiceTicketId,
                    st.service_ticket_code AS ServiceTicketCode,
                    i.customer_id AS CustomerId,
                    c.customer_name AS CustomerName,
                    c.customer_phone AS CustomerPhone,
                    i.issue_date AS IssueDate,
                    i.parts_amount AS PartsAmount,
                    i.garage_service_amount AS GarageServiceAmount,
                    i.tax_amount AS TaxAmount,
                    i.discount_amount AS DiscountAmount,
                    i.total_amount AS TotalAmount,
                    i.invoice_status AS InvoiceStatus,
                    i.invoice_code AS InvoiceCode
                FROM `invoice` i
                INNER JOIN `service_ticket` st ON i.service_ticket_id = st.service_ticket_id
                LEFT JOIN `customer` c ON i.customer_id = c.customer_id
                WHERE i.is_deleted = 0";

            // Filter theo customer
            if (filter.CustomerId.HasValue)
            {
                whereConditions.Add("i.customer_id = @CustomerId");
                parameters.Add("@CustomerId", filter.CustomerId.Value);
            }

            // Filter theo status
            if (filter.InvoiceStatus.HasValue)
            {
                whereConditions.Add("i.invoice_status = @InvoiceStatus");
                parameters.Add("@InvoiceStatus", filter.InvoiceStatus.Value);
            }

            // Filter theo date range
            if (filter.FromDate.HasValue)
            {
                whereConditions.Add("i.issue_date >= @FromDate");
                parameters.Add("@FromDate", filter.FromDate.Value);
            }

            if (filter.ToDate.HasValue)
            {
                whereConditions.Add("i.issue_date <= @ToDate");
                parameters.Add("@ToDate", filter.ToDate.Value);
            }

            // Build WHERE clause
            var whereClause = whereConditions.Any() 
                ? " AND " + string.Join(" AND ", whereConditions)
                : "";

            // Count query
            var countSql = $@"
                SELECT COUNT(1)
                FROM `invoice` i
                WHERE i.is_deleted = 0{whereClause}";

            // Sort
            var orderBy = "ORDER BY i.invoice_id DESC";

            // Pagination
            var offset = (filter.Page - 1) * filter.PageSize;
            parameters.Add("@Offset", offset);
            parameters.Add("@PageSize", filter.PageSize);

            var dataSql = $"{baseSelect}{whereClause} {orderBy} LIMIT @PageSize OFFSET @Offset";

            using var connection = new MySqlConnection(_connection);
            
            var total = await connection.QuerySingleAsync<int>(countSql, parameters);
            var items = await connection.QueryAsync<InvoiceListItemDto>(dataSql, parameters);

            return new PagedResult<InvoiceListItemDto>
            {
                Items = items,
                Total = total,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }

        /// <summary>
        /// Lấy chi tiết Invoice theo ID
        /// </summary>
        public async Task<InvoiceDetailDto?> GetDetailAsync(int id)
        {
            // Query chi tiết invoice với customer, vehicle, parts, services
            // Implementation tương tự ServiceTicketDetailDto
            // TODO: Implement full detail query
            var sql = @"
                SELECT 
                    i.invoice_id AS InvoiceId,
                    i.service_ticket_id AS ServiceTicketId,
                    st.service_ticket_code AS ServiceTicketCode,
                    i.customer_id AS CustomerId,
                    c.customer_name AS CustomerName,
                    c.customer_phone AS CustomerPhone,
                    c.customer_email AS CustomerEmail,
                    v.vehicle_id AS VehicleId,
                    v.vehicle_name AS VehicleName,
                    v.vehicle_license_plate AS VehicleLicensePlate,
                    i.issue_date AS IssueDate,
                    i.parts_amount AS PartsAmount,
                    i.garage_service_amount AS GarageServiceAmount,
                    i.tax_amount AS TaxAmount,
                    i.discount_amount AS DiscountAmount,
                    i.total_amount AS TotalAmount,
                    i.invoice_status AS InvoiceStatus,
                    i.invoice_code AS InvoiceCode
                FROM `invoice` i
                INNER JOIN `service_ticket` st ON i.service_ticket_id = st.service_ticket_id
                LEFT JOIN `customer` c ON i.customer_id = c.customer_id
                LEFT JOIN `vehicle` v ON st.vehicle_id = v.vehicle_id
                WHERE i.invoice_id = @Id AND i.is_deleted = 0";

            using var connection = new MySqlConnection(_connection);
            // TODO: Map to InvoiceDetailDto with parts and services
            return null;
        }

        /// <summary>
        /// Lấy Invoice theo Service Ticket ID
        /// </summary>
        public async Task<Invoice?> GetByServiceTicketIdAsync(int serviceTicketId)
        {
            var sql = "SELECT * FROM `invoice` WHERE `service_ticket_id` = @ServiceTicketId AND `is_deleted` = 0";
            using var connection = new MySqlConnection(_connection);
            return await connection.QueryFirstOrDefaultAsync<Invoice>(sql, new { ServiceTicketId = serviceTicketId });
        }
    }
}

