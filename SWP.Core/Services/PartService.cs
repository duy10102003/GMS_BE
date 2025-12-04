using SWP.Core.Dtos;
using SWP.Core.Dtos.PartDto;
using SWP.Core.Entities;
using SWP.Core.Exceptions;
using SWP.Core.Interfaces.Repositories;
using SWP.Core.Interfaces.Services;

namespace SWP.Core.Services
{
    /// <summary>
    /// Service cho Part
    /// Created by: DuyLC(02/12/2025)
    /// </summary>
    public class PartService : IPartService
    {
        private readonly IPartRepo _partRepo;
        private readonly IPartCategoryRepo _partCategoryRepo;

        public PartService(IPartRepo partRepo, IPartCategoryRepo partCategoryRepo)
        {
            _partRepo = partRepo;
            _partCategoryRepo = partCategoryRepo;
        }

        /// <summary>
        /// Lấy danh sách Part có phân trang
        /// </summary>
        public Task<PagedResult<PartListItemDto>> GetPagingAsync(PartFilterDtoRequest filter)
        {
            var result = _partRepo.GetPagingAsync(filter);
            if (result == null)
            {
                throw new NotFoundException("Không tìm thấy danh sách part.");
            }
            return result;
        }

        /// <summary>
        /// Lấy Part theo ID
        /// </summary>
        public async Task<PartDetailDto> GetByIdAsync(int id)
        {
            var result = await _partRepo.GetDetailAsync(id);
            if (result == null)
            {
                throw new NotFoundException("Không tìm thấy part.");
            }
            return result;
        }

        /// <summary>
        /// Tạo mới Part
        /// </summary>
        public async Task<int> CreateAsync(PartCreateDto request)
        {
            // Validate
            ValidateCreatePayload(request);

            // Kiểm tra mã Part đã tồn tại chưa
            await EnsureCodeUnique(request.PartCode, null);

            // Kiểm tra PartCategory tồn tại
            await EnsurePartCategoryExists(request.PartCategoryId);

            // Tạo entity
            var part = new Part
            {
                PartName = request.PartName.Trim(),
                PartCode = request.PartCode.Trim(),
                PartQuantity = request.PartQuantity,
                PartUnit = request.PartUnit.Trim(),
                PartCategoryId = request.PartCategoryId,
                PartPrice = request.PartPrice,
                WarrantyMonth = request.WarrantyMonth
            };

            var idObj = await _partRepo.InsertAsync(part);
            return idObj is int id ? id : (int)idObj;
        }

        /// <summary>
        /// Cập nhật Part
        /// </summary>
        public async Task<int> UpdateAsync(int id, PartUpdateDto request)
        {
            var existing = await _partRepo.GetById(id);
            if (existing == null)
            {
                throw new NotFoundException("Không tìm thấy part cần cập nhật.");
            }

            // Validate
            ValidateUpdatePayload(request);

            // Kiểm tra mã Part đã tồn tại chưa (trừ record hiện tại)
            await EnsureCodeUnique(request.PartCode, id);

            // Kiểm tra PartCategory tồn tại
            await EnsurePartCategoryExists(request.PartCategoryId);

            // Cập nhật entity
            existing.PartName = request.PartName.Trim();
            existing.PartCode = request.PartCode.Trim();
            existing.PartQuantity = request.PartQuantity;
            existing.PartUnit = request.PartUnit.Trim();
            existing.PartCategoryId = request.PartCategoryId;
            existing.PartPrice = request.PartPrice;
            existing.WarrantyMonth = request.WarrantyMonth;

            return await _partRepo.UpdateAsync(id, existing);
        }

        /// <summary>
        /// Xóa Part
        /// </summary>
        public async Task<int> DeleteAsync(int id)
        {
            var existing = await _partRepo.GetById(id);
            if (existing == null)
            {
                throw new NotFoundException("Không tìm thấy part.");
            }

            // Kiểm tra xem part có đang được sử dụng trong service ticket detail không
            // Note: Có thể thêm validation sau nếu cần

            return await _partRepo.DeleteAsync(id);
        }

        /// <summary>
        /// Kiểm tra mã Part đã tồn tại hay chưa
        /// </summary>
        public Task<bool> CheckCodeExistsAsync(string partCode, int? excludeId = null)
        {
            return _partRepo.CheckCodeExistsAsync(partCode, excludeId);
        }

