using FluentAssertions;
using Moq;
using NUnit.Framework;
using SWP.Core.Dtos;
using SWP.Core.Dtos.SeriveTicketDto;
using SWP.Core.Entities;
using SWP.Core.Exceptions;
using SWP.Core.Interfaces.Repositories;

namespace SWP.Core.UnitTest.ServiceTicketService.StaffOperation
{
    [TestFixture]
    public class CreateAsyncTest
    {
        private Mock<IServiceTicketRepo> _serviceTicketRepoMock;
        private Mock<IBaseRepo<User>> _userRepoMock;
        private Mock<IBaseRepo<Booking>> _bookingRepoMock;
        private Mock<IBaseRepo<Customer>> _customerRepoMock;
        private Mock<IBaseRepo<Vehicle>> _vehicleRepoMock;
        private Mock<IBaseRepo<Part>> _partRepoMock;
        private Mock<IBaseRepo<ServiceTicketDetail>> _serviceTicketDetailRepoMock;
        private Mock<IBaseRepo<GarageService>> _garageServiceRepoMock;
        private Mock<IBaseRepo<TechnicalTask>> _technicalTaskRepoMock;
        private SWP.Core.Services.ServiceTicketService _service;

        [SetUp]
        public void SetUp()
        {
            _serviceTicketRepoMock = new Mock<IServiceTicketRepo>();
            _userRepoMock = new Mock<IBaseRepo<User>>();
            _bookingRepoMock = new Mock<IBaseRepo<Booking>>();
            _customerRepoMock = new Mock<IBaseRepo<Customer>>();
            _vehicleRepoMock = new Mock<IBaseRepo<Vehicle>>();
            _partRepoMock = new Mock<IBaseRepo<Part>>();
            _serviceTicketDetailRepoMock = new Mock<IBaseRepo<ServiceTicketDetail>>();
            _garageServiceRepoMock = new Mock<IBaseRepo<GarageService>>();
            _technicalTaskRepoMock = new Mock<IBaseRepo<TechnicalTask>>();

            _service = new SWP.Core.Services.ServiceTicketService(
                _serviceTicketRepoMock.Object,
                _userRepoMock.Object,
                _vehicleRepoMock.Object,
                _partRepoMock.Object,
                _bookingRepoMock.Object,
                _customerRepoMock.Object,
                _serviceTicketDetailRepoMock.Object,
                _technicalTaskRepoMock.Object,
                _garageServiceRepoMock.Object
            );
        }

        [Test]
        public void CreateAsync_WhenCreatedByIsZero_ShouldThrowValidateException()
        {
            var request = new ServiceTicketCreateDto { CreatedBy = 0 };
            Func<Task> act = async () => await _service.CreateAsync(request);
            act.Should().ThrowAsync<ValidateException>().WithMessage("Người tạo không được để trống.");
        }

        [Test]
        public void CreateAsync_WhenNoCustomerProvided_ShouldThrowValidateException()
        {
            var request = new ServiceTicketCreateDto { CreatedBy = 1, CustomerId = null, CustomerInfo = null };
            Func<Task> act = async () => await _service.CreateAsync(request);
            act.Should().ThrowAsync<ValidateException>().WithMessage("Phải cung cấp CustomerId hoặc CustomerInfo.");
        }

        [Test]
        public void CreateAsync_WhenCustomerPhoneEmpty_ShouldThrowValidateException()
        {
            var request = new ServiceTicketCreateDto 
            { 
                CreatedBy = 1, 
                CustomerInfo = new CustomerInfoDto { CustomerPhone = "" } 
            };
            Func<Task> act = async () => await _service.CreateAsync(request);
            act.Should().ThrowAsync<ValidateException>().WithMessage("Số điện thoại khách hàng không được để trống.");
        }

        [Test]
        public void CreateAsync_WhenNoVehicleProvided_ShouldThrowValidateException()
        {
            var request = new ServiceTicketCreateDto 
            { 
                CreatedBy = 1, 
                CustomerId = 1, // Valid customer
                VehicleId = null, 
                VehicleInfo = null 
            };
            Func<Task> act = async () => await _service.CreateAsync(request);
            act.Should().ThrowAsync<ValidateException>().WithMessage("Phải cung cấp VehicleId hoặc VehicleInfo.");
        }

