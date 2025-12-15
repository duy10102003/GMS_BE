using Dapper;
using Microsoft.Extensions.Configuration;
using SWP.Core.Dtos;
using SWP.Core.Dtos.InvoiceDto;
using SWP.Core.Dtos.SeriveTicketDto;
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
            _connection = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("DefaultConnection");
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
            var sql = @"
                SELECT 
                    i.invoice_id AS InvoiceId,
                    i.service_ticket_id AS ServiceTicketId,
                    st.service_ticket_code AS ServiceTicketCode,
                    i.issue_date AS IssueDate,
                    i.parts_amount AS PartsAmount,
                    i.garage_service_amount AS GarageServiceAmount,
                    i.tax_amount AS TaxAmount,
                    i.discount_amount AS DiscountAmount,
                    i.total_amount AS TotalAmount,
                    CAST(i.invoice_status AS UNSIGNED) AS InvoiceStatus,
                    i.invoice_code AS InvoiceCode
                FROM `invoice` i
                INNER JOIN `service_ticket` st ON i.service_ticket_id = st.service_ticket_id
                WHERE i.invoice_id = @Id AND i.is_deleted = 0";

            var customerSql = @"
                SELECT 
                    c.customer_id AS CustomerId,
                    c.customer_name AS CustomerName,
                    c.customer_phone AS CustomerPhone,
                    c.customer_email AS CustomerEmail
                FROM `customer` c
                INNER JOIN `invoice` i ON c.customer_id = i.customer_id
                WHERE i.invoice_id = @Id AND c.is_deleted = 0";

            var vehicleSql = @"
                SELECT 
                    v.vehicle_id AS VehicleId,
                    v.vehicle_name AS VehicleName,
                    v.vehicle_license_plate AS VehicleLicensePlate,
                    v.make AS Make,
                    v.model AS Model,
                    v.current_km AS CurrentKm
                FROM `vehicle` v
                INNER JOIN `service_ticket` st ON v.vehicle_id = st.vehicle_id
                INNER JOIN `invoice` i ON st.service_ticket_id = i.service_ticket_id
                WHERE i.invoice_id = @Id AND v.is_deleted = 0";

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
                INNER JOIN `invoice` i ON std.service_ticket_id = i.service_ticket_id
                WHERE i.invoice_id = @Id AND std.is_deleted = 0 AND std.part_id IS NOT NULL AND p.is_deleted = 0";

            var servicesSql = @"
                SELECT 
                    std.service_ticket_detail_id AS ServiceTicketDetailId,
                    gs.garage_service_id AS GarageServiceId,
                    gs.garage_service_name AS GarageServiceName,
                    gs.garage_service_price AS GarageServicePrice
                FROM `service_ticket_detail` std
                INNER JOIN `garage_service` gs ON std.garage_service_id = gs.garage_service_id
                INNER JOIN `invoice` i ON std.service_ticket_id = i.service_ticket_id
                WHERE i.invoice_id = @Id AND std.is_deleted = 0 AND std.garage_service_id IS NOT NULL AND gs.is_deleted = 0";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);

            using var connection = new MySqlConnection(_connection);

            // Query invoice info
            var invoiceData = await connection.QueryFirstOrDefaultAsync(sql, parameters);
            if (invoiceData == null)
            {
                return null;
            }

            // Map invoice info
            var detail = new InvoiceDetailDto
            {
                InvoiceId = invoiceData.InvoiceId,
                ServiceTicketId = invoiceData.ServiceTicketId,
                ServiceTicketCode = invoiceData.ServiceTicketCode,
                IssueDate = invoiceData.IssueDate,
                PartsAmount = invoiceData.PartsAmount,
                GarageServiceAmount = invoiceData.GarageServiceAmount,
                TaxAmount = invoiceData.TaxAmount,
                DiscountAmount = invoiceData.DiscountAmount,
                TotalAmount = invoiceData.TotalAmount,
                InvoiceStatus = invoiceData.InvoiceStatus != null ? (byte?)Convert.ToByte(invoiceData.InvoiceStatus) : null,
                InvoiceCode = invoiceData.InvoiceCode
            };

            // Map customer info
            var customerInfo = await connection.QueryFirstOrDefaultAsync<CustomerInfoDto>(customerSql, parameters);
            if (customerInfo != null) detail.Customer = customerInfo;

            // Map vehicle info
            var vehicleInfo = await connection.QueryFirstOrDefaultAsync<VehicleInfoDto>(vehicleSql, parameters);
            if (vehicleInfo != null) detail.Vehicle = vehicleInfo;

            // Map parts
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

            detail.Parts = parts;
            detail.GarageServices = services;

            return detail;
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

        public async Task MarkAsPaidAsync(int invoiceId)
        {
            var sql = @"
        UPDATE invoice
        SET invoice_status = 1
        WHERE invoice_id = @InvoiceId";
            using var connection = new MySqlConnection(_connection);
            await connection.ExecuteAsync(sql, new { InvoiceId = invoiceId });
        }

        public async Task MarkAsFailedAsync(int invoiceId)
        {
            var sql = @"
        UPDATE invoice
        SET invoice_status = 0
        WHERE invoice_id = @InvoiceId";
            using var connection = new MySqlConnection(_connection);
            await connection.ExecuteAsync(sql, new { InvoiceId = invoiceId });
        }


        public async Task<Invoice?> GetByIdAsync(int invoiceId)
        {
            const string sql = @"
                SELECT 
                    invoice_id        AS InvoiceId,
                    service_ticket_id AS ServiceTicketId,
                    customer_id       AS CustomerId,
                    issue_date         AS IssueDate,
                    parts_amount       AS PartsAmount,
                    garage_service_amount AS GarageServiceAmount,
                    tax_amount         AS TaxAmount,
                    discount_amount    AS DiscountAmount,
                    total_amount       AS TotalAmount,
                    invoice_status     AS InvoiceStatus,
                    invoice_code       AS InvoiceCode,
                    is_deleted         AS IsDeleted
                FROM invoice
                WHERE invoice_id = @InvoiceId
                  AND is_deleted = 0
                LIMIT 1;
            ";

            using var connection = new MySqlConnection(_connection);

            return await connection.QueryFirstOrDefaultAsync<Invoice>(
                sql,
                new { InvoiceId = invoiceId }
            );
        }

    }
}