        /// <summary>
        /// Lấy tất cả Part (cho select)
        /// </summary>
        public Task<List<PartSelectDto>> GetAllForSelectAsync()
        {
            return _partRepo.GetAllForSelectAsync();
        }

        /// <summary>
        /// Tìm kiếm Part cho select (với search keyword)
        /// </summary>
        public Task<List<PartSelectDto>> SearchForSelectAsync(string? searchKeyword, int limit = 50)
        {
            return _partRepo.SearchForSelectAsync(searchKeyword, limit);
        }

        /// <summary>
        /// Tìm kiếm Part cho select (với DTO request)
        /// </summary>
        public Task<List<PartSelectDto>> SearchForSelectAsync(PartSearchRequest request)
        {
            return _partRepo.SearchForSelectAsync(request.SearchKeyword, request.Limit);
        }

        #region Helpers

        /// <summary>
        /// Validate payload khi tạo mới
        /// </summary>
        private static void ValidateCreatePayload(PartCreateDto request)
        {
            if (string.IsNullOrWhiteSpace(request.PartName))
            {
                throw new ValidateException("Tên phụ tùng không được để trống.");
            }

            if (request.PartName.Length > 100)
            {
                throw new ValidateException("Tên phụ tùng không được vượt quá 100 ký tự.");
            }

            if (string.IsNullOrWhiteSpace(request.PartCode))
            {
                throw new ValidateException("Mã phụ tùng không được để trống.");
            }

            if (request.PartCode.Length > 20)
            {
                throw new ValidateException("Mã phụ tùng không được vượt quá 20 ký tự.");
            }

            if (request.PartQuantity < 0)
            {
                throw new ValidateException("Số lượng tồn kho phải lớn hơn hoặc bằng 0.");
            }

            if (string.IsNullOrWhiteSpace(request.PartUnit))
            {
                throw new ValidateException("Đơn vị tính không được để trống.");
            }

            if (request.PartUnit.Length > 20)
            {
                throw new ValidateException("Đơn vị tính không được vượt quá 20 ký tự.");
            }
        }

        /// <summary>
        /// Validate payload khi cập nhật
        /// </summary>
        private static void ValidateUpdatePayload(PartUpdateDto request)
        {
            if (string.IsNullOrWhiteSpace(request.PartName))
            {
                throw new ValidateException("Tên phụ tùng không được để trống.");
            }

            if (request.PartName.Length > 100)
            {
                throw new ValidateException("Tên phụ tùng không được vượt quá 100 ký tự.");
            }

            if (string.IsNullOrWhiteSpace(request.PartCode))
            {
                throw new ValidateException("Mã phụ tùng không được để trống.");
            }

            if (request.PartCode.Length > 20)
            {
                throw new ValidateException("Mã phụ tùng không được vượt quá 20 ký tự.");
            }

            if (request.PartQuantity < 0)
            {
                throw new ValidateException("Số lượng tồn kho phải lớn hơn hoặc bằng 0.");
            }

            if (string.IsNullOrWhiteSpace(request.PartUnit))
            {
                throw new ValidateException("Đơn vị tính không được để trống.");
            }

            if (request.PartUnit.Length > 20)
            {
                throw new ValidateException("Đơn vị tính không được vượt quá 20 ký tự.");
            }
        }

        /// <summary>
        /// Hàm kiểm tra tính duy nhất của mã Part trước khi tạo mới hoặc cập nhật
        /// </summary>
        private async Task EnsureCodeUnique(string partCode, int? excludeId)
        {
            var existed = await _partRepo.CheckCodeExistsAsync(partCode.Trim(), excludeId);
            if (existed)
            {
                throw new ConflictException("Mã phụ tùng đã tồn tại.");
            }
        }

        /// <summary>
        /// Kiểm tra sự tồn tại của PartCategory theo ID trước khi thực hiện nghiệp vụ
        /// </summary>
        private async Task EnsurePartCategoryExists(int partCategoryId)
        {
            // Dùng ExistsAsync để kiểm tra tồn tại (đơn giản và hiệu quả hơn)
            var exists = await _partCategoryRepo.ExistsAsync(partCategoryId);
            if (!exists)
            {
                throw new ValidateException("Không tìm thấy danh mục phụ tùng.");
            }
        }

        #endregion
    }
}

