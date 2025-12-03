using SWP.Core.Dtos.CustomerDto;
using SWP.Core.Entities;

namespace SWP.Core.Interfaces.Services
{
    /// <summary>
    /// Interface cho Customer Service
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public interface ICustomerService
    {
        /// <summary>
        /// Tìm kiếm Customer cho select (với search keyword)
        /// </summary>
        /// <param name="request">Request tìm kiếm</param>
        /// <returns>Danh sách Customer</returns>
        Task<List<CustomerSelectDto>> SearchForSelectAsync(CustomerSearchRequest request);
    }
}

