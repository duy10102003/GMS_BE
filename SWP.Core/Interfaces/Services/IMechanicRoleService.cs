using SWP.Core.Dtos.MechanicRoleDto;

namespace SWP.Core.Interfaces.Services
{
    public interface IMechanicRoleService
    {
        Task<List<MechanicRoleDto>> GetAllRolesAsync();
        Task<List<MechanicRoleAssignmentDto>> GetAssignmentsByUserAsync(int userId);
        Task<int> AssignRoleAsync(AssignMechanicRoleRequest request);
        Task<int> RemoveAssignmentAsync(int userId, int mechanicRoleId);
    }
}
