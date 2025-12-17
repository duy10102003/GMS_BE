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
    public class DeleteAsyncTest
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
        public async Task DeleteAsync_WithPendingStatus_ShouldDeleteSuccessfully()
        {
            // Arrange
            var serviceTicketId = 1;
            var existingServiceTicket = new SWP.Core.Entities.ServiceTicket
            {
                ServiceTicketId = serviceTicketId,
                ServiceTicketStatus = 0, // PendingTechnicalConfirmation
                VehicleId = 1
            };

            _serviceTicketRepoMock.Setup(x => x.GetById(serviceTicketId)).ReturnsAsync(existingServiceTicket);
            _serviceTicketRepoMock.Setup(x => x.GetServiceTicketDetailsAsync(serviceTicketId))
                .ReturnsAsync(new List<ServiceTicketDetail>());
            _serviceTicketRepoMock.Setup(x => x.DeleteAsync(serviceTicketId)).ReturnsAsync(1);

            // Act
            var result = await _service.DeleteAsync(serviceTicketId);

            // Assert
            result.Should().Be(1);
            _serviceTicketRepoMock.Verify(x => x.DeleteAsync(serviceTicketId), Times.Once);
        }

        [Test]
        public void DeleteAsync_WhenServiceTicketNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var serviceTicketId = 999;

            _serviceTicketRepoMock.Setup(x => x.GetById(serviceTicketId))
                .ReturnsAsync((SWP.Core.Entities.ServiceTicket)null!);

            // Act
            Func<Task> act = async () => await _service.DeleteAsync(serviceTicketId);

            // Assert
            act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Không tìm thấy service ticket.");
        }

        [Test]
        public void DeleteAsync_WhenStatusIsCompleted_ShouldThrowValidateException()
        {
            // Arrange
            var serviceTicketId = 1;
            var existingServiceTicket = new SWP.Core.Entities.ServiceTicket
            {
                ServiceTicketId = serviceTicketId,
                ServiceTicketStatus = 3 // Completed
            };

            _serviceTicketRepoMock.Setup(x => x.GetById(serviceTicketId)).ReturnsAsync(existingServiceTicket);

            // Act
            Func<Task> act = async () => await _service.DeleteAsync(serviceTicketId);

            // Assert
            act.Should().ThrowAsync<ValidateException>()
                .WithMessage("Không thể xóa service ticket đã hoàn thành.");
        }

        [Test]
        public async Task DeleteAsync_WithParts_ShouldRollbackPartQuantity()
        {
            // Arrange
            var serviceTicketId = 1;
            var existingServiceTicket = new SWP.Core.Entities.ServiceTicket
            {
                ServiceTicketId = serviceTicketId,
                ServiceTicketStatus = 0,
                VehicleId = 1
            };

            var details = new List<ServiceTicketDetail>
            {
                new ServiceTicketDetail { ServiceTicketDetailId = 1, PartId = 1, Quantity = 5 }
            };

            var part = new Part { PartId = 1, PartQuantity = 10 };

            _serviceTicketRepoMock.Setup(x => x.GetById(serviceTicketId)).ReturnsAsync(existingServiceTicket);
            _serviceTicketRepoMock.Setup(x => x.GetServiceTicketDetailsAsync(serviceTicketId))
                .ReturnsAsync(details);
            _partRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(part);
            _partRepoMock.Setup(x => x.UpdateAsync(1, It.IsAny<Part>())).ReturnsAsync(1);
            _serviceTicketRepoMock.Setup(x => x.DeleteAsync(serviceTicketId)).ReturnsAsync(1);

            // Act
            var result = await _service.DeleteAsync(serviceTicketId);

            // Assert
            result.Should().Be(1);
            _partRepoMock.Verify(x => x.UpdateAsync(1, It.Is<Part>(p => p.PartQuantity == 15)), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_WithMultipleParts_ShouldRollbackAllQuantities()
        {
            // Arrange
            var serviceTicketId = 1;
            var existingServiceTicket = new SWP.Core.Entities.ServiceTicket
            {
                ServiceTicketId = serviceTicketId,
                ServiceTicketStatus = 0,
                VehicleId = 1
            };

            var details = new List<ServiceTicketDetail>
            {
                new ServiceTicketDetail { PartId = 1, Quantity = 5 },
                new ServiceTicketDetail { PartId = 2, Quantity = 3 }
            };

            var part1 = new Part { PartId = 1, PartQuantity = 10 };
            var part2 = new Part { PartId = 2, PartQuantity = 20 };

            _serviceTicketRepoMock.Setup(x => x.GetById(serviceTicketId)).ReturnsAsync(existingServiceTicket);
            _serviceTicketRepoMock.Setup(x => x.GetServiceTicketDetailsAsync(serviceTicketId))
                .ReturnsAsync(details);
            _partRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(part1);
            _partRepoMock.Setup(x => x.GetById(2)).ReturnsAsync(part2);
            _partRepoMock.Setup(x => x.UpdateAsync(It.IsAny<int>(), It.IsAny<Part>())).ReturnsAsync(1);
            _serviceTicketRepoMock.Setup(x => x.DeleteAsync(serviceTicketId)).ReturnsAsync(1);

            // Act
            var result = await _service.DeleteAsync(serviceTicketId);

            // Assert
            result.Should().Be(1);
            _partRepoMock.Verify(x => x.UpdateAsync(1, It.Is<Part>(p => p.PartQuantity == 15)), Times.Once);
            _partRepoMock.Verify(x => x.UpdateAsync(2, It.Is<Part>(p => p.PartQuantity == 23)), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_WithOnlyGarageServices_ShouldDeleteWithoutRollback()
        {
            // Arrange
            var serviceTicketId = 1;
            var existingServiceTicket = new SWP.Core.Entities.ServiceTicket
            {
                ServiceTicketId = serviceTicketId,
                ServiceTicketStatus = 0,
                VehicleId = 1
            };

            var details = new List<ServiceTicketDetail>
            {
                new ServiceTicketDetail { GarageServiceId = 1, Quantity = 1 }
            };

            _serviceTicketRepoMock.Setup(x => x.GetById(serviceTicketId)).ReturnsAsync(existingServiceTicket);
            _serviceTicketRepoMock.Setup(x => x.GetServiceTicketDetailsAsync(serviceTicketId))
                .ReturnsAsync(details);
            _serviceTicketRepoMock.Setup(x => x.DeleteAsync(serviceTicketId)).ReturnsAsync(1);

            // Act
            var result = await _service.DeleteAsync(serviceTicketId);

            // Assert
            result.Should().Be(1);
            _partRepoMock.Verify(x => x.UpdateAsync(It.IsAny<int>(), It.IsAny<Part>()), Times.Never);
        }
    }
}
