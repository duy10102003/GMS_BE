using SWP.Core.Dtos.CustomerDto;
using SWP.Core.Entities;

namespace SWP.Core.Interfaces.Repositories
{
    /// <summary>
    /// Interface cho Customer Repository
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public interface ICustomerRepo : IBaseRepo<Customer>
    {
        /// <summary>
        /// Tìm kiếm Customer cho select (với search keyword)
        /// </summary>
        /// <param name="request">Request tìm kiếm</param>
        /// <returns>Danh sách Customer</returns>
        Task<List<CustomerSelectDto>> SearchForSelectAsync(CustomerSearchRequest request);

        /// <summary>
        /// Lấy thông tin customer theo userId.
        /// </summary>
        /// <param name="userId">Id của user</param>
        /// <returns>Customer nếu tồn tại</returns>
        Task<Customer?> GetByUserIdAsync(int userId);
    }
}