        [Test]
        public void CreateAsync_WhenVehicleNameEmpty_ShouldThrowValidateException()
        {
            var request = new ServiceTicketCreateDto 
            { 
                CreatedBy = 1, 
                CustomerId = 1,
                VehicleInfo = new VehicleInfoDto { VehicleName = "" } 
            };
            Func<Task> act = async () => await _service.CreateAsync(request);
            act.Should().ThrowAsync<ValidateException>().WithMessage("Tên xe không được để trống.");
        }

        [Test]
        public void CreateAsync_WhenVehicleLicensePlateEmpty_ShouldThrowValidateException()
        {
            var request = new ServiceTicketCreateDto 
            { 
                CreatedBy = 1, 
                CustomerId = 1,
                VehicleInfo = new VehicleInfoDto { VehicleName = "Car", VehicleLicensePlate = "" } 
            };
            Func<Task> act = async () => await _service.CreateAsync(request);
            act.Should().ThrowAsync<ValidateException>().WithMessage("Biển số xe không được để trống.");
        }

        [Test]
        public void CreateAsync_WhenCodeTooLong_ShouldThrowValidateException()
        {
             var request = new ServiceTicketCreateDto 
            { 
                CreatedBy = 1, 
                CustomerId = 1,
                VehicleId = 1,
                ServiceTicketCode = new string('A', 21)
            };
            Func<Task> act = async () => await _service.CreateAsync(request);
            act.Should().ThrowAsync<ValidateException>().WithMessage("Mã service ticket không được vượt quá 20 ký tự.");
        }

        [Test]
        public void CreateAsync_WhenAssignDescriptionTooLong_ShouldThrowValidateException()
        {
             var request = new ServiceTicketCreateDto 
            { 
                CreatedBy = 1, 
                CustomerId = 1,
                VehicleId = 1,
                AssignDescription = new string('A', 256)
            };
            Func<Task> act = async () => await _service.CreateAsync(request);
            act.Should().ThrowAsync<ValidateException>().WithMessage("Mô tả assign không được vượt quá 255 ký tự.");
        }

        [Test]
        public void CreateAsync_WhenUserNotFound_ShouldThrowNotFoundException()
        {
            var request = new ServiceTicketCreateDto { CreatedBy = 999, CustomerId = 1, VehicleId = 1 };
            _userRepoMock.Setup(x => x.GetById(999)).ReturnsAsync((User)null!);
            
            Func<Task> act = async () => await _service.CreateAsync(request);
            act.Should().ThrowAsync<NotFoundException>().WithMessage("Không tìm thấy user.");
        }

        [Test]
        public void CreateAsync_WhenBookingNotFound_ShouldThrowNotFoundException()
        {
            var request = new ServiceTicketCreateDto { CreatedBy = 1, CustomerId = 1, VehicleId = 1, BookingId = 999 };
            _userRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(new User());
            _bookingRepoMock.Setup(x => x.GetById(999)).ReturnsAsync((Booking)null!);

            Func<Task> act = async () => await _service.CreateAsync(request);
            act.Should().ThrowAsync<NotFoundException>().WithMessage("Không tìm thấy booking.");
        }
        
        [Test]
        public void CreateAsync_WhenCustomerNotFound_ShouldThrowNotFoundException()
        {
            var request = new ServiceTicketCreateDto { CreatedBy = 1, CustomerId = 999, VehicleId = 1 };
            _userRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(new User());
            _customerRepoMock.Setup(x => x.GetById(999)).ReturnsAsync((Customer)null!);

            Func<Task> act = async () => await _service.CreateAsync(request);
            act.Should().ThrowAsync<NotFoundException>().WithMessage("Không tìm thấy customer.");
        }

