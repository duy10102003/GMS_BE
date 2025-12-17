using FluentAssertions;
using Moq;
using NUnit.Framework;
using SWP.Core.Dtos.PartDto;
using SWP.Core.Dtos.SeriveTicketDto;
using SWP.Core.Entities;
using SWP.Core.Exceptions;
using SWP.Core.Interfaces.Repositories;

namespace SWP.Core.UnitTest.ServiceTicketService.StaffOperation
{
    [TestFixture]
    public class AddPartAsyncTest
    {
        private Mock<IServiceTicketRepo> _serviceTicketRepoMock;
        private Mock<IBaseRepo<Part>> _partRepoMock;
        private Mock<IBaseRepo<ServiceTicketDetail>> _serviceTicketDetailRepoMock;
        private SWP.Core.Services.ServiceTicketService _service;
        
        // Mock unused dependencies
        private Mock<IBaseRepo<User>> _userRepoMock;
        private Mock<IBaseRepo<Vehicle>> _vehicleRepoMock;
        private Mock<IBaseRepo<Booking>> _bookingRepoMock;
        private Mock<IBaseRepo<Customer>> _customerRepoMock;
        private Mock<IBaseRepo<TechnicalTask>> _technicalTaskRepoMock;
        private Mock<IBaseRepo<GarageService>> _garageServiceRepoMock;

        [SetUp]
        public void SetUp()
        {
            _serviceTicketRepoMock = new Mock<IServiceTicketRepo>();
            _partRepoMock = new Mock<IBaseRepo<Part>>();
            _serviceTicketDetailRepoMock = new Mock<IBaseRepo<ServiceTicketDetail>>();
            
            _userRepoMock = new Mock<IBaseRepo<User>>();
            _vehicleRepoMock = new Mock<IBaseRepo<Vehicle>>();
            _bookingRepoMock = new Mock<IBaseRepo<Booking>>();
            _customerRepoMock = new Mock<IBaseRepo<Customer>>();
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
        public async Task AddPartAsync_ShouldAddSuccessfully()
        {
            // Arrange
            var id = 1;
            var request = new ServiceTicketAddPartDto { PartId = 1, Quantity = 2 };
            var serviceTicket = new ServiceTicket { ServiceTicketId = id };
            var part = new Part { PartId = 1, PartQuantity = 10 };

            _serviceTicketRepoMock.Setup(x => x.GetById(id)).ReturnsAsync(serviceTicket);
            _partRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(part);
            _partRepoMock.Setup(x => x.UpdateAsync(1, It.IsAny<Part>())).ReturnsAsync(1);
            _serviceTicketDetailRepoMock.Setup(x => x.InsertAsync(It.IsAny<ServiceTicketDetail>())).ReturnsAsync(1);

            // Act
            var result = await _service.AddPartAsync(id, request);

            // Assert
            result.Should().Be(1);
            // Verify deduction
            _partRepoMock.Verify(x => x.UpdateAsync(1, It.Is<Part>(p => p.PartQuantity == 8)), Times.Once);
            // Verify insert
            _serviceTicketDetailRepoMock.Verify(x => x.InsertAsync(It.Is<ServiceTicketDetail>(d => 
                d.ServiceTicketId == id && 
                d.PartId == 1 &&
                d.Quantity == 2)), Times.Once);
        }

        [Test]
        public void AddPartAsync_WhenTicketNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var id = 999;
            var request = new ServiceTicketAddPartDto { PartId = 1, Quantity = 2 };
            _serviceTicketRepoMock.Setup(x => x.GetById(id)).ReturnsAsync((ServiceTicket)null!);

            // Act
            Func<Task> act = async () => await _service.AddPartAsync(id, request);

            // Assert
            act.Should().ThrowAsync<NotFoundException>();
        }

        [Test]
        public void AddPartAsync_WhenQuantityInsufficient_ShouldThrowValidateException()
        {
            // Arrange
            var id = 1;
            var request = new ServiceTicketAddPartDto { PartId = 1, Quantity = 20 };
            var serviceTicket = new ServiceTicket { ServiceTicketId = id };
            var part = new Part { PartId = 1, PartQuantity = 5 };

            _serviceTicketRepoMock.Setup(x => x.GetById(id)).ReturnsAsync(serviceTicket);
            _partRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(part);

            // Act
            Func<Task> act = async () => await _service.AddPartAsync(id, request);

            // Assert
            act.Should().ThrowAsync<ValidateException>()
                .WithMessage("Số lượng trong kho không đủ (còn 5).");
        }
    }
}
