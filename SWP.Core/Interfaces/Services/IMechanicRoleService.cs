using SWP.Core.Dtos.MechanicRoleDto;

namespace SWP.Core.Interfaces.Services
{
    public interface IMechanicRoleService
    {
        Task<List<MechanicRoleDto>> GetAllRolesAsync();
        Task<List<MechanicRoleMechanicDto>> GetMechanicsByRoleAsync(int mechanicRoleId);
        Task<List<MechanicRoleAssignmentDto>> GetAssignmentsByUserAsync(int userId);
        Task<int> AssignRoleAsync(AssignMechanicRoleRequest request);
        Task<int> RemoveAssignmentAsync(int userId, int mechanicRoleId);
        Task<MechanicRoleDto?> GetRoleByIdAsync(int mechanicRoleId);
        Task<int> CreateRoleAsync(MechanicRoleCreateDto request);
        Task<int> UpdateRoleAsync(int mechanicRoleId, MechanicRoleUpdateDto request);
        Task<int> SoftDeleteRoleAsync(int mechanicRoleId);
    }
}
