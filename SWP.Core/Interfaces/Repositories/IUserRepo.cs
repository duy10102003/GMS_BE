using SWP.Core.Dtos.UserDto;
using SWP.Core.Entities;

namespace SWP.Core.Interfaces.Repositories
{
    /// <summary>
    /// Interface cho User Repository
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public interface IUserRepo : IBaseRepo<User>
    {
        /// <summary>
        /// Lấy danh sách Technical Staff với trạng thái rảnh/không rảnh
        /// </summary>
        /// <param name="roleName">Tên role (ví dụ: "TechnicalStaff", "Mechanic")</param>
        /// <returns>Danh sách Technical Staff</returns>
        Task<List<TechnicalStaffSelectDto>> GetTechnicalStaffWithAvailabilityAsync(string roleName);
    }
}

