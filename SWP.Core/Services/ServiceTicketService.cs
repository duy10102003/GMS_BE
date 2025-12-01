using SWP.Core.Constants.ServiceTicketStatus;
using SWP.Core.Dtos;
using SWP.Core.Dtos.SeriveTicketDto;
using SWP.Core.Entities;
using SWP.Core.Exceptions;
using SWP.Core.Interfaces.Repositories;
using SWP.Core.Interfaces.Services;

namespace SWP.Core.Services
{
    /// <summary>
    /// Service cho Service Ticket
    /// Created by: DuyLC(01/12/2025)
    /// </summary>
    public class ServiceTicketService : IServiceTicketService
    {
        private readonly IServiceTicketRepo _serviceTicketRepo;
        private readonly IBaseRepo<User> _userRepo;
        private readonly IBaseRepo<Vehicle> _vehicleRepo;
        private readonly IBaseRepo<Part> _partRepo;
        private readonly IBaseRepo<Booking> _bookingRepo;
        private readonly IBaseRepo<Customer> _customerRepo;
        private readonly IBaseRepo<ServiceTicketDetail> _serviceTicketDetailRepo;
        private readonly IBaseRepo<TechnicalTask> _technicalTaskRepo;

        public ServiceTicketService(
            IServiceTicketRepo serviceTicketRepo,
            IBaseRepo<User> userRepo,
            IBaseRepo<Vehicle> vehicleRepo,
            IBaseRepo<Part> partRepo,
            IBaseRepo<Booking> bookingRepo,
            IBaseRepo<Customer> customerRepo,
            IBaseRepo<ServiceTicketDetail> serviceTicketDetailRepo,
            IBaseRepo<TechnicalTask> technicalTaskRepo)
        {
            _serviceTicketRepo = serviceTicketRepo;
            _userRepo = userRepo;
            _vehicleRepo = vehicleRepo;
            _partRepo = partRepo;
            _bookingRepo = bookingRepo;
            _customerRepo = customerRepo;
            _serviceTicketDetailRepo = serviceTicketDetailRepo;
            _technicalTaskRepo = technicalTaskRepo;
        }

        /// <summary>
        /// Lấy danh sách Service Ticket có phân trang
        /// </summary>
        public Task<PagedResult<ServiceTicketListItemDto>> GetPagingAsync(ServiceTicketFilterDtoRequest filter)
        {
            var result = _serviceTicketRepo.GetPagingAsync(filter);
            if (result == null)
            {
                throw new NotFoundException("Không tìm thấy danh sách service ticket.");
            }
            return result;
        }

        /// <summary>
        /// Lấy Service Ticket theo ID
        /// </summary>
        public async Task<ServiceTicketDetailDto> GetByIdAsync(Guid id)
        {
            var result = await _serviceTicketRepo.GetDetailAsync(id);
            if (result == null)
            {
                throw new NotFoundException("Không tìm thấy service ticket.");
            }
            return result;
        }