        [Test]
        public void CreateAsync_CustomerCreation_WhenUserNotFound_ShouldThrowNotFoundException()
        {
            var request = new ServiceTicketCreateDto 
            { 
                CreatedBy = 1, 
                CustomerInfo = new CustomerInfoDto { CustomerPhone = "123", UserId = 999 } 
            };
            
            _userRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(new User());
            // Fail on Customer Info User check
            _userRepoMock.Setup(x => x.GetById(999)).ReturnsAsync((User)null!);

            Func<Task> act = async () => await _service.CreateAsync(request);
            act.Should().ThrowAsync<NotFoundException>().WithMessage("Không tìm thấy user.");
        }

        [Test]
        public void CreateAsync_CustomerCreation_WhenFailed_ShouldThrowValidateException()
        {
            var request = new ServiceTicketCreateDto 
            { 
                CreatedBy = 1, 
                CustomerInfo = new CustomerInfoDto { CustomerPhone = "123" } 
            };
            
            _userRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(new User());
            _customerRepoMock.Setup(x => x.InsertAsync(It.IsAny<Customer>())).ReturnsAsync(null); // Return null

            Func<Task> act = async () => await _service.CreateAsync(request);
            act.Should().ThrowAsync<ValidateException>().WithMessage("Không thể lấy ID của customer vừa tạo.");
        }

        [Test]
        public void CreateAsync_WhenVehicleNotFound_ShouldThrowNotFoundException()
        {
            var request = new ServiceTicketCreateDto { CreatedBy = 1, CustomerId = 1, VehicleId = 999 };
            _userRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(new User());
            _customerRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(new Customer());
            _vehicleRepoMock.Setup(x => x.GetById(999)).ReturnsAsync((Vehicle)null!);

            Func<Task> act = async () => await _service.CreateAsync(request);
            act.Should().ThrowAsync<NotFoundException>().WithMessage("Không tìm thấy vehicle.");
        }

        [Test]
        public void CreateAsync_WhenVehicleNotBelongToCustomer_ShouldThrowValidateException()
        {
            var request = new ServiceTicketCreateDto { CreatedBy = 1, CustomerId = 1, VehicleId = 1 };
            _userRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(new User());
            _customerRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(new Customer { CustomerId = 1 });
            _vehicleRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(new Vehicle { VehicleId = 1, CustomerId = 2 }); // Diff customer

            Func<Task> act = async () => await _service.CreateAsync(request);
            act.Should().ThrowAsync<ValidateException>().WithMessage("Vehicle không thuộc về customer được chọn.");
        }
        
        [Test]
        public void CreateAsync_VehicleCreation_WhenFailed_ShouldThrowValidateException()
        {
            var request = new ServiceTicketCreateDto 
            { 
                CreatedBy = 1, 
                CustomerId = 1, 
                VehicleInfo = new VehicleInfoDto { VehicleName = "Car", VehicleLicensePlate = "ABC" }
            };
            
            _userRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(new User());
            _customerRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(new Customer { CustomerId = 1 });
            _vehicleRepoMock.Setup(x => x.InsertAsync(It.IsAny<Vehicle>())).ReturnsAsync(null);

            Func<Task> act = async () => await _service.CreateAsync(request);
            act.Should().ThrowAsync<ValidateException>().WithMessage("Không thể lấy ID của vehicle vừa tạo.");
        }

        [Test]
        public void CreateAsync_WhenCodeConflict_ShouldThrowConflictException()
        {
             var request = new ServiceTicketCreateDto { CreatedBy = 1, CustomerId = 1, VehicleId = 1, ServiceTicketCode = "EXISTING" };
             _userRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(new User());
             _customerRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(new Customer { CustomerId = 1 });
             _vehicleRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(new Vehicle { VehicleId = 1, CustomerId = 1 });
             _serviceTicketRepoMock.Setup(x => x.CheckCodeExistsAsync("EXISTING", null)).ReturnsAsync(true);

             Func<Task> act = async () => await _service.CreateAsync(request);
             act.Should().ThrowAsync<ConflictException>().WithMessage("Mã service ticket đã tồn tại.");
        }

