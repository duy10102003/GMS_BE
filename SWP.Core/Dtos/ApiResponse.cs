namespace SWP.Core.Dtos
{
    /// <summary>
    /// Response wrapper chuẩn cho tất cả API
    /// Created by: DuyLC(01/12/2025)
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu trả về</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Trạng thái thành công hay không
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Dữ liệu trả về
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Thông báo
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Tạo response thành công
        /// </summary>
        public static ApiResponse<T> SuccessResponse(T data, string message = "Thành công")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message
            };
        }

        /// <summary>
        /// Tạo response thất bại
        /// </summary>
        public static ApiResponse<T> ErrorResponse(string message, T? data = default)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Data = data,
                Message = message
            };
        }
    }

    /// <summary>
    /// Response wrapper không có data
    /// </summary>
    public class ApiResponse
    {
        /// <summary>
        /// Trạng thái thành công hay không
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Thông báo
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Tạo response thành công
        /// </summary>
        public static ApiResponse SuccessResponse(string message = "Thành công")
        {
            return new ApiResponse
            {
                Success = true,
                Message = message
            };
        }

        /// <summary>
        /// Tạo response thất bại
        /// </summary>
        public static ApiResponse ErrorResponse(string message)
        {
            return new ApiResponse
            {
                Success = false,
                Message = message
            };
        }
    }
}

