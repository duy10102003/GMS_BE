using Dapper;
using Microsoft.Extensions.Configuration;
using SWP.Core.Dtos;
using SWP.Core.Dtos.PartDto;
using SWP.Core.Entities;
using SWP.Core.Interfaces.Repositories;
using MySqlConnector;
using MISA.QLSX.Infrastructure.Repositories;

namespace SWP.Infrastructure.Repositories
{
    /// <summary>
    /// Repository cho Part
    /// Created by: DuyLC(02/12/2025)
    /// </summary>
    public class PartRepo : BaseRepo<Part>, IPartRepo
    {
        private readonly string _connection;

        public PartRepo(IConfiguration configuration) : base(configuration)
        {
            _connection = configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Lấy danh sách Part có phân trang
        /// </summary>
        public async Task<PagedResult<PartListItemDto>> GetPagingAsync(PartFilterDtoRequest filter)
        {
            var parameters = new DynamicParameters();
            var whereConditions = new List<string>();

            // Base query
            var baseSelect = @"SELECT 
                    p.part_id AS PartId,
                    p.part_name AS PartName,
                    p.part_code AS PartCode,
                    p.part_quantity AS PartQuantity,
                    p.part_unit AS PartUnit,
                    p.part_category_id AS PartCategoryId,
                    pc.part_category_name AS PartCategoryName,
                    pc.part_category_code AS PartCategoryCode,
                    p.part_price AS PartPrice,
                    p.warranty_month AS WarrantyMonth
                FROM part p
                LEFT JOIN part_category pc ON p.part_category_id = pc.part_category_id
                WHERE p.is_deleted = 0";

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
                ? " AND " + string.Join(" AND ", whereConditions)
                : "";

            // Count query
            var countSql = $@"
                SELECT COUNT(1)
                FROM part p
                WHERE p.is_deleted = 0{whereClause}";

            // Sort
            var orderBy = "ORDER BY p.part_id DESC";
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

            var dataSql = $"{baseSelect}{whereClause} {orderBy} LIMIT @PageSize OFFSET @Offset";

            using var connection = new MySqlConnection(_connection);
            
            var total = await connection.QuerySingleAsync<int>(countSql, parameters);
            var items = await connection.QueryAsync<PartListItemDto>(dataSql, parameters);

            return new PagedResult<PartListItemDto>
            {
                Items = items,
                Total = total,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }

        /// <summary>
        /// Lấy chi tiết Part theo ID
        /// </summary>
        public async Task<PartDetailDto?> GetDetailAsync(int id)
        {
            var sql = @"
                SELECT 
                    p.part_id AS PartId,
                    p.part_name AS PartName,
                    p.part_code AS PartCode,
                    p.part_quantity AS PartQuantity,
                    p.part_unit AS PartUnit,
                    p.part_category_id AS PartCategoryId,
                    pc.part_category_name AS PartCategoryName,
                    pc.part_category_code AS PartCategoryCode,
                    pc.part_category_discription AS PartCategoryDiscription,
                    pc.part_category_phone AS PartCategoryPhone,
                    pc.status AS Status,
                    p.part_price AS PartPrice,
                    p.warranty_month AS WarrantyMonth
                FROM part p
                LEFT JOIN part_category pc ON p.part_category_id = pc.part_category_id
                WHERE p.part_id = @Id AND p.is_deleted = 0";

            using var connection = new MySqlConnection(_connection);
            return await connection.QueryFirstOrDefaultAsync<PartDetailDto>(sql, new { Id = id });
        }

        /// <summary>
        /// Kiểm tra mã Part đã tồn tại hay chưa
        /// </summary>
        public async Task<bool> CheckCodeExistsAsync(string partCode, int? excludeId = null)
        {
            var sql = "SELECT COUNT(1) FROM part WHERE part_code = @PartCode AND is_deleted = 0";
            var parameters = new DynamicParameters();
            parameters.Add("@PartCode", partCode);

            if (excludeId.HasValue)
            {
                sql += " AND part_id != @ExcludeId";
                parameters.Add("@ExcludeId", excludeId.Value);
            }

            using var connection = new MySqlConnection(_connection);
            var count = await connection.QuerySingleAsync<int>(sql, parameters);
            return count > 0;
        }

        /// <summary>
        /// Lấy tất cả Part (cho select)
        /// </summary>
        public async Task<List<PartSelectDto>> GetAllForSelectAsync()
        {
            var sql = @"
                SELECT 
                    p.part_id AS PartId,
                    p.part_name AS PartName,
                    p.part_code AS PartCode,
                    p.part_quantity AS PartQuantity,
                    p.part_unit AS PartUnit
                FROM part p
                WHERE p.is_deleted = 0
                ORDER BY p.part_name ASC";

            using var connection = new MySqlConnection(_connection);
            var result = await connection.QueryAsync<PartSelectDto>(sql);
            return result.ToList();
        }

        /// <summary>
        /// Lấy tên cột cho filter
        /// </summary>
        private string GetColumnNameForFilter(string columnName)
        {
            return columnName.ToLower() switch
            {
                "partname" or "part_name" => "p.part_name",
                "partcode" or "part_code" => "p.part_code",
                "partquantity" or "part_quantity" => "p.part_quantity",
                "partunit" or "part_unit" => "p.part_unit",
                "partcategoryid" or "part_category_id" => "p.part_category_id",
                "partcategoryname" or "part_category_name" => "pc.part_category_name",
                "partcategorycode" or "part_category_code" => "pc.part_category_code",
                "partprice" or "part_price" => "p.part_price",
                "warrantymonth" or "warranty_month" => "p.warranty_month",
                _ => $"p.{columnName}"
            };
        }

        /// <summary>
        /// Lấy tên cột cho sort
        /// </summary>
        private string GetColumnNameForSort(string columnName)
        {
            return columnName.ToLower() switch
            {
                "partname" or "part_name" => "p.part_name",
                "partcode" or "part_code" => "p.part_code",
                "partquantity" or "part_quantity" => "p.part_quantity",
                "partunit" or "part_unit" => "p.part_unit",
                "partcategoryid" or "part_category_id" => "p.part_category_id",
                "partcategoryname" or "part_category_name" => "pc.part_category_name",
                "partcategorycode" or "part_category_code" => "pc.part_category_code",
                "partprice" or "part_price" => "p.part_price",
                "warrantymonth" or "warranty_month" => "p.warranty_month",
                _ => $"p.{columnName}"
            };
        }
    }
}

