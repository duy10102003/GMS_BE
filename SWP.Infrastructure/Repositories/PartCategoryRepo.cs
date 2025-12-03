using Dapper;
using Microsoft.Extensions.Configuration;
using SWP.Core.Dtos;
using SWP.Core.Dtos.PartCategoryDto;
using SWP.Core.Entities;
using SWP.Core.Interfaces.Repositories;
using MySqlConnector;
using MISA.QLSX.Infrastructure.Repositories;

namespace SWP.Infrastructure.Repositories
{
    /// <summary>
    /// Repository cho Part Category
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public class PartCategoryRepo : BaseRepo<PartCategory>, IPartCategoryRepo
    {
        private readonly string _connection;

        public PartCategoryRepo(IConfiguration configuration) : base(configuration)
        {
            _connection = configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Lấy danh sách Part Category có phân trang
        /// </summary>
        public async Task<PagedResult<PartCategoryListItemDto>> GetPagingAsync(PartCategoryFilterDtoRequest filter)
        {
            var parameters = new DynamicParameters();
            var whereConditions = new List<string>();

            // Base query
            var baseSelect = @"SELECT 
                    pc.part_category_id AS PartCategoryId,
                    pc.part_category_name AS PartCategoryName,
                    pc.part_category_code AS PartCategoryCode,
                    pc.part_category_discription AS PartCategoryDiscription,
                    pc.part_category_phone AS PartCategoryPhone,
                    pc.status AS Status
                FROM part_category pc";

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
                ? " WHERE " + string.Join(" AND ", whereConditions)
                : "";

            // Count query
            var countSql = $@"
                SELECT COUNT(1)
                FROM part_category pc{whereClause}";

            // Sort
            var orderBy = "ORDER BY pc.part_category_id DESC";
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

            var dataSql = string.IsNullOrWhiteSpace(whereClause)
                ? $"{baseSelect} {orderBy} LIMIT @PageSize OFFSET @Offset"
                : $"{baseSelect}{whereClause} {orderBy} LIMIT @PageSize OFFSET @Offset";

            using var connection = new MySqlConnection(_connection);
            
            var total = await connection.QuerySingleAsync<int>(countSql, parameters);
            var items = await connection.QueryAsync<PartCategoryListItemDto>(dataSql, parameters);

            return new PagedResult<PartCategoryListItemDto>
            {
                Items = items,
                Total = total,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }

        /// <summary>
        /// Lấy chi tiết Part Category theo ID
        /// </summary>
        public async Task<PartCategoryDetailDto?> GetDetailAsync(int id)
        {
            var sql = @"
                SELECT 
                    pc.part_category_id AS PartCategoryId,
                    pc.part_category_name AS PartCategoryName,
                    pc.part_category_code AS PartCategoryCode,
                    pc.part_category_discription AS PartCategoryDiscription,
                    pc.part_category_phone AS PartCategoryPhone,
                    pc.status AS Status
                FROM part_category pc
                WHERE pc.part_category_id = @Id";

            using var connection = new MySqlConnection(_connection);
            return await connection.QueryFirstOrDefaultAsync<PartCategoryDetailDto>(sql, new { Id = id });
        }

        /// <summary>
        /// Kiểm tra mã Part Category đã tồn tại hay chưa
        /// </summary>
        public async Task<bool> CheckCodeExistsAsync(string partCategoryCode, int? excludeId = null)
        {
            var sql = "SELECT COUNT(1) FROM part_category WHERE part_category_code = @PartCategoryCode";
            var parameters = new DynamicParameters();
            parameters.Add("@PartCategoryCode", partCategoryCode);

            if (excludeId.HasValue)
            {
                sql += " AND part_category_id != @ExcludeId";
                parameters.Add("@ExcludeId", excludeId.Value);
            }

            using var connection = new MySqlConnection(_connection);
            var count = await connection.QuerySingleAsync<int>(sql, parameters);
            return count > 0;
        }

        /// <summary>
        /// Lấy tất cả Part Category (cho select)
        /// </summary>
        public async Task<List<PartCategorySelectDto>> GetAllForSelectAsync()
        {
            var sql = @"
                SELECT 
                    pc.part_category_id AS PartCategoryId,
                    pc.part_category_name AS PartCategoryName,
                    pc.part_category_code AS PartCategoryCode
                FROM part_category pc
                ORDER BY pc.part_category_name ASC";

            using var connection = new MySqlConnection(_connection);
            var result = await connection.QueryAsync<PartCategorySelectDto>(sql);
            return result.ToList();
        }

        /// <summary>
        /// Lấy tên cột cho filter
        /// </summary>
        private string GetColumnNameForFilter(string columnName)
        {
            return columnName.ToLower() switch
            {
                "partcategoryname" or "part_category_name" => "pc.part_category_name",
                "partcategorycode" or "part_category_code" => "pc.part_category_code",
                "partcategorydiscription" or "part_category_discription" => "pc.part_category_discription",
                "partcategoryphone" or "part_category_phone" => "pc.part_category_phone",
                "status" => "pc.status",
                _ => $"pc.{columnName}"
            };
        }

        /// <summary>
        /// Lấy tên cột cho sort
        /// </summary>
        private string GetColumnNameForSort(string columnName)
        {
            return columnName.ToLower() switch
            {
                "partcategoryname" or "part_category_name" => "pc.part_category_name",
                "partcategorycode" or "part_category_code" => "pc.part_category_code",
                "partcategorydiscription" or "part_category_discription" => "pc.part_category_discription",
                "partcategoryphone" or "part_category_phone" => "pc.part_category_phone",
                "status" => "pc.status",
                "partcategoryid" or "part_category_id" => "pc.part_category_id",
                _ => $"pc.{columnName}"
            };
        }
    }
}

