using SWP.Core.Constants.ServiceTicketStatus;
using SWP.Core.Dtos;
using SWP.Core.Dtos.SeriveTicketDto;
using SWP.Core.Dtos.TechnicalTaskDto;
using SWP.Core.Entities;
using SWP.Core.Exceptions;
using SWP.Core.Interfaces.Repositories;
using SWP.Core.Interfaces.Services;

namespace SWP.Core.Services
{
    /// <summary>
    /// Service cho Technical Task
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public class TechnicalTaskService : ITechnicalTaskService
    {
        private readonly ITechnicalTaskRepo _technicalTaskRepo;
        private readonly IServiceTicketRepo _serviceTicketRepo;
        private readonly IBaseRepo<ServiceTicketDetail> _serviceTicketDetailRepo;
        private readonly IPartRepo _partRepo;

        public TechnicalTaskService(
            ITechnicalTaskRepo technicalTaskRepo,
            IServiceTicketRepo serviceTicketRepo,
            IBaseRepo<ServiceTicketDetail> serviceTicketDetailRepo,
            IPartRepo partRepo)
        {
            _technicalTaskRepo = technicalTaskRepo;
            _serviceTicketRepo = serviceTicketRepo;
            _serviceTicketDetailRepo = serviceTicketDetailRepo;
            _partRepo = partRepo;
        }

        /// <summary>
        /// Lấy danh sách Technical Task có phân trang (cho technical staff)
        /// </summary>
        public Task<PagedResult<TechnicalTaskListItemDto>> GetPagingAsync(TechnicalTaskFilterDtoRequest filter)
        {
            var result = _technicalTaskRepo.GetPagingAsync(filter);
            if (result == null)
            {
                throw new NotFoundException("Không tìm thấy danh sách technical task.");
            }
            return result;
        }

        /// <summary>
        /// Lấy Technical Task theo ID
        /// </summary>
        public async Task<TechnicalTaskDetailDto> GetByIdAsync(int id)
        {
            var result = await _technicalTaskRepo.GetDetailAsync(id);
            if (result == null)
            {
                throw new NotFoundException("Không tìm thấy technical task.");
            }
            return result;
        }

        /// <summary>
        /// Technical staff điều chỉnh Parts và Services trong Service Ticket
        /// </summary>
        public async Task<int> AdjustPartsServicesAsync(int technicalTaskId, TechnicalTaskAdjustPartsServicesDto request)
        {
            // Lấy technical task
            var technicalTask = await _technicalTaskRepo.GetById(technicalTaskId);
            if (technicalTask == null)
            {
                throw new NotFoundException("Không tìm thấy technical task.");
            }

            // Lấy service ticket
            var serviceTicket = await _serviceTicketRepo.GetById(technicalTask.ServiceTicketId);
            if (serviceTicket == null)
            {
                throw new NotFoundException("Không tìm thấy service ticket.");
            }

            // Kiểm tra status: chỉ cho phép điều chỉnh khi status là PendingTechnicalConfirmation
            //if (serviceTicket.ServiceTicketStatus != ServiceTicketStatus.PendingTechnicalConfirmation)
            //{
            //    throw new ValidateException("Chỉ có thể điều chỉnh khi service ticket đang chờ technical xác nhận.");
            //}

            // Lấy danh sách service ticket detail hiện tại để rollback part quantity
            var currentDetails = await _serviceTicketRepo.GetServiceTicketDetailsAsync(technicalTask.ServiceTicketId);

            // Rollback part quantity từ các detail cũ
            foreach (var detail in currentDetails.Where(d => d.PartId.HasValue))
            {
                if (detail.PartId.HasValue && detail.Quantity.HasValue)
                {
                    await RollbackPartQuantityAsync(detail.PartId.Value, detail.Quantity.Value);
                }
            }

            // Xóa các detail cũ (soft delete)
            foreach (var detail in currentDetails)
            {
                await _serviceTicketDetailRepo.DeleteAsync(detail.ServiceTicketDetailId);
            }

            // Thêm parts mới
            if (request.Parts != null && request.Parts.Any())
            {
                foreach (var partDto in request.Parts)
                {
                    // Validate part quantity
                    await ValidatePartQuantityAsync(partDto.PartId, partDto.Quantity);

                    // Tạo service ticket detail
                    var detail = new ServiceTicketDetail
                    {
                        ServiceTicketId = technicalTask.ServiceTicketId,
                        PartId = partDto.PartId,
                        Quantity = partDto.Quantity
                    };

                    await _serviceTicketDetailRepo.InsertAsync(detail);
                }
            }

            // Thêm garage services mới
            if (request.GarageServices != null && request.GarageServices.Any())
            {
                foreach (var serviceDto in request.GarageServices)
                {
                    var detail = new ServiceTicketDetail
                    {
                        ServiceTicketId = technicalTask.ServiceTicketId,
                        GarageServiceId = serviceDto.GarageServiceId,
                        Quantity = serviceDto.Quantity
                    };

                    await _serviceTicketDetailRepo.InsertAsync(detail);
                }
            }

            // Cập nhật status service ticket thành AdjustedByTechnical
            serviceTicket.ServiceTicketStatus = ServiceTicketStatus.AdjustedByTechnical;
            await _serviceTicketRepo.UpdateAsync(technicalTask.ServiceTicketId, serviceTicket);

            return 1;
        }

        /// <summary>
        /// Staff xác nhận điều chỉnh từ technical (chuyển status sang InProgress)
        /// </summary>
        public async Task<int> ConfirmAdjustmentAsync(int technicalTaskId, int confirmedBy)
        {
            var technicalTask = await _technicalTaskRepo.GetById(technicalTaskId);
            if (technicalTask == null)
            {
                throw new NotFoundException("Không tìm thấy technical task.");
            }

            var serviceTicket = await _serviceTicketRepo.GetById(technicalTask.ServiceTicketId);
            if (serviceTicket == null)
            {
                throw new NotFoundException("Không tìm thấy service ticket.");
            }

            // Kiểm tra status: chỉ cho phép xác nhận khi status là AdjustedByTechnical
            if (serviceTicket.ServiceTicketStatus != ServiceTicketStatus.AdjustedByTechnical)
            {
                throw new ValidateException("Chỉ có thể xác nhận khi service ticket đã được technical điều chỉnh.");
            }

            // Cập nhật technical task
            technicalTask.ConfirmedBy = confirmedBy;
            technicalTask.ConfirmedAt = DateTime.Now;
            technicalTask.TaskStatus = 1; // In progress
            await _technicalTaskRepo.UpdateAsync(technicalTaskId, technicalTask);

            // Cập nhật service ticket status
            serviceTicket.ServiceTicketStatus = ServiceTicketStatus.InProgress;
            await _serviceTicketRepo.UpdateAsync(technicalTask.ServiceTicketId, serviceTicket);

            return 1;
        }

        /// <summary>
        /// Technical staff hoàn thành công việc (chuyển status sang Completed)
        /// </summary>
        public async Task<int> CompleteTaskAsync(int technicalTaskId)
        {
            var technicalTask = await _technicalTaskRepo.GetById(technicalTaskId);
            if (technicalTask == null)
            {
                throw new NotFoundException("Không tìm thấy technical task.");
            }

            var serviceTicket = await _serviceTicketRepo.GetById(technicalTask.ServiceTicketId);
            if (serviceTicket == null)
            {
                throw new NotFoundException("Không tìm thấy service ticket.");
            }

            // Kiểm tra status: chỉ cho phép hoàn thành khi status là InProgress
            //if (serviceTicket.ServiceTicketStatus != ServiceTicketStatus.InProgress 
            //    && serviceTicket.ServiceTicketStatus != ServiceTicketStatus.CompletedPayment )
            //{
            //    throw new ValidateException("Chỉ có thể hoàn thành khi service ticket đang làm.");
            //}

            // Lấy danh sách parts trong service ticket để deduct quantity
            var details = await _serviceTicketRepo.GetServiceTicketDetailsAsync(technicalTask.ServiceTicketId);
            foreach (var detail in details.Where(d => d.PartId.HasValue && d.Quantity.HasValue))
            {
                await DeductPartQuantityAsync(detail.PartId!.Value, detail.Quantity!.Value);
            }

            // Cập nhật technical task
            technicalTask.TaskStatus = 2; // Completed
            await _technicalTaskRepo.UpdateAsync(technicalTaskId, technicalTask);

            if (serviceTicket.ServiceTicketStatus != ServiceTicketStatus.CompletedPayment)
            {
                // Cập nhật service ticket status
                serviceTicket.ServiceTicketStatus = ServiceTicketStatus.Completed;
            } else if(serviceTicket.ServiceTicketStatus == ServiceTicketStatus.CompletedPayment)
            {
                serviceTicket.ServiceTicketStatus = ServiceTicketStatus.Closed;
            }
            await _serviceTicketRepo.UpdateAsync(technicalTask.ServiceTicketId, serviceTicket);

            return 1;
        }

        #region Helpers

        /// <summary>
        /// Validate part quantity trước khi thêm
        /// </summary>
        private async Task ValidatePartQuantityAsync(int partId, int quantity)
        {
            var part = await _partRepo.GetById(partId);
            if (part == null)
            {
                throw new NotFoundException("Không tìm thấy phụ tùng.");
            }

            if (part.PartQuantity < quantity)
            {
                throw new ValidateException($"Số lượng phụ tùng không đủ. Hiện có: {part.PartQuantity}, yêu cầu: {quantity}");
            }
        }

        /// <summary>
        /// Deduct part quantity (khi confirm task)
        /// </summary>
        private async Task DeductPartQuantityAsync(int partId, int quantity)
        {
            var part = await _partRepo.GetById(partId);
            if (part == null)
            {
                throw new NotFoundException("Không tìm thấy phụ tùng.");
            }

            part.PartQuantity -= quantity;
            if (part.PartQuantity < 0)
            {
                throw new ValidateException("Số lượng phụ tùng không đủ.");
            }

            await _partRepo.UpdateAsync(partId, part);
        }

        /// <summary>
        /// Rollback part quantity (khi điều chỉnh hoặc hủy)
        /// </summary>
        private async Task RollbackPartQuantityAsync(int partId, int quantity)
        {
            var part = await _partRepo.GetById(partId);
            if (part == null)
            {
                return; // Part không tồn tại, bỏ qua
            }

            part.PartQuantity += quantity;
            await _partRepo.UpdateAsync(partId, part);
        }

        #endregion
    }
}


