using SWP.Core.Dtos;
using SWP.Core.Dtos.MechanicRoleDto;
using SWP.Core.Entities;

namespace SWP.Core.Interfaces.Repositories
{
    public interface IMechanicRoleRepo : IBaseRepo<MechanicRole>
    {
        Task<PagedResult<MechanicRoleDto>> GetPagingAsync(MechanicRoleFilterDtoRequest filter);
        Task<List<MechanicRoleDto>> GetAllRolesAsync();
        Task<PagedResult<MechanicRoleMechanicDto>> GetMechanicsByRolePagingAsync(int mechanicRoleId, MechanicRoleMechanicFilterDtoRequest filter);
        Task<List<MechanicRoleMechanicDto>> GetMechanicsByRoleAsync(int mechanicRoleId);
        Task<List<MechanicRoleAssignmentDto>> GetAssignmentsByUserAsync(int userId);
        Task<int> AssignRoleAsync(AssignMechanicRoleRequest request);
        Task<int> RemoveAssignmentAsync(int userId, int mechanicRoleId);
        Task<int> CreateRoleAsync(MechanicRoleCreateDto request);
        Task<int> UpdateRoleAsync(int mechanicRoleId, MechanicRoleUpdateDto request);
        Task<int> SoftDeleteRoleAsync(int mechanicRoleId);
        Task<MechanicRoleDto?> GetRoleByIdAsync(int mechanicRoleId);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);

    }
}
