using Dapper;
using Microsoft.Extensions.Configuration;
using SWP.Core.Exceptions;
using SWP.Core.Entities;
using SWP.Core.Interfaces.Repositories;
using SWP.Core.GMSAttribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;

namespace MISA.QLSX.Infrastructure.Repositories
{
    public class BaseRepo<T> : IBaseRepo<T> where T : class
    {
        private readonly string _connection;
        private readonly string _tableName; // lấy ra tên bảng
        private readonly PropertyInfo _keyAtribute; // lấy ra khóa chính của bảng
        private readonly string _keyColumn; // lấy ra tên các cột trong bảng

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="configuration"></param>
        public BaseRepo(IConfiguration configuration)
        {
            // lấy ra chuỗi connection
            _connection = configuration.GetConnectionString("DefaultConnection");
            // gọi hàm lấy tên bảng
            _tableName = getTableName();
            // lấy properties của kiểu T
            var properties = typeof(T).GetProperties();
            _keyAtribute = properties.FirstOrDefault(prop => Attribute.IsDefined(prop, typeof(KeyAttribute)));

            if (_keyAtribute == null)
            {
                throw new ValidateException($"Entity {typeof(T).Name} không có thuộc tính nào được đánh dấu [GMSKey]");
            }
            // gọi hàm lấy tên cột 
            _keyColumn = getColumnName(_keyAtribute);

        }

        /// <summary>
        /// hàm lấy ra tên của cột trong bảng
        /// </summary>
        /// <param name="property">thông tin của cột</param>
        /// <returns>tên cột</returns>
        /// created by: DuyLC (29/11/2025)
        public string getColumnName(PropertyInfo property)
        {
            var columnAttribute = (ColumnAttribute?)Attribute.GetCustomAttribute(property, typeof(ColumnAttribute));
            if (columnAttribute != null)
            {
                return $"`{columnAttribute.Name}`";
            }
            // Nếu không có ColumnAttribute, throw exception thay vì trả về property name
            // (điều này không nên xảy ra vì đã filter trước)
            throw new ValidateException($"Property {property.Name} không có [Column] attribute.");
        }

        /// <summary>
        /// Hàm lấy ra tên của bảng
        /// </summary>
        /// <param name="entity">thực thể mà mình cần lấy tên</param>
        /// <returns>tên của bảng</returns>
        /// created by: DuyLC (29/11/2025)
        public string getTableName()
        {
            // Lấy name của kiểu T thông qua reflection.
            var tableName = typeof(T).Name;
            var tableAttribute = (GMSTableNameAttribute?)Attribute.GetCustomAttribute(typeof(T), typeof(GMSTableNameAttribute));
            if (tableAttribute != null)
            {
                tableName = tableAttribute.TableName;
            }
            return tableName;
        }

        /// <summary>
        /// xóa mềm 1 bản ghi trong database
        /// </summary>
        /// <param name="id">id mà mình muốn xóa </param>
        /// <returns>số dòng bị nahr hưởng </returns>
        /// Created By: DuyLC (29/11/2025)
        public async Task<int> DeleteAsync(Guid id)
        {
            //câu lệnh sql xóa mềm theo id
            var sql = $"UPDATE `{_tableName}` SET `is_deleted` = 1 WHERE {_keyColumn} = @id;";
            //dùng dynamic truyền tham số vào câu lệnh sql
            var parameter = new DynamicParameters();
            parameter.Add("@id", id);
            // kết nối đến database
            using (var connection = new MySqlConnection(_connection))
            {
                // thực hiện câu lệnh sql
                var result = await connection.ExecuteAsync(sql, parameter);
                return result;
            }
        }
        /// <summary>
        /// lấy ra tất cả danh sách của thực thể
        /// </summary>
        /// <returns>danh sách thực thể</returns>
        /// created by: DuyLC(29/11/2025)
        public async Task<List<T>> GetAllAsync()
        {
            // câu lệnh sql lấy tất cả danh sách
            var sql = $"select * from `{_tableName}` where `is_deleted` = 0";
            // kết nối đến database bằng dapper và npgsqlconnection
            using (var connection = new MySqlConnection(_connection))
            {
                var result = await connection.QueryAsync<T>(sql);
                return result.ToList();
            }
        }

        /// <summary>
        /// Hàm lấy thông tin chi tiết theo id
        /// </summary>
        /// <param name="id"> id của đối tượng mà mình muốn tìm </param>
        /// <returns>đối tượng có id phù hợp</returns>
        /// Created By: DuyLC (29/11/2025)
        public async Task<T?> GetById(Guid id)
        {
            //câu lệnh sql lấy chi tiết theo id
            var sql = $"select * from `{_tableName}` where {_keyColumn} = @id and `is_deleted` = 0";
            //dùng dynamic truyền tham số vào câu lệnh sql
            var parameter = new DynamicParameters();
            parameter.Add("@id", id);
            Console.WriteLine(sql);
            // kết nối đến database
            using (var connection = new MySqlConnection(_connection))
            {
                // thực hiện câu lệnh sql
                var result = await connection.QueryAsync<T>(sql, parameter);
                return result.FirstOrDefault();
            }
        }

