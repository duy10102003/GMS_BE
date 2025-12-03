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
    /// Updated by: DuyLC(02/12/2025) - Cập nhật logic theo yêu cầu mới
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
        private readonly IBaseRepo<GarageService> _garageServiceRepo;
        private readonly IBaseRepo<Inventory> _inventoryRepo;

        public ServiceTicketService(
            IServiceTicketRepo serviceTicketRepo,
            IBaseRepo<User> userRepo,
            IBaseRepo<Vehicle> vehicleRepo,
            IBaseRepo<Part> partRepo,
            IBaseRepo<Booking> bookingRepo,
            IBaseRepo<Customer> customerRepo,
            IBaseRepo<ServiceTicketDetail> serviceTicketDetailRepo,
            IBaseRepo<TechnicalTask> technicalTaskRepo,
            IBaseRepo<GarageService> garageServiceRepo,
            IBaseRepo<Inventory> inventoryRepo)
        {
            _serviceTicketRepo = serviceTicketRepo;
            _userRepo = userRepo;
            _vehicleRepo = vehicleRepo;
            _partRepo = partRepo;
            _bookingRepo = bookingRepo;
            _customerRepo = customerRepo;
            _serviceTicketDetailRepo = serviceTicketDetailRepo;
            _technicalTaskRepo = technicalTaskRepo;
            _garageServiceRepo = garageServiceRepo;
            _inventoryRepo = inventoryRepo;
        }

        #region Staff Operations

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
        public async Task<ServiceTicketDetailDto> GetByIdAsync(int id)
        {
            var result = await _serviceTicketRepo.GetDetailAsync(id);
            if (result == null)
            {
                throw new NotFoundException("Không tìm thấy service ticket.");
            }
            return result;
        }

        /// <summary>
        /// Tạo mới Service Ticket (Staff)
        /// </summary>
        public async Task<int> CreateAsync(ServiceTicketCreateDto request)
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
            int customerId;
            if (request.CustomerId.HasValue)
            {
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
                    CustomerName = request.CustomerInfo.CustomerName?.Trim(),
                    CustomerPhone = request.CustomerInfo.CustomerPhone.Trim(),
                    CustomerEmail = request.CustomerInfo.CustomerEmail?.Trim(),
                    UserId = request.CustomerInfo.UserId
                };

                var customerIdObj = await _customerRepo.InsertAsync(customer);
                customerId = customerIdObj is int id ? id : (int)customerIdObj;
            }
            else
            {
                throw new ValidateException("Phải cung cấp CustomerId hoặc CustomerInfo.");
            }

            // Xử lý vehicle: Nếu có VehicleId thì dùng, nếu không thì tạo mới từ VehicleInfo
            int vehicleId;
            if (request.VehicleId.HasValue)
            {
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
                    VehicleName = request.VehicleInfo.VehicleName.Trim(),
                    VehicleLicensePlate = request.VehicleInfo.VehicleLicensePlate.Trim(),
                    CurrentKm = request.VehicleInfo.CurrentKm,
                    Make = request.VehicleInfo.Make?.Trim(),
                    Model = request.VehicleInfo.Model?.Trim(),
                    CustomerId = customerId
                };

                var vehicleIdObj = await _vehicleRepo.InsertAsync(vehicle);
                vehicleId = vehicleIdObj is int vid ? vid : (int)vehicleIdObj;
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
                BookingId = request.BookingId,
                VehicleId = vehicleId,
                CreatedBy = request.CreatedBy,
                CreatedDate = DateTime.UtcNow,
                ServiceTicketStatus = ServiceTicketStatus.Pending,
                InitialIssue = request.InitialIssue,
                ServiceTicketCode = serviceTicketCode
            };

            var serviceTicketIdObj = await _serviceTicketRepo.InsertAsync(serviceTicket);
            var serviceTicketId = serviceTicketIdObj is int stid ? stid : (int)serviceTicketIdObj;

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
        /// Cập nhật Service Ticket (Staff: chỉnh sửa thông tin khách hàng, xe, status)
        /// </summary>
        public async Task<int> UpdateAsync(int id, ServiceTicketUpdateDto request)
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

            // Cập nhật thông tin customer nếu có
            if (request.CustomerInfo != null)
            {
                var vehicle = await _vehicleRepo.GetById(existing.VehicleId);
                if (vehicle == null)
                {
                    throw new NotFoundException("Không tìm thấy vehicle của service ticket này.");
                }

                if (vehicle.CustomerId.HasValue)
                {
                    var customer = await _customerRepo.GetById(vehicle.CustomerId.Value);
                    if (customer != null)
                    {
                        customer.CustomerName = request.CustomerInfo.CustomerName?.Trim();
                        customer.CustomerPhone = request.CustomerInfo.CustomerPhone.Trim();
                        customer.CustomerEmail = request.CustomerInfo.CustomerEmail?.Trim();
                        if (request.CustomerInfo.UserId.HasValue)
                        {
                            await EnsureUserExists(request.CustomerInfo.UserId.Value);
                            customer.UserId = request.CustomerInfo.UserId;
                        }
                        await _customerRepo.UpdateAsync(customer.CustomerId, customer);
                    }
                }
            }

            // Cập nhật thông tin vehicle nếu có (không đổi vehicle_id)
            if (request.VehicleInfo != null)
            {
                var vehicle = await _vehicleRepo.GetById(existing.VehicleId);
                if (vehicle == null)
                {
                    throw new NotFoundException("Không tìm thấy vehicle của service ticket này.");
                }

                vehicle.VehicleName = request.VehicleInfo.VehicleName.Trim();
                vehicle.VehicleLicensePlate = request.VehicleInfo.VehicleLicensePlate.Trim();
                vehicle.CurrentKm = request.VehicleInfo.CurrentKm;
                vehicle.Make = request.VehicleInfo.Make?.Trim();
                vehicle.Model = request.VehicleInfo.Model?.Trim();

                if (request.VehicleInfo.CustomerId.HasValue)
                {
                    await EnsureCustomerExists(request.VehicleInfo.CustomerId.Value);
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
        public async Task<int> DeleteAsync(int id)
        {
            var existing = await _serviceTicketRepo.GetById(id);
            if (existing == null)
            {
                throw new NotFoundException("Không tìm thấy service ticket.");
            }
            return await _serviceTicketRepo.DeleteAsync(id);
        }

        /// <summary>
        /// Assign Service Ticket cho technical staff (tạo TechnicalTask)
        /// </summary>
        public async Task<int> AssignToTechnicalAsync(int id, ServiceTicketAssignDto request)
        {
            var serviceTicket = await _serviceTicketRepo.GetById(id);
            if (serviceTicket == null)
            {
                throw new NotFoundException("Không tìm thấy service ticket.");
            }

            // Kiểm tra technical staff tồn tại
            await EnsureUserExists(request.AssignedToTechnical);

            // Tạo technical task
            var technicalTaskId = await AssignToTechnicalInternalAsync(id, request.AssignedToTechnical, request.Description);

            // Cập nhật trạng thái
            serviceTicket.ServiceTicketStatus = ServiceTicketStatus.Assigned;
            serviceTicket.ModifiedDate = DateTime.UtcNow;
            await _serviceTicketRepo.UpdateAsync(id, serviceTicket);

            return technicalTaskId;
        }

        /// <summary>
        /// Thêm part vào Service Ticket (Staff có thể thêm)
        /// </summary>
        public async Task<int> AddPartAsync(int id, ServiceTicketAddPartDto request)
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

            // Kiểm tra inventory tồn tại và số lượng (chỉ validate, không trừ)
            await ValidateInventoryQuantityAsync(request.PartId, request.Quantity);

            // Tạo service ticket detail (chưa trừ inventory, sẽ trừ khi confirm)
            var detail = new ServiceTicketDetail
            {
                ServiceTicketId = id,
                PartId = request.PartId,
                Quantity = request.Quantity
            };

            var detailIdObj = await _serviceTicketDetailRepo.InsertAsync(detail);
            return detailIdObj is int did ? did : (int)detailIdObj;
        }

        /// <summary>
        /// Thêm garage service vào Service Ticket (Staff có thể thêm)
        /// </summary>
        public async Task<int> AddGarageServiceAsync(int id, ServiceTicketAddGarageServiceDto request)
        {
            var serviceTicket = await _serviceTicketRepo.GetById(id);
            if (serviceTicket == null)
            {
                throw new NotFoundException("Không tìm thấy service ticket.");
            }

            // Kiểm tra garage service tồn tại
            var garageService = await _garageServiceRepo.GetById(request.GarageServiceId);
            if (garageService == null)
            {
                throw new NotFoundException("Không tìm thấy garage service.");
            }

            // Tạo service ticket detail
            var detail = new ServiceTicketDetail
            {
                ServiceTicketId = id,
                GarageServiceId = request.GarageServiceId,
                Quantity = request.Quantity
            };

            var detailIdObj = await _serviceTicketDetailRepo.InsertAsync(detail);
            return detailIdObj is int did ? did : (int)detailIdObj;
        }

        /// <summary>
        /// Xóa part/garage service khỏi Service Ticket
        /// </summary>
        public async Task<int> RemoveDetailAsync(int serviceTicketId, int serviceTicketDetailId)
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
                throw new ValidateException("Không thể xóa khi service ticket đã hoàn thành.");
            }

            // Nếu là part và service ticket đã completed (đã trừ inventory), rollback inventory
            if (detail.PartId.HasValue && detail.Quantity.HasValue && serviceTicket.ServiceTicketStatus == ServiceTicketStatus.Completed)
            {
                await RollbackInventoryQuantityAsync(detail.PartId.Value, detail.Quantity.Value);
            }

            return await _serviceTicketDetailRepo.DeleteAsync(serviceTicketDetailId);
        }

        /// <summary>
        /// Duyệt parts và services do Mechanic đề xuất
        /// </summary>
        public async Task<int> ApproveMechanicProposalAsync(int technicalTaskId, ServiceTicketUpdatePartsServicesDto request, int staffId)
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

            // Kiểm tra staff tồn tại
            await EnsureUserExists(staffId);

            // Xóa các detail cũ của task này (nếu có)
            var existingDetails = await _serviceTicketDetailRepo.GetAllAsync();
            var taskDetails = existingDetails.Where(d => d.ServiceTicketId == serviceTicket.ServiceTicketId).ToList();
            
            // Thêm/cập nhật parts
            foreach (var partDto in request.Parts)
            {
                var part = await _partRepo.GetById(partDto.PartId);
                if (part == null)
                {
                    throw new NotFoundException($"Không tìm thấy part với ID: {partDto.PartId}");
                }

                if (partDto.ServiceTicketDetailId.HasValue)
                {
                    // Update existing - rollback số cũ trước
                    var oldDetail = await _serviceTicketDetailRepo.GetById(partDto.ServiceTicketDetailId.Value);
                    if (oldDetail != null && oldDetail.PartId.HasValue && oldDetail.Quantity.HasValue)
                    {
                        // Rollback số lượng cũ (nếu service ticket đã completed)
                        if (serviceTicket.ServiceTicketStatus == ServiceTicketStatus.Completed)
                        {
                            await RollbackInventoryQuantityAsync(oldDetail.PartId.Value, oldDetail.Quantity.Value);
                        }
                    }

                    // Validate số lượng mới
                    await ValidateInventoryQuantityAsync(partDto.PartId, partDto.Quantity);

                    // Update detail
                    var detail = await _serviceTicketDetailRepo.GetById(partDto.ServiceTicketDetailId.Value);
                    if (detail != null)
                    {
                        detail.PartId = partDto.PartId;
                        detail.Quantity = partDto.Quantity;
                        await _serviceTicketDetailRepo.UpdateAsync(partDto.ServiceTicketDetailId.Value, detail);
                    }
                }
                else
                {
                    // Insert new - chỉ validate, chưa trừ
                    await ValidateInventoryQuantityAsync(partDto.PartId, partDto.Quantity);

                    var detail = new ServiceTicketDetail
                    {
                        ServiceTicketId = serviceTicket.ServiceTicketId,
                        PartId = partDto.PartId,
                        Quantity = partDto.Quantity
                    };
                    var _ = await _serviceTicketDetailRepo.InsertAsync(detail);
                }
            }

            // Thêm/cập nhật garage services
            foreach (var serviceDto in request.GarageServices)
            {
                var garageService = await _garageServiceRepo.GetById(serviceDto.GarageServiceId);
                if (garageService == null)
                {
                    throw new NotFoundException($"Không tìm thấy garage service với ID: {serviceDto.GarageServiceId}");
                }

                if (serviceDto.ServiceTicketDetailId.HasValue)
                {
                    // Update existing
                    var detail = await _serviceTicketDetailRepo.GetById(serviceDto.ServiceTicketDetailId.Value);
                    if (detail != null)
                    {
                        detail.GarageServiceId = serviceDto.GarageServiceId;
                        detail.Quantity = serviceDto.Quantity;
                        await _serviceTicketDetailRepo.UpdateAsync(serviceDto.ServiceTicketDetailId.Value, detail);
                    }
                }
                else
                {
                    // Insert new
                    var detail = new ServiceTicketDetail
                    {
                        ServiceTicketId = serviceTicket.ServiceTicketId,
                        GarageServiceId = serviceDto.GarageServiceId,
                        Quantity = serviceDto.Quantity
                    };
                    var _ = await _serviceTicketDetailRepo.InsertAsync(detail);
                }
            }

            // Cập nhật trạng thái service ticket
            serviceTicket.ServiceTicketStatus = ServiceTicketStatus.InProgress;
            serviceTicket.ModifiedBy = staffId;
            serviceTicket.ModifiedDate = DateTime.UtcNow;
            return await _serviceTicketRepo.UpdateAsync(serviceTicket.ServiceTicketId, serviceTicket);
        }

        #endregion

        #region Mechanic Operations

        /// <summary>
        /// Lấy danh sách tasks của Mechanic
        /// </summary>
        public async Task<PagedResult<MechanicTaskDto>> GetMyTasksAsync(int mechanicId, ServiceTicketFilterDtoRequest filter)
        {
            var result = await _serviceTicketRepo.GetMechanicTasksAsync(mechanicId, filter);
            if (result == null)
            {
                throw new NotFoundException("Không tìm thấy danh sách tasks.");
            }
            return result;
        }

        /// <summary>
        /// Lấy chi tiết task của Mechanic
        /// </summary>
        public async Task<MechanicTaskDto> GetMyTaskDetailAsync(int technicalTaskId, int mechanicId)
        {
            var result = await _serviceTicketRepo.GetMechanicTaskDetailAsync(technicalTaskId, mechanicId);
            if (result == null)
            {
                throw new NotFoundException("Không tìm thấy task.");
            }
            return result;
        }

        /// <summary>
        /// Mechanic đề xuất parts và services (gửi cho Staff duyệt)
        /// </summary>
        public async Task<int> ProposePartsServicesAsync(int technicalTaskId, ServiceTicketUpdatePartsServicesDto request, int mechanicId)
        {
            var technicalTask = await _technicalTaskRepo.GetById(technicalTaskId);
            if (technicalTask == null)
            {
                throw new NotFoundException("Không tìm thấy technical task.");
            }

            // Kiểm tra task thuộc về mechanic này
            if (technicalTask.AssignedToTechnical != mechanicId)
            {
                throw new ValidateException("Bạn không có quyền chỉnh sửa task này.");
            }

            // Validate parts và services
            foreach (var partDto in request.Parts)
            {
                var part = await _partRepo.GetById(partDto.PartId);
                if (part == null)
                {
                    throw new NotFoundException($"Không tìm thấy part với ID: {partDto.PartId}");
                }
            }

            foreach (var serviceDto in request.GarageServices)
            {
                var garageService = await _garageServiceRepo.GetById(serviceDto.GarageServiceId);
                if (garageService == null)
                {
                    throw new NotFoundException($"Không tìm thấy garage service với ID: {serviceDto.GarageServiceId}");
                }
            }

            // Lưu đề xuất vào service ticket detail (có thể đánh dấu là pending approval)
            // Ở đây ta sẽ lưu trực tiếp, Staff sẽ duyệt sau
            var serviceTicket = await _serviceTicketRepo.GetById(technicalTask.ServiceTicketId);
            if (serviceTicket == null)
            {
                throw new NotFoundException("Không tìm thấy service ticket.");
            }

            // Xóa các detail cũ (nếu có)
            var existingDetails = await _serviceTicketDetailRepo.GetAllAsync();
            var taskDetails = existingDetails
                .Where(d => d.ServiceTicketId == serviceTicket.ServiceTicketId && d.IsDeleted == 0)
                .ToList();

            // Thêm parts
            foreach (var partDto in request.Parts)
            {
                // Validate part tồn tại
                var part = await _partRepo.GetById(partDto.PartId);
                if (part == null)
                {
                    throw new NotFoundException($"Không tìm thấy part với ID: {partDto.PartId}");
                }

                if (partDto.ServiceTicketDetailId.HasValue)
                {
                    // Update existing - rollback số cũ nếu đã completed
                    var oldDetail = await _serviceTicketDetailRepo.GetById(partDto.ServiceTicketDetailId.Value);
                    if (oldDetail != null && oldDetail.PartId.HasValue && oldDetail.Quantity.HasValue)
                    {
                        // Kiểm tra service ticket status để rollback nếu cần
                        if (serviceTicket.ServiceTicketStatus == ServiceTicketStatus.Completed)
                        {
                            await RollbackInventoryQuantityAsync(oldDetail.PartId.Value, oldDetail.Quantity.Value);
                        }
                    }

                    // Validate số lượng mới
                    await ValidateInventoryQuantityAsync(partDto.PartId, partDto.Quantity);

                    var detail = await _serviceTicketDetailRepo.GetById(partDto.ServiceTicketDetailId.Value);
                    if (detail != null)
                    {
                        detail.PartId = partDto.PartId;
                        detail.Quantity = partDto.Quantity;
                        await _serviceTicketDetailRepo.UpdateAsync(partDto.ServiceTicketDetailId.Value, detail);
                    }
                }
                else
                {
                    // Insert new - chỉ validate
                    await ValidateInventoryQuantityAsync(partDto.PartId, partDto.Quantity);

                    var detail = new ServiceTicketDetail
                    {
                        ServiceTicketId = serviceTicket.ServiceTicketId,
                        PartId = partDto.PartId,
                        Quantity = partDto.Quantity
                    };
                    var _ = await _serviceTicketDetailRepo.InsertAsync(detail);
                }
            }

            // Thêm garage services
            foreach (var serviceDto in request.GarageServices)
            {
                if (serviceDto.ServiceTicketDetailId.HasValue)
                {
                    var detail = await _serviceTicketDetailRepo.GetById(serviceDto.ServiceTicketDetailId.Value);
                    if (detail != null)
                    {
                        detail.GarageServiceId = serviceDto.GarageServiceId;
                        detail.Quantity = serviceDto.Quantity;
                        await _serviceTicketDetailRepo.UpdateAsync(serviceDto.ServiceTicketDetailId.Value, detail);
                    }
                }
                else
                {
                    var detail = new ServiceTicketDetail
                    {
                        ServiceTicketId = serviceTicket.ServiceTicketId,
                        GarageServiceId = serviceDto.GarageServiceId,
                        Quantity = serviceDto.Quantity
                    };
                    var _ = await _serviceTicketDetailRepo.InsertAsync(detail);
                }
            }

            return 1;
        }

        /// <summary>
        /// Mechanic bắt đầu làm task (duyệt và bắt đầu)
        /// </summary>
        public async Task<int> StartTaskAsync(int technicalTaskId, int mechanicId)
        {
            var technicalTask = await _technicalTaskRepo.GetById(technicalTaskId);
            if (technicalTask == null)
            {
                throw new NotFoundException("Không tìm thấy technical task.");
            }

            // Kiểm tra task thuộc về mechanic này
            if (technicalTask.AssignedToTechnical != mechanicId)
            {
                throw new ValidateException("Bạn không có quyền thực hiện task này.");
            }

            // Kiểm tra trạng thái
            if (technicalTask.TaskStatus != null && technicalTask.TaskStatus != 0)
            {
                throw new ValidateException("Task này đã được bắt đầu hoặc hoàn thành.");
            }

            // Cập nhật task
            technicalTask.TaskStatus = 1; // In Progress
            technicalTask.AssignedAt = DateTime.UtcNow;
            await _technicalTaskRepo.UpdateAsync(technicalTaskId, technicalTask);

            // Cập nhật trạng thái service ticket
            var serviceTicket = await _serviceTicketRepo.GetById(technicalTask.ServiceTicketId);
            if (serviceTicket != null)
            {
                serviceTicket.ServiceTicketStatus = ServiceTicketStatus.InProgress;
                serviceTicket.ModifiedDate = DateTime.UtcNow;
                await _serviceTicketRepo.UpdateAsync(serviceTicket.ServiceTicketId, serviceTicket);
            }

            return 1;
        }

        /// <summary>
        /// Mechanic confirm task đã hoàn thành
        /// </summary>
        public async Task<int> ConfirmTaskAsync(int technicalTaskId, int mechanicId)
        {
            var technicalTask = await _technicalTaskRepo.GetById(technicalTaskId);
            if (technicalTask == null)
            {
                throw new NotFoundException("Không tìm thấy technical task.");
            }

            // Kiểm tra task thuộc về mechanic này
            if (technicalTask.AssignedToTechnical != mechanicId)
            {
                throw new ValidateException("Bạn không có quyền confirm task này.");
            }

            // Kiểm tra trạng thái
            if (technicalTask.TaskStatus != 1)
            {
                throw new ValidateException("Chỉ có thể confirm task đang ở trạng thái In Progress.");
            }

            // Cập nhật task
            technicalTask.TaskStatus = 2; // Completed
            technicalTask.ConfirmedBy = mechanicId;
            technicalTask.ConfirmedAt = DateTime.UtcNow;
            await _technicalTaskRepo.UpdateAsync(technicalTaskId, technicalTask);

            // Trừ inventory cho tất cả parts trong service ticket này
            var serviceTicket = await _serviceTicketRepo.GetById(technicalTask.ServiceTicketId);
            if (serviceTicket != null)
            {
                // Lấy tất cả parts trong service ticket
                var allDetails = await _serviceTicketDetailRepo.GetAllAsync();
                var serviceTicketDetails = allDetails
                    .Where(d => d.ServiceTicketId == serviceTicket.ServiceTicketId 
                             && d.PartId.HasValue 
                             && d.Quantity.HasValue
                             && d.IsDeleted == 0)
                    .ToList();

                // Trừ inventory cho mỗi part
                foreach (var detail in serviceTicketDetails)
                {
                    if (detail.PartId.HasValue && detail.Quantity.HasValue)
                    {
                        await DeductInventoryQuantityAsync(detail.PartId.Value, detail.Quantity.Value);
                    }
                }
            }

            // Kiểm tra xem tất cả tasks của service ticket đã hoàn thành chưa
            var allTasks = await _technicalTaskRepo.GetAllAsync();
            var serviceTicketTasks = allTasks
                .Where(t => t.ServiceTicketId == technicalTask.ServiceTicketId && t.IsDeleted == 0)
                .ToList();

            // Nếu tất cả tasks đã hoàn thành, cập nhật status service ticket
            if (serviceTicketTasks.All(t => t.TaskStatus == 2))
            {
                if (serviceTicket != null)
                {
                    serviceTicket.ServiceTicketStatus = ServiceTicketStatus.Completed;
                    serviceTicket.ModifiedDate = DateTime.UtcNow;
                    await _serviceTicketRepo.UpdateAsync(serviceTicket.ServiceTicketId, serviceTicket);
                }
            }

            return 1;
        }

        #endregion

        #region Inventory Helpers

        /// <summary>
        /// Validate số lượng inventory có đủ không (không trừ)
        /// </summary>
        private async Task ValidateInventoryQuantityAsync(int partId, int quantity)
        {
            var inventory = await _inventoryRepo.GetById(partId);
            if (inventory == null)
            {
                throw new NotFoundException($"Không tìm thấy inventory cho part với ID: {partId}");
            }

            if (inventory.InventoryQuantity < quantity)
            {
                throw new ValidateException($"Số lượng tồn kho không đủ. Tồn kho hiện tại: {inventory.InventoryQuantity}");
            }
        }

        /// <summary>
        /// Trừ số lượng trong inventory
        /// </summary>
        private async Task DeductInventoryQuantityAsync(int partId, int quantity)
        {
            var inventory = await _inventoryRepo.GetById(partId);
            if (inventory == null)
            {
                throw new NotFoundException($"Không tìm thấy inventory cho part với ID: {partId}");
            }

            if (inventory.InventoryQuantity < quantity)
            {
                throw new ValidateException($"Số lượng tồn kho không đủ để trừ. Tồn kho hiện tại: {inventory.InventoryQuantity}, cần trừ: {quantity}");
            }

            inventory.InventoryQuantity -= quantity;
            await _inventoryRepo.UpdateAsync(partId, inventory);
        }

        /// <summary>
        /// Rollback (hoàn trả) số lượng trong inventory
        /// </summary>
        private async Task RollbackInventoryQuantityAsync(int partId, int quantity)
        {
            var inventory = await _inventoryRepo.GetById(partId);
            if (inventory == null)
            {
                throw new NotFoundException($"Không tìm thấy inventory cho part với ID: {partId}");
            }

            inventory.InventoryQuantity += quantity;
            await _inventoryRepo.UpdateAsync(partId, inventory);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Validate payload khi tạo mới
        /// </summary>
        private static void ValidateCreatePayload(ServiceTicketCreateDto request)
        {
            if (request.CreatedBy == 0)
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
            if (request.ModifiedBy == 0)
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

            // Nếu có CustomerInfo thì validate
            if (request.CustomerInfo != null)
            {
                if (string.IsNullOrWhiteSpace(request.CustomerInfo.CustomerPhone))
                {
                    throw new ValidateException("Số điện thoại khách hàng không được để trống.");
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
        private async Task EnsureVehicleExists(int vehicleId)
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
        private async Task EnsureUserExists(int userId)
        {
            var user = await _userRepo.GetById(userId);
            if (user == null)
            {
                throw new NotFoundException("Không tìm thấy user.");
            }
        }

        /// <summary>
        /// Kiểm tra customer tồn tại
        /// </summary>
        private async Task EnsureCustomerExists(int customerId)
        {
            var customer = await _customerRepo.GetById(customerId);
            if (customer == null)
            {
                throw new NotFoundException("Không tìm thấy customer.");
            }
        }

        /// <summary>
        /// Kiểm tra booking tồn tại
        /// </summary>
        private async Task EnsureBookingExists(int bookingId)
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
        private async Task EnsureCodeUnique(string code, int? excludeId)
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
        private async Task<int> AssignToTechnicalInternalAsync(int serviceTicketId, int technicalStaffId, string description)
        {
            var technicalTask = new TechnicalTask
            {
                ServiceTicketId = serviceTicketId,
                Description = description,
                AssignedToTechnical = technicalStaffId,
                AssignedAt = DateTime.UtcNow,
                TaskStatus = 0 // Pending
            };

            var taskIdObj = await _technicalTaskRepo.InsertAsync(technicalTask);
            return taskIdObj is int tid ? tid : (int)taskIdObj;
        }

        #endregion
    }
}
