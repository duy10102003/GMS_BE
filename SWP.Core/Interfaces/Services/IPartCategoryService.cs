using SWP.Core.Dtos;
using SWP.Core.Dtos.PartCategoryDto;

namespace SWP.Core.Interfaces.Services
{
    /// <summary>
    /// Interface cho Part Category Service
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public interface IPartCategoryService
    {
        /// <summary>
        /// Lấy danh sách Part Category có phân trang
        /// </summary>
        /// <param name="filter">Filter và phân trang</param>
        /// <returns>Danh sách Part Category</returns>
        Task<PagedResult<PartCategoryListItemDto>> GetPagingAsync(PartCategoryFilterDtoRequest filter);

        /// <summary>
        /// Lấy Part Category theo ID
        /// </summary>
        /// <param name="id">ID của Part Category</param>
        /// <returns>Chi tiết Part Category</returns>
        Task<PartCategoryDetailDto> GetByIdAsync(int id);

        /// <summary>
        /// Tạo mới Part Category
        /// </summary>
        /// <param name="request">Thông tin Part Category mới</param>
        /// <returns>ID của Part Category vừa tạo</returns>
        Task<int> CreateAsync(PartCategoryCreateDto request);

        /// <summary>
        /// Cập nhật Part Category
        /// </summary>
        /// <param name="id">ID của Part Category</param>
        /// <param name="request">Thông tin cập nhật</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        Task<int> UpdateAsync(int id, PartCategoryUpdateDto request);

        /// <summary>
        /// Xóa Part Category
        /// </summary>
        /// <param name="id">ID của Part Category</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        Task<int> DeleteAsync(int id);

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
    }
}