        /// <summary>
        /// Tạo mới Service Ticket
        /// </summary>
        public async Task<Guid> CreateAsync(ServiceTicketCreateDto request)
        {
            // Validate
            ValidateCreatePayload(request);

            // Kiểm tra user tồn tại
            await EnsureUserExists(request.CreatedBy);

            // Kiểm tra booking nếu có
            if (request.BookingId.HasValue)
            {
                await EnsureBookingExists(request.BookingId.Value);
            }

            // Xử lý customer: Nếu có CustomerId thì dùng, nếu không thì tạo mới từ CustomerInfo
            Guid customerId;
            if (request.CustomerId.HasValue)
            {
                // Kiểm tra customer tồn tại
                var existingCustomer = await _customerRepo.GetById(request.CustomerId.Value);
                if (existingCustomer == null)
                {
                    throw new NotFoundException("Không tìm thấy customer.");
                }
                customerId = request.CustomerId.Value;
            }
            else if (request.CustomerInfo != null)
            {
                // Kiểm tra user tồn tại nếu có UserId
                if (request.CustomerInfo.UserId.HasValue)
                {
                    await EnsureUserExists(request.CustomerInfo.UserId.Value);
                }

                // Tạo customer mới
                var customer = new Customer
                {
                    CustomerId = Guid.NewGuid(),
                    CustomerName = request.CustomerInfo.CustomerName?.Trim(),
                    CustomerPhone = request.CustomerInfo.CustomerPhone.Trim(),
                    CustomerEmail = request.CustomerInfo.CustomerEmail?.Trim(),
                    UserId = request.CustomerInfo.UserId
                };

                customerId = await _customerRepo.InsertAsync(customer);
            }
            else
            {
                throw new ValidateException("Phải cung cấp CustomerId hoặc CustomerInfo.");
            }

            // Xử lý vehicle: Nếu có VehicleId thì dùng, nếu không thì tạo mới từ VehicleInfo
            Guid vehicleId;
            if (request.VehicleId.HasValue)
            {
                // Kiểm tra vehicle tồn tại
                await EnsureVehicleExists(request.VehicleId.Value);
                vehicleId = request.VehicleId.Value;

                // Cập nhật customer_id cho vehicle nếu chưa có
                var vehicle = await _vehicleRepo.GetById(vehicleId);
                if (vehicle != null && !vehicle.CustomerId.HasValue)
                {
                    vehicle.CustomerId = customerId;
                    await _vehicleRepo.UpdateAsync(vehicleId, vehicle);
                }
            }
            else if (request.VehicleInfo != null)
            {
                // Tạo vehicle mới và link với customer
                var vehicle = new Vehicle
                {
                    VehicleId = Guid.NewGuid(),
                    VehicleName = request.VehicleInfo.VehicleName.Trim(),
                    VehicleLicensePlate = request.VehicleInfo.VehicleLicensePlate.Trim(),
                    CurrentKm = request.VehicleInfo.CurrentKm,
                    Make = request.VehicleInfo.Make?.Trim(),
                    Model = request.VehicleInfo.Model?.Trim(),
                    CustomerId = customerId // Link với customer vừa tạo hoặc chọn
                };

                vehicleId = await _vehicleRepo.InsertAsync(vehicle);
            }
            else
            {
                throw new ValidateException("Phải cung cấp VehicleId hoặc VehicleInfo.");
            }

            // Kiểm tra mã code nếu có
            if (!string.IsNullOrWhiteSpace(request.ServiceTicketCode))
            {
                await EnsureCodeUnique(request.ServiceTicketCode.Trim(), null);
            }

            // Generate code nếu chưa có
            var serviceTicketCode = request.ServiceTicketCode?.Trim();
            if (string.IsNullOrWhiteSpace(serviceTicketCode))
            {
                serviceTicketCode = await GenerateServiceTicketCodeAsync();
            }

            // Tạo entity
            var serviceTicket = new ServiceTicket
            {
                ServiceTicketId = Guid.NewGuid(),
                BookingId = request.BookingId,
                VehicleId = vehicleId,
                CreatedBy = request.CreatedBy,
                CreatedDate = DateTime.UtcNow,
                ServiceTicketStatus = ServiceTicketStatus.Pending,
                InitialIssue = request.InitialIssue,
                ServiceTicketCode = serviceTicketCode
            };

            var serviceTicketId = await _serviceTicketRepo.InsertAsync(serviceTicket);

            // Nếu có assign technical staff, tạo technical task
            if (request.AssignedToTechnical.HasValue)
            {
                var description = request.AssignDescription ?? 
                    request.InitialIssue ?? 
                    "Công việc được assign từ khi tạo service ticket";
                await AssignToTechnicalInternalAsync(serviceTicketId, request.AssignedToTechnical.Value, description);
            }

            return serviceTicketId;
        }

