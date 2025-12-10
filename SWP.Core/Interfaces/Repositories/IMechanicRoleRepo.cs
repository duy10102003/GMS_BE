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
    }
}
