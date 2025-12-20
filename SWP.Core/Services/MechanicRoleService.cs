using SWP.Core.Dtos;
using SWP.Core.Dtos.MechanicRoleDto;
using SWP.Core.Exceptions;
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

        public Task<PagedResult<MechanicRoleMechanicDto>> GetMechanicsByRolePagingAsync(int mechanicRoleId, MechanicRoleMechanicFilterDtoRequest filter)
        {
            return _mechanicRoleRepo.GetMechanicsByRolePagingAsync(mechanicRoleId, filter);
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

        public async Task<int> CreateRoleAsync(MechanicRoleCreateDto request)
        {
            if (string.IsNullOrWhiteSpace(request.MechanicRoleName))
            {
                throw new ValidateException("Tên vai trò không được để trống");
            }

            var exists = await _mechanicRoleRepo.ExistsByNameAsync(
                request.MechanicRoleName.Trim()
            );

            if (exists)
            {
                throw new ValidateException("Tên vai trò đã tồn tại");
            }

            return await _mechanicRoleRepo.CreateRoleAsync(request);
        }


        public async Task<int> UpdateRoleAsync(int mechanicRoleId, MechanicRoleUpdateDto request)
        {
            if (string.IsNullOrWhiteSpace(request.MechanicRoleName))
            {
                throw new ValidateException("Tên vai trò không được để trống");
            }

            var exists = await _mechanicRoleRepo.ExistsByNameAsync(
                request.MechanicRoleName.Trim(),
                mechanicRoleId
            );

            if (exists)
            {
                throw new ValidateException("Tên vai trò đã tồn tại");
            }

            return await _mechanicRoleRepo.UpdateRoleAsync(mechanicRoleId, request);
        }


        public Task<int> SoftDeleteRoleAsync(int mechanicRoleId)
        {
            return _mechanicRoleRepo.SoftDeleteRoleAsync(mechanicRoleId);
        }
    }
}
