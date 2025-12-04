using SWP.Core.Dtos;
using SWP.Core.Dtos.PartDto;

namespace SWP.Core.Interfaces.Services
{
    /// <summary>
    /// Interface cho Part Service
    /// Created by: DuyLC(02/12/2025)
    /// </summary>
    public interface IPartService
    {
        /// <summary>
        /// Lấy danh sách Part có phân trang
        /// </summary>
        /// <param name="filter">Filter và phân trang</param>
        /// <returns>Danh sách Part</returns>
        Task<PagedResult<PartListItemDto>> GetPagingAsync(PartFilterDtoRequest filter);

        /// <summary>
        /// Lấy Part theo ID
        /// </summary>
        /// <param name="id">ID của Part</param>
        /// <returns>Chi tiết Part</returns>
        Task<PartDetailDto> GetByIdAsync(int id);

        /// <summary>
        /// Tạo mới Part
        /// </summary>
        /// <param name="request">Thông tin Part mới</param>
        /// <returns>ID của Part vừa tạo</returns>
        Task<int> CreateAsync(PartCreateDto request);

        /// <summary>
        /// Cập nhật Part
        /// </summary>
        /// <param name="id">ID của Part</param>
        /// <param name="request">Thông tin cập nhật</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        Task<int> UpdateAsync(int id, PartUpdateDto request);

        /// <summary>
        /// Xóa Part
        /// </summary>
        /// <param name="id">ID của Part</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        Task<int> DeleteAsync(int id);

        /// <summary>
        /// Kiểm tra mã Part đã tồn tại hay chưa
        /// </summary>
        /// <param name="partCode">Mã Part cần kiểm tra</param>
        /// <param name="excludeId">ID của Part hiện tại (null khi tạo mới, có giá trị khi update để loại trừ chính record đang sửa)</param>
        /// <returns>True nếu đã tồn tại, False nếu chưa</returns>
        Task<bool> CheckCodeExistsAsync(string partCode, int? excludeId = null);

        /// <summary>
        /// Lấy tất cả Part (cho select)
        /// </summary>
        /// <returns>Danh sách Part</returns>
        Task<List<PartSelectDto>> GetAllForSelectAsync();

        /// <summary>
        /// Tìm kiếm Part cho select (với search keyword)
        /// </summary>
        /// <param name="searchKeyword">Từ khóa tìm kiếm (tên, mã)</param>
        /// <param name="limit">Số lượng kết quả tối đa</param>
        /// <returns>Danh sách Part</returns>
        Task<List<PartSelectDto>> SearchForSelectAsync(string? searchKeyword, int limit = 50);
    }
}

