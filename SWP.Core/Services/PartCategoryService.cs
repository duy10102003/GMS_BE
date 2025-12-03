using SWP.Core.Dtos;
using SWP.Core.Dtos.PartCategoryDto;
using SWP.Core.Entities;
using SWP.Core.Exceptions;
using SWP.Core.Interfaces.Repositories;
using SWP.Core.Interfaces.Services;

namespace SWP.Core.Services
{
    /// <summary>
    /// Service cho Part Category
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public class PartCategoryService : IPartCategoryService
    {
        private readonly IPartCategoryRepo _partCategoryRepo;

        public PartCategoryService(IPartCategoryRepo partCategoryRepo)
        {
            _partCategoryRepo = partCategoryRepo;
        }

        /// <summary>
        /// Lấy danh sách Part Category có phân trang
        /// </summary>
        public Task<PagedResult<PartCategoryListItemDto>> GetPagingAsync(PartCategoryFilterDtoRequest filter)
        {
            var result = _partCategoryRepo.GetPagingAsync(filter);
            if (result == null)
            {
                throw new NotFoundException("Không tìm thấy danh sách part category.");
            }
            return result;
        }

        /// <summary>
        /// Lấy Part Category theo ID
        /// </summary>
        public async Task<PartCategoryDetailDto> GetByIdAsync(int id)
        {
            var result = await _partCategoryRepo.GetDetailAsync(id);
            if (result == null)
            {
                throw new NotFoundException("Không tìm thấy part category.");
            }
            return result;
        }

        /// <summary>
        /// Tạo mới Part Category
        /// </summary>
        public async Task<int> CreateAsync(PartCategoryCreateDto request)
        {
            // Validate
            ValidateCreatePayload(request);

            // Kiểm tra mã Part Category đã tồn tại chưa
            await EnsureCodeUnique(request.PartCategoryCode, null);

            // Tạo entity
            var partCategory = new PartCategory
            {
                PartCategoryName = request.PartCategoryName?.Trim(),
                PartCategoryCode = request.PartCategoryCode.Trim(),
                PartCategoryDiscription = request.PartCategoryDiscription?.Trim(),
                PartCategoryPhone = request.PartCategoryPhone?.Trim(),
                Status = request.Status?.Trim()
            };

            var idObj = await _partCategoryRepo.InsertAsync(partCategory);
            return idObj is int id ? id : (int)idObj;
        }

        /// <summary>
        /// Cập nhật Part Category
        /// </summary>
        public async Task<int> UpdateAsync(int id, PartCategoryUpdateDto request)
        {
            var existing = await _partCategoryRepo.GetById(id);
            if (existing == null)
            {
                throw new NotFoundException("Không tìm thấy part category cần cập nhật.");
            }

            // Validate
            ValidateUpdatePayload(request);

            // Kiểm tra mã Part Category đã tồn tại chưa (trừ record hiện tại)
            await EnsureCodeUnique(request.PartCategoryCode, id);

            // Cập nhật entity
            existing.PartCategoryName = request.PartCategoryName?.Trim();
            existing.PartCategoryCode = request.PartCategoryCode.Trim();
            existing.PartCategoryDiscription = request.PartCategoryDiscription?.Trim();
            existing.PartCategoryPhone = request.PartCategoryPhone?.Trim();
            existing.Status = request.Status?.Trim();

            return await _partCategoryRepo.UpdateAsync(id, existing);
        }

        /// <summary>
        /// Xóa Part Category
        /// </summary>
        public async Task<int> DeleteAsync(int id)
        {
            var existing = await _partCategoryRepo.GetById(id);
            if (existing == null)
            {
                throw new NotFoundException("Không tìm thấy part category.");
            }

            // Kiểm tra xem part category có đang được sử dụng trong part không
            // Note: Có thể thêm validation sau nếu cần

            return await _partCategoryRepo.DeleteAsync(id);
        }

        /// <summary>
        /// Kiểm tra mã Part Category đã tồn tại hay chưa
        /// </summary>
        public Task<bool> CheckCodeExistsAsync(string partCategoryCode, int? excludeId = null)
        {
            return _partCategoryRepo.CheckCodeExistsAsync(partCategoryCode, excludeId);
        }

        /// <summary>
        /// Lấy tất cả Part Category (cho select)
        /// </summary>
        public Task<List<PartCategorySelectDto>> GetAllForSelectAsync()
        {
            return _partCategoryRepo.GetAllForSelectAsync();
        }

        #region Helpers

        /// <summary>
        /// Validate payload khi tạo mới
        /// </summary>
        private static void ValidateCreatePayload(PartCategoryCreateDto request)
        {
            if (string.IsNullOrWhiteSpace(request.PartCategoryCode))
            {
                throw new ValidateException("Mã danh mục không được để trống.");
            }

            if (request.PartCategoryCode.Length > 20)
            {
                throw new ValidateException("Mã danh mục không được vượt quá 20 ký tự.");
            }

            if (request.PartCategoryName != null && request.PartCategoryName.Length > 100)
            {
                throw new ValidateException("Tên danh mục không được vượt quá 100 ký tự.");
            }

            if (request.PartCategoryDiscription != null && request.PartCategoryDiscription.Length > 255)
            {
                throw new ValidateException("Mô tả không được vượt quá 255 ký tự.");
            }

            if (request.PartCategoryPhone != null && request.PartCategoryPhone.Length > 50)
            {
                throw new ValidateException("Số điện thoại không được vượt quá 50 ký tự.");
            }

            if (request.Status != null && request.Status.Length > 20)
            {
                throw new ValidateException("Trạng thái không được vượt quá 20 ký tự.");
            }
        }

        /// <summary>
        /// Validate payload khi cập nhật
        /// </summary>
        private static void ValidateUpdatePayload(PartCategoryUpdateDto request)
        {
            if (string.IsNullOrWhiteSpace(request.PartCategoryCode))
            {
                throw new ValidateException("Mã danh mục không được để trống.");
            }

            if (request.PartCategoryCode.Length > 20)
            {
                throw new ValidateException("Mã danh mục không được vượt quá 20 ký tự.");
            }

            if (request.PartCategoryName != null && request.PartCategoryName.Length > 100)
            {
                throw new ValidateException("Tên danh mục không được vượt quá 100 ký tự.");
            }

            if (request.PartCategoryDiscription != null && request.PartCategoryDiscription.Length > 255)
            {
                throw new ValidateException("Mô tả không được vượt quá 255 ký tự.");
            }

            if (request.PartCategoryPhone != null && request.PartCategoryPhone.Length > 50)
            {
                throw new ValidateException("Số điện thoại không được vượt quá 50 ký tự.");
            }

            if (request.Status != null && request.Status.Length > 20)
            {
                throw new ValidateException("Trạng thái không được vượt quá 20 ký tự.");
            }
        }

        /// <summary>
        /// Hàm kiểm tra tính duy nhất của mã Part Category trước khi tạo mới hoặc cập nhật
        /// </summary>
        private async Task EnsureCodeUnique(string partCategoryCode, int? excludeId)
        {
            var existed = await _partCategoryRepo.CheckCodeExistsAsync(partCategoryCode.Trim(), excludeId);
            if (existed)
            {
                throw new ConflictException("Mã danh mục đã tồn tại.");
            }
        }

        #endregion
    }
}

