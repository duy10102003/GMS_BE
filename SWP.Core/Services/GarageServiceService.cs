using SWP.Core.Dtos;
using SWP.Core.Dtos.GarageServiceDto;
using SWP.Core.Entities;
using SWP.Core.Exceptions;
using SWP.Core.Interfaces.Repositories;
using SWP.Core.Interfaces.Services;

namespace SWP.Core.Services
{
    /// <summary>
    /// Service cho Garage Service
    /// Created by: DuyLC(02/12/2025)
    /// Updated by: DuyLC(03/12/2025)
    /// </summary>
    public class GarageServiceService : IGarageServiceService
    {
        private readonly IGarageServiceRepo _garageServiceRepo;

        public GarageServiceService(IGarageServiceRepo garageServiceRepo)
        {
            _garageServiceRepo = garageServiceRepo;
        }

        /// <summary>
        /// Lấy danh sách Garage Service có phân trang
        /// </summary>
        public Task<PagedResult<GarageServiceListItemDto>> GetPagingAsync(GarageServiceFilterDtoRequest filter)
        {
            var result = _garageServiceRepo.GetPagingAsync(filter);
            if (result == null)
            {
                throw new NotFoundException("Không tìm thấy danh sách garage service.");
            }
            return result;
        }

        /// <summary>
        /// Lấy Garage Service theo ID
        /// </summary>
        public async Task<GarageServiceDetailDto> GetByIdAsync(int id)
        {
            var result = await _garageServiceRepo.GetDetailAsync(id);
            if (result == null)
            {
                throw new NotFoundException("Không tìm thấy garage service.");
            }
            return result;
        }

        /// <summary>
        /// Tạo mới Garage Service
        /// </summary>
        public async Task<int> CreateAsync(GarageServiceCreateDto request)
        {
            // Validate
            ValidateCreatePayload(request);

            // Tạo entity
            var garageService = new GarageService
            {
                GarageServiceName = request.GarageServiceName.Trim(),
                GarageServicePrice = request.GarageServicePrice
            };

            var idObj = await _garageServiceRepo.InsertAsync(garageService);
            return idObj is int id ? id : (int)idObj;
        }

        /// <summary>
        /// Cập nhật Garage Service
        /// </summary>
        public async Task<int> UpdateAsync(int id, GarageServiceUpdateDto request)
        {
            var existing = await _garageServiceRepo.GetById(id);
            if (existing == null)
            {
                throw new NotFoundException("Không tìm thấy garage service cần cập nhật.");
            }

            // Validate
            ValidateUpdatePayload(request);

            // Cập nhật entity
            existing.GarageServiceName = request.GarageServiceName.Trim();
            existing.GarageServicePrice = request.GarageServicePrice;

            return await _garageServiceRepo.UpdateAsync(id, existing);
        }

        /// <summary>
        /// Xóa Garage Service
        /// </summary>
        public async Task<int> DeleteAsync(int id)
        {
            var existing = await _garageServiceRepo.GetById(id);
            if (existing == null)
            {
                throw new NotFoundException("Không tìm thấy garage service.");
            }

            // Kiểm tra xem garage service có đang được sử dụng trong service ticket không
            var allDetails = await _garageServiceRepo.GetAllAsync();
            // Note: Cần kiểm tra qua ServiceTicketDetail, nhưng vì không có navigation property trực tiếp,
            // nên tạm thời cho phép xóa. Có thể thêm validation sau nếu cần.

            return await _garageServiceRepo.DeleteAsync(id);
        }

        /// <summary>
        /// Tìm kiếm Garage Service cho select (với search keyword)
        /// </summary>
        public Task<List<GarageServiceSelectDto>> SearchForSelectAsync(GarageServiceSearchRequest request)
        {
            return _garageServiceRepo.SearchForSelectAsync(request);
        }

        #region Helpers

        /// <summary>
        /// Validate payload khi tạo mới
        /// </summary>
        private static void ValidateCreatePayload(GarageServiceCreateDto request)
        {
            if (string.IsNullOrWhiteSpace(request.GarageServiceName))
            {
                throw new ValidateException("Tên dịch vụ không được để trống.");
            }

            if (request.GarageServiceName.Length > 255)
            {
                throw new ValidateException("Tên dịch vụ không được vượt quá 255 ký tự.");
            }

            if (request.GarageServicePrice.HasValue && request.GarageServicePrice.Value < 0)
            {
                throw new ValidateException("Giá dịch vụ phải lớn hơn hoặc bằng 0.");
            }
        }

        /// <summary>
        /// Validate payload khi cập nhật
        /// </summary>
        private static void ValidateUpdatePayload(GarageServiceUpdateDto request)
        {
            if (string.IsNullOrWhiteSpace(request.GarageServiceName))
            {
                throw new ValidateException("Tên dịch vụ không được để trống.");
            }

            if (request.GarageServiceName.Length > 255)
            {
                throw new ValidateException("Tên dịch vụ không được vượt quá 255 ký tự.");
            }

            if (request.GarageServicePrice.HasValue && request.GarageServicePrice.Value < 0)
            {
                throw new ValidateException("Giá dịch vụ phải lớn hơn hoặc bằng 0.");
            }
        }

        #endregion
    }
}


