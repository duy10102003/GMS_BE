using SWP.Core.Dtos;
using SWP.Core.Dtos.PartCategoryDto;
using SWP.Core.Entities;

namespace SWP.Core.Interfaces.Repositories
{
    /// <summary>
    /// Interface cho Part Category Repository
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public interface IPartCategoryRepo : IBaseRepo<PartCategory>
    {
        /// <summary>
        /// Lấy danh sách Part Category có phân trang
        /// </summary>
        /// <param name="filter">Filter và phân trang</param>
        /// <returns>Danh sách Part Category</returns>
        Task<PagedResult<PartCategoryListItemDto>> GetPagingAsync(PartCategoryFilterDtoRequest filter);

        /// <summary>
        /// Lấy chi tiết Part Category theo ID
        /// </summary>
        /// <param name="id">ID của Part Category</param>
        /// <returns>Chi tiết Part Category</returns>
        Task<PartCategoryDetailDto?> GetDetailAsync(int id);

        /// <summary>
        /// Kiểm tra mã Part Category đã tồn tại hay chưa
        /// </summary>
        /// <param name="partCategoryCode">Mã Part Category cần kiểm tra</param>
        /// <param name="excludeId">ID của Part Category hiện tại (null khi tạo mới, có giá trị khi update để loại trừ chính record đang sửa)</param>
        /// <returns>True nếu đã tồn tại, False nếu chưa</returns>
        Task<bool> CheckCodeExistsAsync(string partCategoryCode, int? excludeId = null);

        /// <summary>
        /// Lấy tất cả Part Category (cho select)
        /// </summary>
        /// <returns>Danh sách Part Category</returns>
        Task<List<PartCategorySelectDto>> GetAllForSelectAsync();

        /// <summary>
        /// Xóa cứng Part Category (Hard Delete vì không có is_deleted)
        /// </summary>
        /// <param name="id">ID của Part Category cần xóa</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        Task<int> DeleteHardAsync(int id);

        /// <summary>
        /// Kiểm tra Part Category có tồn tại hay không
        /// </summary>
        /// <param name="id">ID của Part Category</param>
        /// <returns>True nếu tồn tại, False nếu không</returns>
        Task<bool> ExistsAsync(int id);
    }
}


