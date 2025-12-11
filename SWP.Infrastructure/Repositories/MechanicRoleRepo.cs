using Dapper;
using Microsoft.Extensions.Configuration;
using SWP.Core.Dtos;
using SWP.Core.Dtos.MechanicRoleDto;
using SWP.Core.Entities;
using SWP.Core.Interfaces.Repositories;
using MySqlConnector;
using MISA.QLSX.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWP.Infrastructure.Repositories
{
    public class MechanicRoleRepo : BaseRepo<MechanicRole>, IMechanicRoleRepo
    {
        private readonly string _connection;

        public MechanicRoleRepo(IConfiguration configuration) : base(configuration)
        {
            _connection = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<PagedResult<MechanicRoleDto>> GetPagingAsync(MechanicRoleFilterDtoRequest filter)
        {
            var parameters = new DynamicParameters();
            var whereConditions = new List<string>();

            const string baseSelect = @"
                SELECT 
                    mr.mechanic_role_id AS MechanicRoleId,
                    mr.mechanic_role_name AS MechanicRoleName,
                    mr.mechanic_role_description AS MechanicRoleDescription
                FROM mechanic_role mr
                WHERE mr.is_deleted = 0";

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
                    if (string.IsNullOrEmpty(columnName))
                    {
                        continue;
                    }

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
                    }

                    filterIndex++;
                }
            }

            var whereClause = whereConditions.Any()
                ? " AND " + string.Join(" AND ", whereConditions)
                : string.Empty;

            var page = filter.Page <= 0 ? 1 : filter.Page;
            var pageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;

            var countSql = $@"
                SELECT COUNT(1)
                FROM mechanic_role mr
                WHERE mr.is_deleted = 0{whereClause}";

            var orderBy = "ORDER BY mr.mechanic_role_id DESC";
            if (filter.ColumnSorts != null && filter.ColumnSorts.Any())
            {
                var sortParts = filter.ColumnSorts
                    .Where(s => !string.IsNullOrWhiteSpace(s.ColumnName) && !string.IsNullOrWhiteSpace(s.SortDirection))
                    .Select(s =>
                    {
                        var columnName = GetColumnNameForSort(s.ColumnName);
                        if (string.IsNullOrEmpty(columnName))
                        {
                            return string.Empty;
                        }
                        var direction = s.SortDirection.ToUpper() == "ASC" ? "ASC" : "DESC";
                        return $"{columnName} {direction}";
                    })
                    .Where(x => !string.IsNullOrEmpty(x));

                if (sortParts.Any())
                {
                    orderBy = "ORDER BY " + string.Join(", ", sortParts);
                }
            }

            var offset = (page - 1) * pageSize;
            parameters.Add("@Offset", offset);
            parameters.Add("@PageSize", pageSize);

            var dataSql = $"{baseSelect}{whereClause} {orderBy} LIMIT @Offset, @PageSize";

            using var connection = new MySqlConnection(_connection);
            var total = await connection.QuerySingleAsync<int>(countSql, parameters);
            var items = await connection.QueryAsync<MechanicRoleDto>(dataSql, parameters);

            return new PagedResult<MechanicRoleDto>
            {
                Items = items,
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PagedResult<MechanicRoleMechanicDto>> GetMechanicsByRolePagingAsync(int mechanicRoleId, MechanicRoleMechanicFilterDtoRequest filter)
        {
            var parameters = new DynamicParameters();
            var whereConditions = new List<string>
            {
                "mrp.is_deleted = 0",
                "mr.is_deleted = 0",
                "u.is_deleted = 0",
                "mrp.mechanic_role_id = @MechanicRoleId"
            };
            parameters.Add("@MechanicRoleId", mechanicRoleId);

            const string baseSelect = @"
                SELECT 
                    mrp.mechanic_role_permission_id AS MechanicRolePermissionId,
                    mrp.mechanic_role_id AS MechanicRoleId,
                    mr.mechanic_role_name AS MechanicRoleName,
                    u.user_id AS UserId,
                    u.full_name AS FullName,
                    u.email AS Email,
                    u.phone AS Phone,
                    mrp.year_exp AS YearExp
                FROM mechanic_role_permission mrp
                INNER JOIN mechanic_role mr ON mrp.mechanic_role_id = mr.mechanic_role_id
                INNER JOIN users u ON mrp.user_id = u.user_id";

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
                    var columnName = GetMechanicColumnNameForFilter(columnFilter.ColumnName);
                    if (string.IsNullOrEmpty(columnName))
                    {
                        continue;
                    }

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
                    }

                    filterIndex++;
                }
            }

            var whereClause = " WHERE " + string.Join(" AND ", whereConditions);

            var page = filter.Page <= 0 ? 1 : filter.Page;
            var pageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;

            var countSql = $@"
                SELECT COUNT(1)
                FROM mechanic_role_permission mrp
                INNER JOIN mechanic_role mr ON mrp.mechanic_role_id = mr.mechanic_role_id
                INNER JOIN users u ON mrp.user_id = u.user_id
                {whereClause}";

            var orderBy = "ORDER BY u.full_name ASC, mrp.mechanic_role_permission_id DESC";
            if (filter.ColumnSorts != null && filter.ColumnSorts.Any())
            {
                var sortParts = filter.ColumnSorts
                    .Where(s => !string.IsNullOrWhiteSpace(s.ColumnName) && !string.IsNullOrWhiteSpace(s.SortDirection))
                    .Select(s =>
                    {
                        var columnName = GetMechanicColumnNameForSort(s.ColumnName);
                        if (string.IsNullOrEmpty(columnName))
                        {
                            return string.Empty;
                        }
                        var direction = s.SortDirection.ToUpper() == "ASC" ? "ASC" : "DESC";
                        return $"{columnName} {direction}";
                    })
                    .Where(x => !string.IsNullOrEmpty(x));

                if (sortParts.Any())
                {
                    orderBy = "ORDER BY " + string.Join(", ", sortParts);
                }
            }

            var offset = (page - 1) * pageSize;
            parameters.Add("@Offset", offset);
            parameters.Add("@PageSize", pageSize);

            var dataSql = $"{baseSelect}{whereClause} {orderBy} LIMIT @Offset, @PageSize";

            using var connection = new MySqlConnection(_connection);
            var total = await connection.QuerySingleAsync<int>(countSql, parameters);
            var items = await connection.QueryAsync<MechanicRoleMechanicDto>(dataSql, parameters);

            return new PagedResult<MechanicRoleMechanicDto>
            {
                Items = items,
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<List<MechanicRoleDto>> GetAllRolesAsync()
        {
            const string sql = @"SELECT 
                    mechanic_role_id AS MechanicRoleId,
                    mechanic_role_name AS MechanicRoleName,
                    mechanic_role_description AS MechanicRoleDescription
                FROM mechanic_role
                WHERE is_deleted = 0";

            using var connection = new MySqlConnection(_connection);
            var result = await connection.QueryAsync<MechanicRoleDto>(sql);
            return result.ToList();
        }

        public async Task<List<MechanicRoleMechanicDto>> GetMechanicsByRoleAsync(int mechanicRoleId)
        {
            const string sql = @"
                SELECT 
                    mrp.mechanic_role_permission_id AS MechanicRolePermissionId,
                    mrp.mechanic_role_id AS MechanicRoleId,
                    mr.mechanic_role_name AS MechanicRoleName,
                    u.user_id AS UserId,
                    u.full_name AS FullName,
                    u.email AS Email,
                    u.phone AS Phone,
                    mrp.year_exp AS YearExp
                FROM mechanic_role_permission mrp
                INNER JOIN mechanic_role mr ON mrp.mechanic_role_id = mr.mechanic_role_id
                INNER JOIN users u ON mrp.user_id = u.user_id
                WHERE mrp.is_deleted = 0 
                    AND mr.is_deleted = 0
                    AND u.is_deleted = 0
                    AND mrp.mechanic_role_id = @MechanicRoleId";

            using var connection = new MySqlConnection(_connection);
            var result = await connection.QueryAsync<MechanicRoleMechanicDto>(sql, new { MechanicRoleId = mechanicRoleId });
            return result.ToList();
        }

        public async Task<List<MechanicRoleAssignmentDto>> GetAssignmentsByUserAsync(int userId)
        {
            const string sql = @"
                SELECT 
                    mrp.mechanic_role_permission_id AS MechanicRolePermissionId,
                    mrp.mechanic_role_id AS MechanicRoleId,
                    mr.mechanic_role_name AS MechanicRoleName,
                    mrp.user_id AS UserId,
                    mrp.year_exp AS YearExp
                FROM mechanic_role_permission mrp
                INNER JOIN mechanic_role mr ON mrp.mechanic_role_id = mr.mechanic_role_id
                WHERE mrp.is_deleted = 0 AND mr.is_deleted = 0 AND mrp.user_id = @UserId
            ";
            using var connection = new MySqlConnection(_connection);
            var result = await connection.QueryAsync<MechanicRoleAssignmentDto>(sql, new { UserId = userId });
            return result.ToList();
        }

        public async Task<int> AssignRoleAsync(AssignMechanicRoleRequest request)
        {
            const string getSql = @"SELECT mechanic_role_permission_id, is_deleted 
                                    FROM mechanic_role_permission 
                                    WHERE user_id = @UserId AND mechanic_role_id = @MechanicRoleId
                                    LIMIT 1";

            const string insertSql = @"INSERT INTO mechanic_role_permission (mechanic_role_id, user_id, year_exp, is_deleted)
                                       VALUES (@MechanicRoleId, @UserId, @YearExp, 0);";

            const string updateSql = @"UPDATE mechanic_role_permission 
                                       SET year_exp = @YearExp, is_deleted = 0
                                       WHERE mechanic_role_permission_id = @MechanicRolePermissionId;";

            using var connection = new MySqlConnection(_connection);
            var existing = await connection.QueryFirstOrDefaultAsync<(int mechanic_role_permission_id, int is_deleted)>(
                getSql, new { request.UserId, request.MechanicRoleId });

            if (existing.mechanic_role_permission_id != 0)
            {
                return await connection.ExecuteAsync(updateSql, new
                {
                    request.YearExp,
                    MechanicRolePermissionId = existing.mechanic_role_permission_id
                });
            }

            return await connection.ExecuteAsync(insertSql, request);
        }

        public async Task<int> RemoveAssignmentAsync(int userId, int mechanicRoleId)
        {
            const string sql = @"UPDATE mechanic_role_permission
                                 SET is_deleted = 1
                                 WHERE user_id = @UserId AND mechanic_role_id = @MechanicRoleId";
            using var connection = new MySqlConnection(_connection);
            return await connection.ExecuteAsync(sql, new { UserId = userId, MechanicRoleId = mechanicRoleId });
        }

        public async Task<MechanicRoleDto?> GetRoleByIdAsync(int mechanicRoleId)
        {
            const string sql = @"SELECT 
                    mechanic_role_id AS MechanicRoleId,
                    mechanic_role_name AS MechanicRoleName,
                    mechanic_role_description AS MechanicRoleDescription
                FROM mechanic_role
                WHERE is_deleted = 0 AND mechanic_role_id = @MechanicRoleId
                LIMIT 1";
            using var connection = new MySqlConnection(_connection);
            return await connection.QueryFirstOrDefaultAsync<MechanicRoleDto>(sql, new { MechanicRoleId = mechanicRoleId });
        }

        public async Task<int> CreateRoleAsync(MechanicRoleCreateDto request)
        {
            const string sql = @"INSERT INTO mechanic_role (mechanic_role_name, mechanic_role_description, is_deleted)
                                 VALUES (@MechanicRoleName, @MechanicRoleDescription, 0);
                                 SELECT LAST_INSERT_ID();";
            using var connection = new MySqlConnection(_connection);
            var id = await connection.ExecuteScalarAsync<long>(sql, request);
            return (int)id;
        }

        public async Task<int> UpdateRoleAsync(int mechanicRoleId, MechanicRoleUpdateDto request)
        {
            const string sql = @"UPDATE mechanic_role
                                 SET mechanic_role_name = @MechanicRoleName,
                                     mechanic_role_description = @MechanicRoleDescription
                                 WHERE mechanic_role_id = @MechanicRoleId AND is_deleted = 0";
            using var connection = new MySqlConnection(_connection);
            return await connection.ExecuteAsync(sql, new
            {
                MechanicRoleId = mechanicRoleId,
                request.MechanicRoleName,
                request.MechanicRoleDescription
            });
        }

        public async Task<int> SoftDeleteRoleAsync(int mechanicRoleId)
        {
            const string sql = @"UPDATE mechanic_role
                                 SET is_deleted = 1
                                 WHERE mechanic_role_id = @MechanicRoleId";
            using var connection = new MySqlConnection(_connection);
            return await connection.ExecuteAsync(sql, new { MechanicRoleId = mechanicRoleId });
        }

        private string GetColumnNameForFilter(string propertyName)
        {
            return propertyName switch
            {
                "MechanicRoleName" => "mr.mechanic_role_name",
                "MechanicRoleDescription" => "mr.mechanic_role_description",
                _ => string.Empty
            };
        }

        private string GetColumnNameForSort(string propertyName)
        {
            return propertyName switch
            {
                "MechanicRoleId" => "mr.mechanic_role_id",
                "MechanicRoleName" => "mr.mechanic_role_name",
                _ => string.Empty
            };
        }

        private string GetMechanicColumnNameForFilter(string propertyName)
        {
            return propertyName switch
            {
                "FullName" => "u.full_name",
                "Email" => "u.email",
                "Phone" => "u.phone",
                "YearExp" => "mrp.year_exp",
                _ => string.Empty
            };
        }

        private string GetMechanicColumnNameForSort(string propertyName)
        {
            return propertyName switch
            {
                "FullName" => "u.full_name",
                "Email" => "u.email",
                "Phone" => "u.phone",
                "YearExp" => "mrp.year_exp",
                "MechanicRolePermissionId" => "mrp.mechanic_role_permission_id",
                _ => string.Empty
            };
        }
    }
}
