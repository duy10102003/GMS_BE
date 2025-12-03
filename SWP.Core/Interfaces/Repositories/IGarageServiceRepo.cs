using SWP.Core.Dtos;
using SWP.Core.Dtos.GarageServiceDto;
using SWP.Core.Entities;

namespace SWP.Core.Interfaces.Repositories
{
    /// <summary>
    /// Interface cho Garage Service Repository
    /// Created by: DuyLC(02/12/2025)
    /// </summary>
    public interface IGarageServiceRepo : IBaseRepo<GarageService>
    {
        /// <summary>
        /// Lấy danh sách Garage Service có phân trang
        /// </summary>
        /// <param name="filter">Filter và phân trang</param>
        /// <returns>Danh sách Garage Service</returns>
        Task<PagedResult<GarageServiceListItemDto>> GetPagingAsync(GarageServiceFilterDtoRequest filter);

        /// <summary>
        /// Lấy chi tiết Garage Service theo ID
        /// </summary>
        /// <param name="id">ID của Garage Service</param>
        /// <returns>Chi tiết Garage Service</returns>
        Task<GarageServiceDetailDto?> GetDetailAsync(int id);
    }
}

