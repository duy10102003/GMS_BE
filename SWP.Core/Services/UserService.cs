using SWP.Core.Dtos.UserDto;
using SWP.Core.Interfaces.Repositories;
using SWP.Core.Interfaces.Services;

namespace SWP.Core.Services
{
    /// <summary>
    /// Service cho User
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepo _userRepo;

        public UserService(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        /// <summary>
        /// Lấy danh sách Technical Staff với trạng thái rảnh/không rảnh
        /// </summary>
        public Task<List<TechnicalStaffSelectDto>> GetTechnicalStaffWithAvailabilityAsync(string roleName)
        {
            return _userRepo.GetTechnicalStaffWithAvailabilityAsync(roleName);
        }
    }
}