        /// <summary>
        /// Hàm lấy thông tin chi tiết theo id (int)
        /// </summary>
        /// <param name="id"> id của đối tượng mà mình muốn tìm </param>
        /// <returns>đối tượng có id phù hợp</returns>
        /// Created By: DuyLC (02/12/2025)
        public async Task<T?> GetById(int id)
        {
            //câu lệnh sql lấy chi tiết theo id
            var sql = $"select * from `{_tableName}` where {_keyColumn} = @id and `is_deleted` = 0";
            //dùng dynamic truyền tham số vào câu lệnh sql
            var parameter = new DynamicParameters();
            parameter.Add("@id", id);
            // kết nối đến database
            using (var connection = new MySqlConnection(_connection))
            {
                // thực hiện câu lệnh sql
                var result = await connection.QueryAsync<T>(sql, parameter);
                return result.FirstOrDefault();
            }
        }
        /// <summary>
        /// hàm insert 
        /// </summary>
        /// <param name="entity">thông tin đối tượng cần insert</param>
        /// <returns>id của đối tượng vừa thêm</returns>
        /// Created By: DuyLC (29/11/2025)
        public async Task<object> InsertAsync(T entity)
        {
            // Chỉ lấy các properties có [Column] attribute (loại bỏ navigation properties)
            var props = typeof(T).GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(ColumnAttribute)))
                .ToList();
            
            if (!props.Any())
            {
                throw new ValidateException($"Entity {typeof(T).Name} không có properties nào có [Column] attribute.");
            }
            
            var key = _keyAtribute;
            var keyColumnName = getColumnName(key);
            
            // Kiểm tra nếu key là Guid và giá trị là Empty, tạo Guid mới
            if (key.PropertyType == typeof(Guid))
            {
                var currentGuid = (Guid)key.GetValue(entity)!;
                if (currentGuid == Guid.Empty)
                {
                    key.SetValue(entity, Guid.NewGuid());
                }
            }
            
            // Loại bỏ key khỏi INSERT nếu là int/long và giá trị là 0 (để MySQL tự generate)
            var propsToInsert = props.ToList();
            bool skipKey = false;
            if ((key.PropertyType == typeof(int) || key.PropertyType == typeof(long)))
            {
                var keyValue = key.GetValue(entity);
                if (keyValue != null)
                {
                    if (key.PropertyType == typeof(int) && (int)keyValue == 0)
                    {
                        skipKey = true;
                    }
                    else if (key.PropertyType == typeof(long) && (long)keyValue == 0)
                    {
                        skipKey = true;
                    }
                }
            }
            
            if (skipKey)
            {
                propsToInsert = propsToInsert.Where(p => p != key).ToList();
            }
            
            // lấy ra danh sách các cột cách nhau bằng dấu , cho câu lệnh insert
            var columns = string.Join(", ", propsToInsert.Select(p => getColumnName(p)));
            // lấy ra danh sách param cần truyền vào - sử dụng tên cột từ ColumnAttribute
            var paramNames = new List<string>();
            var parameters = new DynamicParameters();
            
            foreach (var prop in propsToInsert)
            {
                var columnAttr = (ColumnAttribute?)Attribute.GetCustomAttribute(prop, typeof(ColumnAttribute));
                if (columnAttr != null)
                {
                    var paramName = $"@p{prop.Name}";
                    paramNames.Add(paramName);
                    var value = prop.GetValue(entity);
                    parameters.Add(paramName, value);
                }
            }
            
            var param = string.Join(", ", paramNames);
            
            //câu lệnh sql
            string sql;
            if (skipKey)
            {
                // Nếu skip key, sử dụng LAST_INSERT_ID() để lấy ID sau khi insert
                sql = $"INSERT INTO `{_tableName}` ({columns}) VALUES ({param}); SELECT LAST_INSERT_ID();";
            }
            else if (key.PropertyType == typeof(Guid))
            {
                // Với Guid, không cần LAST_INSERT_ID
                sql = $"INSERT INTO `{_tableName}` ({columns}) VALUES ({param})";
            }
            else
            {
                // Với int/long có giá trị, vẫn dùng LAST_INSERT_ID để lấy ID (có thể là giá trị đã set hoặc auto-generated)
                sql = $"INSERT INTO `{_tableName}` ({columns}) VALUES ({param}); SELECT LAST_INSERT_ID();";
            }
            
