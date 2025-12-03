using SWP.Core.Dtos;
using SWP.Core.Dtos.GarageServiceDto;
using SWP.Core.Entities;

namespace SWP.Core.Interfaces.Services
{
    /// <summary>
    /// Interface cho Garage Service Service
    /// Created by: DuyLC(02/12/2025)
    /// Updated by: DuyLC(03/12/2025) - Thêm search method
    /// </summary>
    public interface IGarageServiceService
    {
        /// <summary>
        /// Lấy danh sách Garage Service có phân trang
        /// </summary>
        /// <param name="filter">Filter và phân trang</param>
        /// <returns>Danh sách Garage Service</returns>
        Task<PagedResult<GarageServiceListItemDto>> GetPagingAsync(GarageServiceFilterDtoRequest filter);

        /// <summary>
        /// Lấy Garage Service theo ID
        /// </summary>
        /// <param name="id">ID của Garage Service</param>
        /// <returns>Chi tiết Garage Service</returns>
        Task<GarageServiceDetailDto> GetByIdAsync(int id);

        /// <summary>
        /// Tạo mới Garage Service
        /// </summary>
        /// <param name="request">Thông tin Garage Service mới</param>
        /// <returns>ID của Garage Service vừa tạo</returns>
        Task<int> CreateAsync(GarageServiceCreateDto request);

        /// <summary>
        /// Cập nhật Garage Service
        /// </summary>
        /// <param name="id">ID của Garage Service</param>
        /// <param name="request">Thông tin cập nhật</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        Task<int> UpdateAsync(int id, GarageServiceUpdateDto request);

        /// <summary>
        /// Xóa Garage Service
        /// </summary>
        /// <param name="id">ID của Garage Service</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        Task<int> DeleteAsync(int id);

        /// <summary>
        /// Tìm kiếm Garage Service cho select (với search keyword)
        /// </summary>
        /// <param name="request">Request tìm kiếm</param>
        /// <returns>Danh sách Garage Service</returns>
        Task<List<GarageServiceSelectDto>> SearchForSelectAsync(GarageServiceSearchRequest request);
    }
}
