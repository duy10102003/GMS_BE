using FluentAssertions;
using Moq;
using NUnit.Framework;
using SWP.Core.Dtos.SeriveTicketDto;
using SWP.Core.Dtos.CustomerDto;
using SWP.Core.Dtos.VehicleDto;
using SWP.Core.Entities;
using SWP.Core.Exceptions;
using SWP.Core.Interfaces.Repositories;

namespace SWP.Core.UnitTest.ServiceTicketService.StaffOperation
{
    [TestFixture]
    public class UpdateAsyncTest
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
        public async Task UpdateAsync_BasicInfo_ShouldUpdateSuccessfully()
        {
            // Arrange
            var id = 1;
            var request = new ServiceTicketUpdateDto
            {
                ModifiedBy = 1,
                InitialIssue = "Updated Issue",
                ServiceTicketCode = "ST001-UPDATED"
            };

            var existingTicket = new ServiceTicket { ServiceTicketId = id, VehicleId = 1 };
            
            _serviceTicketRepoMock.Setup(x => x.GetById(id)).ReturnsAsync(existingTicket);
            _userRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(new User { UserId = 1 });
            _serviceTicketRepoMock.Setup(x => x.CheckCodeExistsAsync(It.IsAny<string>(), id)).ReturnsAsync(false);
            _serviceTicketRepoMock.Setup(x => x.UpdateAsync(id, It.IsAny<ServiceTicket>())).ReturnsAsync(1);

            // Act
            var result = await _service.UpdateAsync(id, request);

            // Assert
            result.Should().Be(1);
            _serviceTicketRepoMock.Verify(x => x.UpdateAsync(id, It.IsAny<ServiceTicket>()), Times.Once);
        }

        [Test]
        public void UpdateAsync_WhenNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var id = 999;
            var request = new ServiceTicketUpdateDto { ModifiedBy = 1 };

            _serviceTicketRepoMock.Setup(x => x.GetById(id)).ReturnsAsync((ServiceTicket)null!);

            // Act
            Func<Task> act = async () => await _service.UpdateAsync(id, request);

