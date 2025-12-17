using FluentAssertions;
using Moq;
using NUnit.Framework;
using SWP.Core.Dtos;
using SWP.Core.Dtos.SeriveTicketDto;
using SWP.Core.Dtos.CustomerDto;
using SWP.Core.Dtos.VehicleDto;
using SWP.Core.Dtos.PartDto;
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
        private Mock<IBaseRepo<Vehicle>> _vehicleRepoMock;
        private Mock<IBaseRepo<Part>> _partRepoMock;
        private Mock<IBaseRepo<Booking>> _bookingRepoMock;
        private Mock<IBaseRepo<Customer>> _customerRepoMock;
        private Mock<IBaseRepo<ServiceTicketDetail>> _serviceTicketDetailRepoMock;
        private Mock<IBaseRepo<TechnicalTask>> _technicalTaskRepoMock;
        private Mock<IBaseRepo<GarageService>> _garageServiceRepoMock;
        private SWP.Core.Services.ServiceTicketService _service;

        [SetUp]
        public void SetUp()
        {
            _serviceTicketRepoMock = new Mock<IServiceTicketRepo>();
            _userRepoMock = new Mock<IBaseRepo<User>>();
            _vehicleRepoMock = new Mock<IBaseRepo<Vehicle>>();
            _partRepoMock = new Mock<IBaseRepo<Part>>();
            _bookingRepoMock = new Mock<IBaseRepo<Booking>>();
            _customerRepoMock = new Mock<IBaseRepo<Customer>>();
            _serviceTicketDetailRepoMock = new Mock<IBaseRepo<ServiceTicketDetail>>();
            _technicalTaskRepoMock = new Mock<IBaseRepo<TechnicalTask>>();
            _garageServiceRepoMock = new Mock<IBaseRepo<GarageService>>();

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
        public async Task CreateAsync_WithExistingCustomerAndVehicle_ShouldCreateSuccessfully()
        {
            // Arrange
            var request = new ServiceTicketCreateDto
            {
                CustomerId = 1,
                VehicleId = 1,
                CreatedBy = 1,
                InitialIssue = "Engine problem",
                BookingId = null
            };

            var existingUser = new User { UserId = 1 };
            var existingCustomer = new Customer { CustomerId = 1, CustomerPhone = "0901234567" };
            var existingVehicle = new Vehicle { VehicleId = 1, CustomerId = 1 };

            _userRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(existingUser);
            _customerRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(existingCustomer);
            _vehicleRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(existingVehicle);
            _serviceTicketRepoMock.Setup(x => x.CheckCodeExistsAsync(It.IsAny<string>(), null)).ReturnsAsync(false);
            _serviceTicketRepoMock.Setup(x => x.InsertAsync(It.IsAny<SWP.Core.Entities.ServiceTicket>())).ReturnsAsync(1);

            // Act
            var result = await _service.CreateAsync(request);

            // Assert
            result.Should().Be(1);
            _serviceTicketRepoMock.Verify(x => x.InsertAsync(It.IsAny<SWP.Core.Entities.ServiceTicket>()), Times.Once);
        }

        [Test]
        public void CreateAsync_WhenUserNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var request = new ServiceTicketCreateDto
            {
                CustomerId = 1,
                VehicleId = 1,
                CreatedBy = 999
            };

            _userRepoMock.Setup(x => x.GetById(999)).ReturnsAsync((User)null!);

            // Act
            Func<Task> act = async () => await _service.CreateAsync(request);

            // Assert
            act.Should().ThrowAsync<NotFoundException>();
        }

        [Test]
        public void CreateAsync_WhenCustomerNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var request = new ServiceTicketCreateDto
            {
                CustomerId = 999,
                VehicleId = 1,
                CreatedBy = 1
            };

            var existingUser = new User { UserId = 1 };
            _userRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(existingUser);
            _customerRepoMock.Setup(x => x.GetById(999)).ReturnsAsync((Customer)null!);

            // Act
            Func<Task> act = async () => await _service.CreateAsync(request);

            // Assert
            act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Không tìm thấy customer.");
        }

        [Test]
        public void CreateAsync_WhenVehicleNotBelongToCustomer_ShouldThrowValidateException()
        {
            // Arrange
            var request = new ServiceTicketCreateDto
            {
                CustomerId = 1,
                VehicleId = 1,
                CreatedBy = 1
            };

            var existingUser = new User { UserId = 1 };
            var existingCustomer = new Customer { CustomerId = 1 };
            var existingVehicle = new Vehicle { VehicleId = 1, CustomerId = 2 }; // Different customer

            _userRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(existingUser);
            _customerRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(existingCustomer);
            _vehicleRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(existingVehicle);

            // Act
            Func<Task> act = async () => await _service.CreateAsync(request);

            // Assert
            act.Should().ThrowAsync<ValidateException>()
                .WithMessage("Vehicle không thuộc về customer được chọn.");
        }

        [Test]
        public async Task CreateAsync_WithNewCustomer_ShouldCreateCustomerAndServiceTicket()
        {
            // Arrange
            var request = new ServiceTicketCreateDto
            {
                CustomerInfo = new CustomerInfoDto
                {
                    CustomerName = "New Customer",
                    CustomerPhone = "0901234567",
                    CustomerEmail = "test@example.com"
                },
                VehicleId = 1,
                CreatedBy = 1,
                InitialIssue = "Test"
            };

            var existingUser = new User { UserId = 1 };
            var existingVehicle = new Vehicle { VehicleId = 1, CustomerId = null };

            _userRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(existingUser);
            _vehicleRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(existingVehicle);
            _customerRepoMock.Setup(x => x.InsertAsync(It.IsAny<Customer>())).ReturnsAsync(1);
            _vehicleRepoMock.Setup(x => x.UpdateAsync(1, It.IsAny<Vehicle>())).ReturnsAsync(1);
            _serviceTicketRepoMock.Setup(x => x.CheckCodeExistsAsync(It.IsAny<string>(), null)).ReturnsAsync(false);
            _serviceTicketRepoMock.Setup(x => x.InsertAsync(It.IsAny<SWP.Core.Entities.ServiceTicket>())).ReturnsAsync(1);

            // Act
            var result = await _service.CreateAsync(request);

            // Assert
            result.Should().Be(1);
            _customerRepoMock.Verify(x => x.InsertAsync(It.IsAny<Customer>()), Times.Once);
        }

        [Test]
        public async Task CreateAsync_WithNewVehicle_ShouldCreateVehicleAndServiceTicket()
        {
            // Arrange
            var request = new ServiceTicketCreateDto
            {
                CustomerId = 1,
                VehicleInfo = new VehicleInfoDto
                {
                    VehicleName = "Honda City",
                    VehicleLicensePlate = "30A-12345",
                    Make = "Honda",
                    Model = "City",
                    CurrentKm = 10000
                },
                CreatedBy = 1,
                InitialIssue = "Test"
            };

            var existingUser = new User { UserId = 1 };
            var existingCustomer = new Customer { CustomerId = 1 };

            _userRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(existingUser);
            _customerRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(existingCustomer);
            _vehicleRepoMock.Setup(x => x.InsertAsync(It.IsAny<Vehicle>())).ReturnsAsync(1);
            _serviceTicketRepoMock.Setup(x => x.CheckCodeExistsAsync(It.IsAny<string>(), null)).ReturnsAsync(false);
            _serviceTicketRepoMock.Setup(x => x.InsertAsync(It.IsAny<SWP.Core.Entities.ServiceTicket>())).ReturnsAsync(1);

            // Act
            var result = await _service.CreateAsync(request);

            // Assert
            result.Should().Be(1);
            _vehicleRepoMock.Verify(x => x.InsertAsync(It.IsAny<Vehicle>()), Times.Once);
        }

        [Test]
        public async Task CreateAsync_WithParts_ShouldDeductPartQuantity()
        {
            // Arrange
            var request = new ServiceTicketCreateDto
            {
                CustomerId = 1,
                VehicleId = 1,
                CreatedBy = 1,
                InitialIssue = "Test",
                Parts = new List<ServiceTicketAddPartDto>
                {
                    new ServiceTicketAddPartDto { PartId = 1, Quantity = 2 }
                }
            };

            var existingUser = new User { UserId = 1 };
            var existingCustomer = new Customer { CustomerId = 1 };
            var existingVehicle = new Vehicle { VehicleId = 1, CustomerId = 1 };
            var existingPart = new Part { PartId = 1, PartQuantity = 10 };

            _userRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(existingUser);
            _customerRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(existingCustomer);
            _vehicleRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(existingVehicle);
            _partRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(existingPart);
            _partRepoMock.Setup(x => x.UpdateAsync(1, It.IsAny<Part>())).ReturnsAsync(1);
            _serviceTicketRepoMock.Setup(x => x.CheckCodeExistsAsync(It.IsAny<string>(), null)).ReturnsAsync(false);
            _serviceTicketRepoMock.Setup(x => x.InsertAsync(It.IsAny<SWP.Core.Entities.ServiceTicket>())).ReturnsAsync(1);
            _serviceTicketDetailRepoMock.Setup(x => x.InsertAsync(It.IsAny<ServiceTicketDetail>())).ReturnsAsync(1);

            // Act
            var result = await _service.CreateAsync(request);

            // Assert
            result.Should().Be(1);
            _partRepoMock.Verify(x => x.UpdateAsync(1, It.Is<Part>(p => p.PartQuantity == 8)), Times.Once);
        }

        [Test]
        public void CreateAsync_WhenPartQuantityInsufficient_ShouldThrowValidateException()
        {
            // Arrange
            var request = new ServiceTicketCreateDto
            {
                CustomerId = 1,
                VehicleId = 1,
                CreatedBy = 1,
                InitialIssue = "Test",
                Parts = new List<ServiceTicketAddPartDto>
                {
                    new ServiceTicketAddPartDto { PartId = 1, Quantity = 20 }
                }
            };

            var existingUser = new User { UserId = 1 };
            var existingCustomer = new Customer { CustomerId = 1 };
            var existingVehicle = new Vehicle { VehicleId = 1, CustomerId = 1 };
            var existingPart = new Part { PartId = 1, PartQuantity = 5 }; // Only 5 available

            _userRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(existingUser);
            _customerRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(existingCustomer);
            _vehicleRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(existingVehicle);
            _partRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(existingPart);
            _serviceTicketRepoMock.Setup(x => x.CheckCodeExistsAsync(It.IsAny<string>(), null)).ReturnsAsync(false);
            _serviceTicketRepoMock.Setup(x => x.InsertAsync(It.IsAny<SWP.Core.Entities.ServiceTicket>())).ReturnsAsync(1);

            // Act
            Func<Task> act = async () => await _service.CreateAsync(request);

            // Assert
            act.Should().ThrowAsync<ValidateException>();
        }

        [Test]
        public void CreateAsync_WithoutCustomerIdAndInfo_ShouldThrowValidateException()
        {
            // Arrange
            var request = new ServiceTicketCreateDto
            {
                VehicleId = 1,
                CreatedBy = 1
            };

            var existingUser = new User { UserId = 1 };
            _userRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(existingUser);

            // Act
            Func<Task> act = async () => await _service.CreateAsync(request);

            // Assert
            act.Should().ThrowAsync<ValidateException>()
                .WithMessage("Phải cung cấp CustomerId hoặc CustomerInfo.");
        }

        [Test]
        public void CreateAsync_WithoutVehicleIdAndInfo_ShouldThrowValidateException()
        {
            // Arrange
            var request = new ServiceTicketCreateDto
            {
                CustomerId = 1,
                CreatedBy = 1
            };

            var existingUser = new User { UserId = 1 };
            var existingCustomer = new Customer { CustomerId = 1 };

            _userRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(existingUser);
            _customerRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(existingCustomer);

            // Act
            Func<Task> act = async () => await _service.CreateAsync(request);

            // Assert
            act.Should().ThrowAsync<ValidateException>()
                .WithMessage("Phải cung cấp VehicleId hoặc VehicleInfo.");
        }
    }
}