        /// <summary>
        /// Cập nhật Service Ticket
        /// </summary>
        public async Task<int> UpdateAsync(Guid id, ServiceTicketUpdateDto request)
        {
            var existing = await _serviceTicketRepo.GetById(id);
            if (existing == null)
            {
                throw new NotFoundException("Không tìm thấy service ticket cần cập nhật.");
            }

            // Validate
            ValidateUpdatePayload(request);

            // Kiểm tra user tồn tại
            await EnsureUserExists(request.ModifiedBy);

            // Kiểm tra booking nếu có
            if (request.BookingId.HasValue)
            {
                await EnsureBookingExists(request.BookingId.Value);
            }

            // Cập nhật thông tin vehicle nếu có (không đổi vehicle_id)
            if (request.VehicleInfo != null)
            {
                var vehicle = await _vehicleRepo.GetById(existing.VehicleId);
                if (vehicle == null)
                {
                    throw new NotFoundException("Không tìm thấy vehicle của service ticket này.");
                }

                // Cập nhật thông tin vehicle
                vehicle.VehicleName = request.VehicleInfo.VehicleName.Trim();
                vehicle.VehicleLicensePlate = request.VehicleInfo.VehicleLicensePlate.Trim();
                vehicle.CurrentKm = request.VehicleInfo.CurrentKm;
                vehicle.Make = request.VehicleInfo.Make?.Trim();
                vehicle.Model = request.VehicleInfo.Model?.Trim();

                // Kiểm tra customer tồn tại nếu có
                if (request.VehicleInfo.CustomerId.HasValue)
                {
                    var customer = await _customerRepo.GetById(request.VehicleInfo.CustomerId.Value);
                    if (customer == null)
                    {
                        throw new NotFoundException("Không tìm thấy customer.");
                    }
                    vehicle.CustomerId = request.VehicleInfo.CustomerId;
                }

                await _vehicleRepo.UpdateAsync(vehicle.VehicleId, vehicle);
            }

            // Kiểm tra mã code nếu có
            if (!string.IsNullOrWhiteSpace(request.ServiceTicketCode))
            {
                await EnsureCodeUnique(request.ServiceTicketCode.Trim(), id);
            }

            // Cập nhật entity
            existing.BookingId = request.BookingId;
            existing.ModifiedBy = request.ModifiedBy;
            existing.ModifiedDate = DateTime.UtcNow;
            existing.InitialIssue = request.InitialIssue;
            existing.ServiceTicketCode = request.ServiceTicketCode?.Trim();

            if (request.ServiceTicketStatus.HasValue)
            {
                existing.ServiceTicketStatus = request.ServiceTicketStatus.Value;
            }

            return await _serviceTicketRepo.UpdateAsync(id, existing);
        }

        /// <summary>
        /// Xóa Service Ticket
        /// </summary>
        public async Task<int> DeleteAsync(Guid id)
        {
            var existing = await _serviceTicketRepo.GetById(id);
            if (existing == null)
            {
                throw new NotFoundException("Không tìm thấy service ticket.");
            }
            return await _serviceTicketRepo.DeleteAsync(id);
        }

        /// <summary>
        /// Assign Service Ticket cho technical staff
        /// </summary>
        public async Task<int> AssignToTechnicalAsync(Guid id, ServiceTicketAssignDto request)
        {
            var serviceTicket = await _serviceTicketRepo.GetById(id);
            if (serviceTicket == null)
            {
                throw new NotFoundException("Không tìm thấy service ticket.");
            }

            // Kiểm tra technical staff tồn tại
            await EnsureUserExists(request.AssignedToTechnical);

            // Kiểm tra trạng thái
            if (serviceTicket.ServiceTicketStatus != ServiceTicketStatus.Pending && 
                serviceTicket.ServiceTicketStatus != ServiceTicketStatus.Assigned)
            {
                throw new ValidateException("Chỉ có thể assign service ticket ở trạng thái Pending hoặc Assigned.");
            }

            // Tạo technical task
            await AssignToTechnicalInternalAsync(id, request.AssignedToTechnical, request.Description);

            // Cập nhật trạng thái
            serviceTicket.ServiceTicketStatus = ServiceTicketStatus.Assigned;
            serviceTicket.ModifiedDate = DateTime.UtcNow;

            return await _serviceTicketRepo.UpdateAsync(id, serviceTicket);
        }

        /// <summary>
        /// Technical staff nhận task
        /// </summary>
        public async Task<int> AcceptTaskAsync(Guid id, Guid technicalStaffId)
        {
            var serviceTicket = await _serviceTicketRepo.GetById(id);
            if (serviceTicket == null)
            {
                throw new NotFoundException("Không tìm thấy service ticket.");
            }

            // Kiểm tra technical staff tồn tại
            await EnsureUserExists(technicalStaffId);

            // Tìm technical task
            var technicalTasks = await GetTechnicalTasksByServiceTicketIdAsync(id);
            var task = technicalTasks.FirstOrDefault(t => t.AssignedToTechnical == technicalStaffId);
            
            if (task == null)
            {
                throw new NotFoundException("Không tìm thấy task được assign cho technical staff này.");
            }

            if (task.TaskStatus != null && task.TaskStatus != 0)
            {
                throw new ValidateException("Task này đã được nhận hoặc hoàn thành.");
            }

            // Cập nhật task
            task.TaskStatus = 1; // In Progress
            task.AssignedAt = DateTime.UtcNow;
            await _technicalTaskRepo.UpdateAsync(task.TechnicalTaskId, task);

            // Cập nhật trạng thái service ticket
            serviceTicket.ServiceTicketStatus = ServiceTicketStatus.InProgress;
            serviceTicket.ModifiedDate = DateTime.UtcNow;

            return await _serviceTicketRepo.UpdateAsync(id, serviceTicket);
        }

