using Dapper;
using Microsoft.Extensions.Configuration;
using SWP.Core.Dtos;
using SWP.Core.Dtos.GarageServiceDto;
using SWP.Core.Entities;
using SWP.Core.Interfaces.Repositories;
using MySqlConnector;
using System.Text;
using MISA.QLSX.Infrastructure.Repositories;

namespace SWP.Infrastructure.Repositories
{
    /// <summary>
    /// Repository cho Garage Service
    /// Created by: DuyLC(02/12/2025)
    /// </summary>
    public class GarageServiceRepo : BaseRepo<GarageService>, IGarageServiceRepo
    {
        private readonly string _connection;

        public GarageServiceRepo(IConfiguration configuration) : base(configuration)
        {
            _connection = configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Lấy danh sách Garage Service có phân trang
        /// </summary>
        public async Task<PagedResult<GarageServiceListItemDto>> GetPagingAsync(GarageServiceFilterDtoRequest filter)
        {
            var parameters = new DynamicParameters();
            var whereConditions = new List<string>();

            // Base query
            var baseSelect = @"
                SELECT 
                    gs.garage_service_id AS GarageServiceId,
                    gs.garage_service_name AS GarageServiceName,
                    gs.garage_service_price AS GarageServicePrice
                FROM garage_service gs
                WHERE gs.is_deleted = 0";

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
                FROM garage_service gs
                WHERE gs.is_deleted = 0{whereClause}";

            // Sort
            var orderBy = "ORDER BY gs.garage_service_id DESC";
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
            var items = await connection.QueryAsync<GarageServiceListItemDto>(dataSql, parameters);

            return new PagedResult<GarageServiceListItemDto>
            {
                Items = items,
                Total = total,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }

        /// <summary>
        /// Lấy chi tiết Garage Service theo ID
        /// </summary>
        public async Task<GarageServiceDetailDto?> GetDetailAsync(int id)
        {
            var sql = @"
                SELECT 
                    gs.garage_service_id AS GarageServiceId,
                    gs.garage_service_name AS GarageServiceName,
                    gs.garage_service_price AS GarageServicePrice
                FROM garage_service gs
                WHERE gs.garage_service_id = @Id AND gs.is_deleted = 0";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);

            using var connection = new MySqlConnection(_connection);
            
            var detail = await connection.QueryFirstOrDefaultAsync<GarageServiceDetailDto>(sql, parameters);
            return detail;
        }

        /// <summary>
        /// Helper để lấy tên cột database cho filter
        /// </summary>
        private string GetColumnNameForFilter(string propertyName)
        {
            return propertyName switch
            {
                "GarageServiceName" => "gs.garage_service_name",
                "GarageServicePrice" => "gs.garage_service_price",
                _ => ""
            };
        }

        /// <summary>
        /// Helper để lấy tên cột database cho sort
        /// </summary>
        private string GetColumnNameForSort(string propertyName)
        {
            return propertyName switch
            {
                "GarageServiceName" => "gs.garage_service_name",
                "GarageServicePrice" => "gs.garage_service_price",
                "GarageServiceId" => "gs.garage_service_id",
                _ => ""
            };
        }

        /// <summary>
        /// Tìm kiếm Garage Service cho select (với search keyword)
        /// </summary>
        public async Task<List<GarageServiceSelectDto>> SearchForSelectAsync(GarageServiceSearchRequest request)
        {
            var sql = @"
                SELECT 
                    gs.garage_service_id AS GarageServiceId,
                    gs.garage_service_name AS GarageServiceName,
                    gs.garage_service_price AS GarageServicePrice
                FROM `garage_service` gs
                WHERE gs.is_deleted = 0";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(request.SearchKeyword))
            {
                sql += " AND gs.garage_service_name LIKE @SearchKeyword";
                parameters.Add("@SearchKeyword", $"%{request.SearchKeyword.Trim()}%");
            }

            sql += " ORDER BY gs.garage_service_name ASC LIMIT @Limit";
            parameters.Add("@Limit", request.Limit);

            using var connection = new MySqlConnection(_connection);
            var result = await connection.QueryAsync<GarageServiceSelectDto>(sql, parameters);
            return result.ToList();
        }
    }
}