        [Test]
        public void CreateAsync_WhenPartNotFound_ShouldThrowNotFoundException()
        {
             var request = new ServiceTicketCreateDto 
             { 
                 CreatedBy = 1, CustomerId = 1, VehicleId = 1,
                 Parts = new List<ServiceTicketAddPartDto> { new ServiceTicketAddPartDto { PartId = 999, Quantity = 1 } }
             };
             
             _userRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(new User());
             _customerRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(new Customer { CustomerId = 1 });
             _vehicleRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(new Vehicle { VehicleId = 1, CustomerId = 1 });
             _serviceTicketRepoMock.Setup(x => x.CheckCodeExistsAsync(It.IsAny<string>(), null)).ReturnsAsync(false);
             _serviceTicketRepoMock.Setup(x => x.InsertAsync(It.IsAny<ServiceTicket>())).ReturnsAsync(1);
             
             _partRepoMock.Setup(x => x.GetById(999)).ReturnsAsync((Part)null!);

             Func<Task> act = async () => await _service.CreateAsync(request);
             act.Should().ThrowAsync<NotFoundException>().WithMessage("Không tìm thấy part với ID: 999");
        }

        [Test]
        public void CreateAsync_WhenPartQuantityNotEnough_ShouldThrowValidateException()
        {
             var request = new ServiceTicketCreateDto 
             { 
                 CreatedBy = 1, CustomerId = 1, VehicleId = 1,
                 Parts = new List<ServiceTicketAddPartDto> { new ServiceTicketAddPartDto { PartId = 1, Quantity = 10 } }
             };
             
             _userRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(new User());
             _customerRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(new Customer { CustomerId = 1 });
             _vehicleRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(new Vehicle { VehicleId = 1, CustomerId = 1 });
             _serviceTicketRepoMock.Setup(x => x.InsertAsync(It.IsAny<ServiceTicket>())).ReturnsAsync(1);
             
             // Stock is 5. Request 10.
             _partRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(new Part { PartId = 1, PartQuantity = 5 });

             Func<Task> act = async () => await _service.CreateAsync(request);
             act.Should().ThrowAsync<ValidateException>().WithMessage($"Số lượng tồn kho không đủ. Tồn kho hiện tại: 5");
        }

        [Test]
        public void CreateAsync_WhenGarageServiceNotFound_ShouldThrowNotFoundException()
        {
             var request = new ServiceTicketCreateDto 
             { 
                 CreatedBy = 1, CustomerId = 1, VehicleId = 1,
                 GarageServiceIds = new List<int> { 999 }
             };
             
             _userRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(new User());
             _customerRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(new Customer { CustomerId = 1 });
             _vehicleRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(new Vehicle { VehicleId = 1, CustomerId = 1 });
             _serviceTicketRepoMock.Setup(x => x.InsertAsync(It.IsAny<ServiceTicket>())).ReturnsAsync(1);
             
             _garageServiceRepoMock.Setup(x => x.GetById(999)).ReturnsAsync((GarageService)null!);

             Func<Task> act = async () => await _service.CreateAsync(request);
             act.Should().ThrowAsync<NotFoundException>().WithMessage("Không tìm thấy garage service.");
        }

        [Test]
        public async Task CreateAsync_Success_WithNewCustomerVehicle()
        {
             var request = new ServiceTicketCreateDto 
             { 
                 CreatedBy = 1, 
                 CustomerInfo = new CustomerInfoDto { CustomerPhone = "0900" },
                 VehicleInfo = new VehicleInfoDto { VehicleName = "Car", VehicleLicensePlate = "A-1" }
             };
             
             _userRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(new User());
             _customerRepoMock.Setup(x => x.InsertAsync(It.IsAny<Customer>())).ReturnsAsync(10);
             _vehicleRepoMock.Setup(x => x.InsertAsync(It.IsAny<Vehicle>())).ReturnsAsync(20);
             _serviceTicketRepoMock.Setup(x => x.InsertAsync(It.IsAny<ServiceTicket>())).ReturnsAsync(100);

             var result = await _service.CreateAsync(request);
             
             result.Should().Be(100);
             _customerRepoMock.Verify(x => x.InsertAsync(It.Is<Customer>(c => c.CustomerPhone == "0900")), Times.Once);
             _vehicleRepoMock.Verify(x => x.InsertAsync(It.Is<Vehicle>(v => v.CustomerId == 10)), Times.Once); // Linked to new customer
             _serviceTicketRepoMock.Verify(x => x.InsertAsync(It.Is<ServiceTicket>(s => s.VehicleId == 20)), Times.Once);
        }
        