        /// <summary>
        /// Thêm part vào Service Ticket
        /// </summary>
        public async Task<Guid> AddPartAsync(Guid id, ServiceTicketAddPartDto request)
        {
            var serviceTicket = await _serviceTicketRepo.GetById(id);
            if (serviceTicket == null)
            {
                throw new NotFoundException("Không tìm thấy service ticket.");
            }

            // Kiểm tra part tồn tại
            var part = await _partRepo.GetById(request.PartId);
            if (part == null)
            {
                throw new NotFoundException("Không tìm thấy part.");
            }

            // Kiểm tra số lượng tồn kho
            if (part.PartStock < request.Quantity)
            {
                throw new ValidateException($"Số lượng tồn kho không đủ. Tồn kho hiện tại: {part.PartStock}");
            }

            // Kiểm tra trạng thái - chỉ cho phép thêm part khi đang in progress
            if (serviceTicket.ServiceTicketStatus != ServiceTicketStatus.InProgress)
            {
                throw new ValidateException("Chỉ có thể thêm part khi service ticket đang ở trạng thái In Progress.");
            }

            // Tạo service ticket detail
            var detail = new ServiceTicketDetail
            {
                ServiceTicketDetailId = Guid.NewGuid(),
                ServiceTicketId = id,
                PartId = request.PartId,
                Quantity = request.Quantity
            };

            return await _serviceTicketDetailRepo.InsertAsync(detail);
        }

        /// <summary>
        /// Xóa part khỏi Service Ticket
        /// </summary>
        public async Task<int> RemovePartAsync(Guid serviceTicketId, Guid serviceTicketDetailId)
        {
            var serviceTicket = await _serviceTicketRepo.GetById(serviceTicketId);
            if (serviceTicket == null)
            {
                throw new NotFoundException("Không tìm thấy service ticket.");
            }

            var detail = await _serviceTicketDetailRepo.GetById(serviceTicketDetailId);
            if (detail == null)
            {
                throw new NotFoundException("Không tìm thấy service ticket detail.");
            }

            if (detail.ServiceTicketId != serviceTicketId)
            {
                throw new ValidateException("Service ticket detail không thuộc service ticket này.");
            }

            // Kiểm tra trạng thái
            if (serviceTicket.ServiceTicketStatus == ServiceTicketStatus.Completed)
            {
                throw new ValidateException("Không thể xóa part khi service ticket đã hoàn thành.");
            }

            return await _serviceTicketDetailRepo.DeleteAsync(serviceTicketDetailId);
        }

        #region Helpers

        /// <summary>
        /// Validate payload khi tạo mới
        /// </summary>
        private static void ValidateCreatePayload(ServiceTicketCreateDto request)
        {
            if (request.CreatedBy == Guid.Empty)
            {
                throw new ValidateException("Người tạo không được để trống.");
            }

            // Phải có CustomerId hoặc CustomerInfo
            if (!request.CustomerId.HasValue && request.CustomerInfo == null)
            {
                throw new ValidateException("Phải cung cấp CustomerId hoặc CustomerInfo.");
            }

            // Nếu có CustomerInfo thì validate
            if (request.CustomerInfo != null)
            {
                if (string.IsNullOrWhiteSpace(request.CustomerInfo.CustomerPhone))
                {
                    throw new ValidateException("Số điện thoại khách hàng không được để trống.");
                }
            }

            // Phải có VehicleId hoặc VehicleInfo
            if (!request.VehicleId.HasValue && request.VehicleInfo == null)
            {
                throw new ValidateException("Phải cung cấp VehicleId hoặc VehicleInfo.");
            }

            // Nếu có VehicleInfo thì validate
            if (request.VehicleInfo != null)
            {
                if (string.IsNullOrWhiteSpace(request.VehicleInfo.VehicleName))
                {
                    throw new ValidateException("Tên xe không được để trống.");
                }

                if (string.IsNullOrWhiteSpace(request.VehicleInfo.VehicleLicensePlate))
                {
                    throw new ValidateException("Biển số xe không được để trống.");
                }
            }

            if (!string.IsNullOrWhiteSpace(request.ServiceTicketCode) && request.ServiceTicketCode.Length > 20)
            {
                throw new ValidateException("Mã service ticket không được vượt quá 20 ký tự.");
            }

            if (request.AssignDescription != null && request.AssignDescription.Length > 255)
            {
                throw new ValidateException("Mô tả assign không được vượt quá 255 ký tự.");
            }
        }

