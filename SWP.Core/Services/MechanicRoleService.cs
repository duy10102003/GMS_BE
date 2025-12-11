using SWP.Core.Dtos;
using SWP.Core.Dtos.MechanicRoleDto;
using SWP.Core.Interfaces.Repositories;
using SWP.Core.Interfaces.Services;

namespace SWP.Core.Services
{
    public class MechanicRoleService : IMechanicRoleService
    {
        private readonly IMechanicRoleRepo _mechanicRoleRepo;

        public MechanicRoleService(IMechanicRoleRepo mechanicRoleRepo)
        {
            _mechanicRoleRepo = mechanicRoleRepo;
        }

        public Task<PagedResult<MechanicRoleDto>> GetPagingAsync(MechanicRoleFilterDtoRequest filter)
        {
            return _mechanicRoleRepo.GetPagingAsync(filter);
        }

        public Task<List<MechanicRoleDto>> GetAllRolesAsync()
        {
            return _mechanicRoleRepo.GetAllRolesAsync();
        }

        public Task<List<MechanicRoleMechanicDto>> GetMechanicsByRoleAsync(int mechanicRoleId)
        {
            return _mechanicRoleRepo.GetMechanicsByRoleAsync(mechanicRoleId);
        }

        public Task<List<MechanicRoleAssignmentDto>> GetAssignmentsByUserAsync(int userId)
        {
            return _mechanicRoleRepo.GetAssignmentsByUserAsync(userId);
        }

        public Task<int> AssignRoleAsync(AssignMechanicRoleRequest request)
        {
            return _mechanicRoleRepo.AssignRoleAsync(request);
        }

        public Task<int> RemoveAssignmentAsync(int userId, int mechanicRoleId)
        {
            return _mechanicRoleRepo.RemoveAssignmentAsync(userId, mechanicRoleId);
        }

        public Task<MechanicRoleDto?> GetRoleByIdAsync(int mechanicRoleId)
        {
            return _mechanicRoleRepo.GetRoleByIdAsync(mechanicRoleId);
        }

        public Task<int> CreateRoleAsync(MechanicRoleCreateDto request)
        {
            return _mechanicRoleRepo.CreateRoleAsync(request);
        }

        public Task<int> UpdateRoleAsync(int mechanicRoleId, MechanicRoleUpdateDto request)
        {
            return _mechanicRoleRepo.UpdateRoleAsync(mechanicRoleId, request);
        }

        public Task<int> SoftDeleteRoleAsync(int mechanicRoleId)
        {
            return _mechanicRoleRepo.SoftDeleteRoleAsync(mechanicRoleId);
        }
    }
}
