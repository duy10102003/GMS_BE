using SWP.Core.Dtos;
using SWP.Core.Dtos.PartDto;
using SWP.Core.Entities;

namespace SWP.Core.Interfaces.Repositories
{
    /// <summary>
    /// Interface cho Part Repository
    /// Created by: DuyLC(02/12/2025)
    /// </summary>
    public interface IPartRepo : IBaseRepo<Part>
    {
        /// <summary>
        /// Lấy danh sách Part có phân trang
        /// </summary>
        /// <param name="filter">Filter và phân trang</param>
        /// <returns>Danh sách Part</returns>
        Task<PagedResult<PartListItemDto>> GetPagingAsync(PartFilterDtoRequest filter);

        /// <summary>
        /// Lấy chi tiết Part theo ID
        /// </summary>
        /// <param name="id">ID của Part</param>
        /// <returns>Chi tiết Part</returns>
        Task<PartDetailDto?> GetDetailAsync(int id);

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