            // Assert
            act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Không tìm thấy service ticket cần cập nhật.");
        }

        [Test]
        public async Task UpdateAsync_CustomerInfo_ShouldUpdateCustomer()
        {
            // Arrange
            var id = 1;
            var request = new ServiceTicketUpdateDto
            {
                ModifiedBy = 1,
                CustomerInfo = new CustomerInfoDto 
                { 
                    CustomerName = "Updated Name",
                    CustomerPhone = "0999999999"
                }
            };

            var existingTicket = new ServiceTicket { ServiceTicketId = id, VehicleId = 1 };
            var existingVehicle = new Vehicle { VehicleId = 1, CustomerId = 1 };
            var existingCustomer = new Customer { CustomerId = 1 };

            _serviceTicketRepoMock.Setup(x => x.GetById(id)).ReturnsAsync(existingTicket);
            _userRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(new User { UserId = 1 });
            _vehicleRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(existingVehicle);
            _customerRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(existingCustomer);
            _customerRepoMock.Setup(x => x.UpdateAsync(1, It.IsAny<Customer>())).ReturnsAsync(1);
            _serviceTicketRepoMock.Setup(x => x.UpdateAsync(id, It.IsAny<ServiceTicket>())).ReturnsAsync(1);

            // Act
            var result = await _service.UpdateAsync(id, request);

            // Assert
            result.Should().Be(1);
            _customerRepoMock.Verify(x => x.UpdateAsync(1, It.Is<Customer>(c => c.CustomerName == "Updated Name")), Times.Once);
        }

        [Test]
        public async Task UpdateAsync_VehicleInfo_ShouldUpdateVehicle()
        {
            // Arrange
            var id = 1;
            var request = new ServiceTicketUpdateDto
            {
                ModifiedBy = 1,
                VehicleInfo = new VehicleInfoDto
                {
                    VehicleName = "Updated Car",
                    VehicleLicensePlate = "30A-99999",
                    CurrentKm = 50000
                }
            };

            var existingTicket = new ServiceTicket { ServiceTicketId = id, VehicleId = 1 };
            var existingVehicle = new Vehicle { VehicleId = 1, CustomerId = 1 };

            _serviceTicketRepoMock.Setup(x => x.GetById(id)).ReturnsAsync(existingTicket);
            _userRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(new User { UserId = 1 });
            _vehicleRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(existingVehicle);
            _vehicleRepoMock.Setup(x => x.UpdateAsync(1, It.IsAny<Vehicle>())).ReturnsAsync(1);
            _serviceTicketRepoMock.Setup(x => x.UpdateAsync(id, It.IsAny<ServiceTicket>())).ReturnsAsync(1);

            // Act
            var result = await _service.UpdateAsync(id, request);

            // Assert
            result.Should().Be(1);
            _vehicleRepoMock.Verify(x => x.UpdateAsync(1, It.Is<Vehicle>(v => v.VehicleName == "Updated Car")), Times.Once);
        }

        [Test]
        public async Task UpdateAsync_Parts_ShouldHandlingRollbackAndDeduct()
        {
            // Arrange
            var id = 1;
            var request = new ServiceTicketUpdateDto
            {
                ModifiedBy = 1,
                Parts = new List<ServiceTicketAddPartDto>
                {
                    new ServiceTicketAddPartDto { PartId = 2, Quantity = 2 } // New part
                }
            };

            var existingTicket = new ServiceTicket { ServiceTicketId = id, VehicleId = 1 };
            
            // Old parts in DB
            var oldDetails = new List<ServiceTicketDetail>
            {
                new ServiceTicketDetail { ServiceTicketDetailId = 10, PartId = 1, Quantity = 5 } 
            };
            
            // Parts entities
            var oldPartEntity = new Part { PartId = 1, PartQuantity = 10 };
            var newPartEntity = new Part { PartId = 2, PartQuantity = 20 };

            _serviceTicketRepoMock.Setup(x => x.GetById(id)).ReturnsAsync(existingTicket);
            _userRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(new User { UserId = 1 });
            
            // Mock part processing logic
            _serviceTicketRepoMock.Setup(x => x.GetServiceTicketDetailsAsync(id)).ReturnsAsync(oldDetails);
            
            // Rollback old part
            _partRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(oldPartEntity);
            _partRepoMock.Setup(x => x.UpdateAsync(1, It.IsAny<Part>())).ReturnsAsync(1);
            
            // Delete old detail
            _serviceTicketDetailRepoMock.Setup(x => x.DeleteAsync(10)).ReturnsAsync(1);
            
            // Handle new part
            _partRepoMock.Setup(x => x.GetById(2)).ReturnsAsync(newPartEntity);
            _partRepoMock.Setup(x => x.UpdateAsync(2, It.IsAny<Part>())).ReturnsAsync(1);
            _serviceTicketDetailRepoMock.Setup(x => x.InsertAsync(It.IsAny<ServiceTicketDetail>())).ReturnsAsync(1);
            
            _serviceTicketRepoMock.Setup(x => x.UpdateAsync(id, It.IsAny<ServiceTicket>())).ReturnsAsync(1);

            // Act
            var result = await _service.UpdateAsync(id, request);

            // Assert
            result.Should().Be(1);
            
            // Verify Rollback: Old part quantity 10 + 5 = 15
            _partRepoMock.Verify(x => x.UpdateAsync(1, It.Is<Part>(p => p.PartQuantity == 15)), Times.Once);
            
            // Verify Delete Old Detail
            _serviceTicketDetailRepoMock.Verify(x => x.DeleteAsync(10), Times.Once);
            
            // Verify Deduct: New part quantity 20 - 2 = 18
            _partRepoMock.Verify(x => x.UpdateAsync(2, It.Is<Part>(p => p.PartQuantity == 18)), Times.Once);
            
            // Verify Insert New Detail
            _serviceTicketDetailRepoMock.Verify(x => x.InsertAsync(It.Is<ServiceTicketDetail>(d => d.PartId == 2 && d.Quantity == 2)), Times.Once);
        }

        [Test]
        public async Task UpdateAsync_GarageServices_ShouldReplaceOldOnes()
        {
            // Arrange
            var id = 1;
            var request = new ServiceTicketUpdateDto
            {
                ModifiedBy = 1,
                GarageServiceIds = new List<int> { 2 } // New garage service
            };

            var existingTicket = new ServiceTicket { ServiceTicketId = id, VehicleId = 1 };
            
            // Old garage services
            var oldDetails = new List<ServiceTicketDetail>
            {
                new ServiceTicketDetail { ServiceTicketDetailId = 10, GarageServiceId = 1, Quantity = 1 } 
            };
            
            var newGarageService = new GarageService { GarageServiceId = 2 };

            _serviceTicketRepoMock.Setup(x => x.GetById(id)).ReturnsAsync(existingTicket);
            _userRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(new User { UserId = 1 });
            
            _serviceTicketRepoMock.Setup(x => x.GetServiceTicketDetailsAsync(id)).ReturnsAsync(oldDetails);
            _serviceTicketDetailRepoMock.Setup(x => x.DeleteAsync(10)).ReturnsAsync(1);
            
            _garageServiceRepoMock.Setup(x => x.GetById(2)).ReturnsAsync(newGarageService);
            _serviceTicketDetailRepoMock.Setup(x => x.InsertAsync(It.IsAny<ServiceTicketDetail>())).ReturnsAsync(1);
            
            _serviceTicketRepoMock.Setup(x => x.UpdateAsync(id, It.IsAny<ServiceTicket>())).ReturnsAsync(1);

            // Act
            var result = await _service.UpdateAsync(id, request);

            // Assert
            result.Should().Be(1);
            _serviceTicketDetailRepoMock.Verify(x => x.DeleteAsync(10), Times.Once);
            _serviceTicketDetailRepoMock.Verify(x => x.InsertAsync(It.Is<ServiceTicketDetail>(d => d.GarageServiceId == 2)), Times.Once);
        }
    }
}
