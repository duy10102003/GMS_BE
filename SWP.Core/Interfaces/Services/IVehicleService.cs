using SWP.Core.Dtos.VehicleDto;
using SWP.Core.Entities;

namespace SWP.Core.Interfaces.Services
{
    /// <summary>
    /// Interface cho Vehicle Service
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public interface IVehicleService
    {
        /// <summary>
        /// Tìm kiếm Vehicle cho select (với search keyword và filter theo customer)
        /// </summary>
        /// <param name="request">Request tìm kiếm</param>
        /// <returns>Danh sách Vehicle</returns>
        Task<List<VehicleSelectDto>> SearchForSelectAsync(VehicleSearchRequest request);
    }
}

