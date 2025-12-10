using SWP.Core.Dtos.MechanicRoleDto;
using SWP.Core.Entities;

namespace SWP.Core.Interfaces.Repositories
{
    public interface IMechanicRoleRepo : IBaseRepo<MechanicRole>
    {
        Task<List<MechanicRoleDto>> GetAllRolesAsync();
        Task<List<MechanicRoleAssignmentDto>> GetAssignmentsByUserAsync(int userId);
        Task<int> AssignRoleAsync(AssignMechanicRoleRequest request);
        Task<int> RemoveAssignmentAsync(int userId, int mechanicRoleId);
        Task<int> CreateRoleAsync(MechanicRoleCreateDto request);
        Task<int> UpdateRoleAsync(int mechanicRoleId, MechanicRoleUpdateDto request);
        Task<int> SoftDeleteRoleAsync(int mechanicRoleId);
        Task<MechanicRoleDto?> GetRoleByIdAsync(int mechanicRoleId);
    }
}