        [Test]
        public async Task CreateAsync_Success_WithPartsAndServices()
        {
             var request = new ServiceTicketCreateDto 
             { 
                 CreatedBy = 1, CustomerId = 1, VehicleId = 1,
                 Parts = new List<ServiceTicketAddPartDto> { new ServiceTicketAddPartDto { PartId = 1, Quantity = 2 } },
                 GarageServiceIds = new List<int> { 2 }
             };
             
             _userRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(new User());
             _customerRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(new Customer { CustomerId = 1 });
             _vehicleRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(new Vehicle { VehicleId = 1, CustomerId = 1 });
             _serviceTicketRepoMock.Setup(x => x.InsertAsync(It.IsAny<ServiceTicket>())).ReturnsAsync(100);
             
             _partRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(new Part { PartId = 1, PartQuantity = 10 });
             _partRepoMock.Setup(x => x.UpdateAsync(1, It.IsAny<Part>())).ReturnsAsync(1);
             
             _garageServiceRepoMock.Setup(x => x.GetById(2)).ReturnsAsync(new GarageService { GarageServiceId = 2 });
             _serviceTicketDetailRepoMock.Setup(x => x.InsertAsync(It.IsAny<ServiceTicketDetail>())).ReturnsAsync(1);

             var result = await _service.CreateAsync(request);
             
             result.Should().Be(100);
             // Deduct check: 10 - 2 = 8
             _partRepoMock.Verify(x => x.UpdateAsync(1, It.Is<Part>(p => p.PartQuantity == 8)), Times.Once);
             // Insert Details check
             _serviceTicketDetailRepoMock.Verify(x => x.InsertAsync(It.Is<ServiceTicketDetail>(d => d.PartId == 1 && d.Quantity == 2)), Times.Once);
             _serviceTicketDetailRepoMock.Verify(x => x.InsertAsync(It.Is<ServiceTicketDetail>(d => d.GarageServiceId == 2 && d.Quantity == 1)), Times.Once);
        }

        [Test]
        public async Task CreateAsync_Success_WithAssignment()
        {
            var request = new ServiceTicketCreateDto 
            { 
                 CreatedBy = 1, CustomerId = 1, VehicleId = 1, AssignedToTechnical = 5, AssignDescription = "Fix it"
            };
             
            _userRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(new User());
            _customerRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(new Customer { CustomerId = 1 });
            _vehicleRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(new Vehicle { VehicleId = 1, CustomerId = 1 });
            _serviceTicketRepoMock.Setup(x => x.InsertAsync(It.IsAny<ServiceTicket>())).ReturnsAsync(100);
            
            _userRepoMock.Setup(x => x.GetById(5)).ReturnsAsync(new User { UserId = 5 }); // Technical
            _technicalTaskRepoMock.Setup(x => x.InsertAsync(It.IsAny<TechnicalTask>())).ReturnsAsync(1);
            _serviceTicketRepoMock.Setup(x => x.GetById(100)).ReturnsAsync(new ServiceTicket { ServiceTicketId = 100 });
            _serviceTicketRepoMock.Setup(x => x.UpdateAsync(100, It.IsAny<ServiceTicket>())).ReturnsAsync(1);

            var result = await _service.CreateAsync(request);
             
            result.Should().Be(100);
            _technicalTaskRepoMock.Verify(x => x.InsertAsync(It.Is<TechnicalTask>(t => t.AssignedToTechnical == 5 && t.Description == "Fix it")), Times.Once);
        }
    }
}