            // thực hiện kết nối 
            using (var connection = new MySqlConnection(_connection))
            {
                if (skipKey || key.PropertyType == typeof(int) || key.PropertyType == typeof(long))
                {
                    // thực thi câu lệnh và lấy ID vừa tạo
                    var insertedId = await connection.QuerySingleAsync<long>(sql, parameters);
                    
                    // Cập nhật ID vào entity
                    if (key.PropertyType == typeof(int))
                    {
                        key.SetValue(entity, (int)insertedId);
                        return (int)insertedId;
                    }
                    else if (key.PropertyType == typeof(long))
                    {
                        key.SetValue(entity, insertedId);
                        return insertedId;
                    }
                }
                else
                {
                    // Với Guid, chỉ cần execute
                    await connection.ExecuteAsync(sql, parameters);
                    return key.GetValue(entity)!;
                }
            }
            
            return key.GetValue(entity)!;
        }
        /// <summary>
        /// Hàm cập nhật thông tin
        /// </summary>
        /// <param name="id">id của đối tượng cần cật nhập</param>
        /// <param name="entity">thông tin cập nhật</param>
        /// <returns>số dòng bị ảnh hưởng</returns>
        /// Created By: DuyLC (29/11/2025)
        public async Task<int> UpdateAsync(Guid id, T entity)
        {
            // Chỉ lấy các properties có [Column] attribute và không phải key (loại bỏ navigation properties)
            var props = typeof(T).GetProperties()
                .Where(p => p != _keyAtribute && Attribute.IsDefined(p, typeof(ColumnAttribute)))
                .ToList();

            if (!props.Any())
            {
                throw new ValidateException($"Entity {typeof(T).Name} không có properties nào có [Column] attribute để update.");
            }

            var setClauses = new List<string>();
            var parameters = new DynamicParameters();
            
            foreach (var prop in props)
            {
                var columnName = getColumnName(prop);
                var paramName = $"@p{prop.Name}";
                setClauses.Add($"{columnName} = {paramName}");
                var value = prop.GetValue(entity);
                parameters.Add(paramName, value);
            }

            var setClause = string.Join(", ", setClauses);
            var sql = $"UPDATE `{_tableName}` SET {setClause} WHERE {_keyColumn} = @id";
            parameters.Add("@id", id);

            using var connection = new MySqlConnection(_connection);
            return await connection.ExecuteAsync(sql, parameters);
        }

        /// <summary>
        /// Hàm cập nhật thông tin (int ID)
        /// </summary>
        /// <param name="id">id của đối tượng cần cật nhập</param>
        /// <param name="entity">thông tin cập nhật</param>
        /// <returns>số dòng bị ảnh hưởng</returns>
        /// Created By: DuyLC (02/12/2025)
        public async Task<int> UpdateAsync(int id, T entity)
        {
            // Chỉ lấy các properties có [Column] attribute và không phải key (loại bỏ navigation properties)
            var props = typeof(T).GetProperties()
                .Where(p => p != _keyAtribute && Attribute.IsDefined(p, typeof(ColumnAttribute)))
                .ToList();

            if (!props.Any())
            {
                throw new ValidateException($"Entity {typeof(T).Name} không có properties nào có [Column] attribute để update.");
            }

            var setClauses = new List<string>();
            var parameters = new DynamicParameters();
            
            foreach (var prop in props)
            {
                var columnName = getColumnName(prop);
                var paramName = $"@p{prop.Name}";
                setClauses.Add($"{columnName} = {paramName}");
                var value = prop.GetValue(entity);
                parameters.Add(paramName, value);
            }

            var setClause = string.Join(", ", setClauses);
            var sql = $"UPDATE `{_tableName}` SET {setClause} WHERE {_keyColumn} = @id";
            parameters.Add("@id", id);

            using var connection = new MySqlConnection(_connection);
            return await connection.ExecuteAsync(sql, parameters);
        }

        /// <summary>
        /// xóa mềm 1 bản ghi trong database (int ID)
        /// </summary>
        /// <param name="id">id mà mình muốn xóa </param>
        /// <returns>số dòng bị ảnh hưởng </returns>
        /// Created By: DuyLC (02/12/2025)
        public async Task<int> DeleteAsync(int id)
        {
            //câu lệnh sql xóa mềm theo id
            var sql = $"UPDATE `{_tableName}` SET `is_deleted` = 1 WHERE {_keyColumn} = @id;";
            //dùng dynamic truyền tham số vào câu lệnh sql
            var parameter = new DynamicParameters();
            parameter.Add("@id", id);
            // kết nối đến database
            using (var connection = new MySqlConnection(_connection))
            {
                // thực hiện câu lệnh sql
                var result = await connection.ExecuteAsync(sql, parameter);
                return result;
            }
        }
    }
}
