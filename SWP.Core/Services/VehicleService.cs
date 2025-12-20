using SWP.Core.Dtos.VehicleDto;
using SWP.Core.Interfaces.Repositories;
using SWP.Core.Interfaces.Services;

namespace SWP.Core.Services
{
    /// <summary>
    /// Service cho Vehicle
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepo _vehicleRepo;

        public VehicleService(IVehicleRepo vehicleRepo)
        {
            _vehicleRepo = vehicleRepo;
        }

        /// <summary>
        /// Tìm kiếm Vehicle cho select (với search keyword và filter theo customer)
        /// </summary>
        public Task<List<VehicleSelectDto>> SearchForSelectAsync(VehicleSearchRequest request)
        {
            return _vehicleRepo.SearchForSelectAsync(request);
        }
    }
}


