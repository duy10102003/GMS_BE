using SWP.Core.Dtos.UserDto;

namespace SWP.Core.Interfaces.Services
{
    /// <summary>
    /// Interface cho User Service
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Lấy danh sách Technical Staff với trạng thái rảnh/không rảnh
        /// </summary>
        /// <param name="roleName">Tên role (ví dụ: "TechnicalStaff", "Mechanic")</param>
        /// <returns>Danh sách Technical Staff</returns>
        Task<List<TechnicalStaffSelectDto>> GetTechnicalStaffWithAvailabilityAsync(string roleName);
    }
}

