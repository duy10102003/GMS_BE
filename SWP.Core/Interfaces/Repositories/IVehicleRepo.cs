using SWP.Core.Dtos.VehicleDto;
using SWP.Core.Entities;

namespace SWP.Core.Interfaces.Repositories
{
    /// <summary>
    /// Interface cho Vehicle Repository
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public interface IVehicleRepo : IBaseRepo<Vehicle>
    {
        /// <summary>
        /// Tìm kiếm Vehicle cho select (với search keyword và filter theo customer)
        /// </summary>
        /// <param name="request">Request tìm kiếm</param>
        /// <returns>Danh sách Vehicle</returns>
        Task<List<VehicleSelectDto>> SearchForSelectAsync(VehicleSearchRequest request);
    }
}