        /// <summary>
        /// Validate payload khi cập nhật
        /// </summary>
        private static void ValidateUpdatePayload(ServiceTicketUpdateDto request)
        {
            if (request.ModifiedBy == Guid.Empty)
            {
                throw new ValidateException("Người cập nhật không được để trống.");
            }

            // Nếu có VehicleInfo thì validate
            if (request.VehicleInfo != null)
            {
                if (string.IsNullOrWhiteSpace(request.VehicleInfo.VehicleName))
                {
                    throw new ValidateException("Tên xe không được để trống.");
                }

                if (string.IsNullOrWhiteSpace(request.VehicleInfo.VehicleLicensePlate))
                {
                    throw new ValidateException("Biển số xe không được để trống.");
                }
            }

            if (!string.IsNullOrWhiteSpace(request.ServiceTicketCode) && request.ServiceTicketCode.Length > 20)
            {
                throw new ValidateException("Mã service ticket không được vượt quá 20 ký tự.");
            }
        }

        /// <summary>
        /// Kiểm tra vehicle tồn tại
        /// </summary>
        private async Task EnsureVehicleExists(Guid vehicleId)
        {
            var vehicle = await _vehicleRepo.GetById(vehicleId);
            if (vehicle == null)
            {
                throw new NotFoundException("Không tìm thấy vehicle.");
            }
        }

        /// <summary>
        /// Kiểm tra user tồn tại
        /// </summary>
        private async Task EnsureUserExists(Guid userId)
        {
            var user = await _userRepo.GetById(userId);
            if (user == null)
            {
                throw new NotFoundException("Không tìm thấy user.");
            }
        }

        /// <summary>
        /// Kiểm tra booking tồn tại
        /// </summary>
        private async Task EnsureBookingExists(Guid bookingId)
        {
            var booking = await _bookingRepo.GetById(bookingId);
            if (booking == null)
            {
                throw new NotFoundException("Không tìm thấy booking.");
            }
        }

        /// <summary>
        /// Kiểm tra mã code unique
        /// </summary>
        private async Task EnsureCodeUnique(string code, Guid? excludeId)
        {
            var exists = await _serviceTicketRepo.CheckCodeExistsAsync(code, excludeId);
            if (exists)
            {
                throw new ConflictException("Mã service ticket đã tồn tại.");
            }
        }

        /// <summary>
        /// Generate mã service ticket tự động
        /// </summary>
        private async Task<string> GenerateServiceTicketCodeAsync()
        {
            var prefix = "ST";
            var date = DateTime.UtcNow.ToString("yyyyMMdd");
            var random = new Random().Next(1000, 9999);
            var code = $"{prefix}{date}{random}";

            // Kiểm tra code đã tồn tại chưa
            var exists = await _serviceTicketRepo.CheckCodeExistsAsync(code, null);
            if (exists)
            {
                // Thử lại với random khác
                random = new Random().Next(10000, 99999);
                code = $"{prefix}{date}{random}";
            }

            return code;
        }

        /// <summary>
        /// Assign technical staff internal
        /// </summary>
        private async Task AssignToTechnicalInternalAsync(Guid serviceTicketId, Guid technicalStaffId, string description)
        {
            var technicalTask = new TechnicalTask
            {
                TechnicalTaskId = Guid.NewGuid(),
                ServiceTicketId = serviceTicketId,
                Description = description,
                AssignedToTechnical = technicalStaffId,
                AssignedAt = DateTime.UtcNow,
                TaskStatus = 0 // Pending
            };

            await _technicalTaskRepo.InsertAsync(technicalTask);
        }

        /// <summary>
        /// Lấy technical tasks theo service ticket id
        /// </summary>
        private async Task<List<TechnicalTask>> GetTechnicalTasksByServiceTicketIdAsync(Guid serviceTicketId)
        {
            // Tạm thời dùng GetAllAsync và filter, có thể tối ưu sau
            var allTasks = await _technicalTaskRepo.GetAllAsync();
            return allTasks.Where(t => t.ServiceTicketId == serviceTicketId).ToList();
        }

        #endregion
    }
}

