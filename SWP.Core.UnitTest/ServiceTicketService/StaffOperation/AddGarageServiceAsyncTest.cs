using FluentAssertions;
using Moq;
using NUnit.Framework;
using SWP.Core.Dtos.GarageServiceDto;
using SWP.Core.Dtos.SeriveTicketDto;
using SWP.Core.Entities;
using SWP.Core.Exceptions;
using SWP.Core.Interfaces.Repositories;

namespace SWP.Core.UnitTest.ServiceTicketService.StaffOperation
{
    [TestFixture]
    public class AddGarageServiceAsyncTest
    {
        private Mock<IServiceTicketRepo> _serviceTicketRepoMock;
        private Mock<IBaseRepo<GarageService>> _garageServiceRepoMock;
        private Mock<IBaseRepo<ServiceTicketDetail>> _serviceTicketDetailRepoMock;
        private SWP.Core.Services.ServiceTicketService _service;
        
        // Mock unused
        private Mock<IBaseRepo<User>> _userRepoMock;
        private Mock<IBaseRepo<Vehicle>> _vehicleRepoMock;
        private Mock<IBaseRepo<Part>> _partRepoMock;
        private Mock<IBaseRepo<Booking>> _bookingRepoMock;
        private Mock<IBaseRepo<Customer>> _customerRepoMock;
        private Mock<IBaseRepo<TechnicalTask>> _technicalTaskRepoMock;

        [SetUp]
        public void SetUp()
        {
            _serviceTicketRepoMock = new Mock<IServiceTicketRepo>();
            _garageServiceRepoMock = new Mock<IBaseRepo<GarageService>>();
            _serviceTicketDetailRepoMock = new Mock<IBaseRepo<ServiceTicketDetail>>();
            
            _userRepoMock = new Mock<IBaseRepo<User>>();
            _vehicleRepoMock = new Mock<IBaseRepo<Vehicle>>();
            _partRepoMock = new Mock<IBaseRepo<Part>>();
            _bookingRepoMock = new Mock<IBaseRepo<Booking>>();
            _customerRepoMock = new Mock<IBaseRepo<Customer>>();
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
        public async Task AddGarageServiceAsync_ShouldAddSuccessfully()
        {
            // Arrange
            var id = 1;
            var request = new ServiceTicketAddGarageServiceDto { GarageServiceId = 1 };
            var serviceTicket = new ServiceTicket { ServiceTicketId = id };
            var garageService = new GarageService { GarageServiceId = 1 };

            _serviceTicketRepoMock.Setup(x => x.GetById(id)).ReturnsAsync(serviceTicket);
            _garageServiceRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(garageService);
            _serviceTicketDetailRepoMock.Setup(x => x.InsertAsync(It.IsAny<ServiceTicketDetail>())).ReturnsAsync(1);

            // Act
            var result = await _service.AddGarageServiceAsync(id, request);

            // Assert
            result.Should().Be(1);
            _serviceTicketDetailRepoMock.Verify(x => x.InsertAsync(It.Is<ServiceTicketDetail>(d => 
                d.ServiceTicketId == id && 
                d.GarageServiceId == 1)), Times.Once);
        }

        [Test]
        public void AddGarageServiceAsync_WhenTicketNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var id = 999;
            var request = new ServiceTicketAddGarageServiceDto { GarageServiceId = 1 };
            _serviceTicketRepoMock.Setup(x => x.GetById(id)).ReturnsAsync((ServiceTicket)null!);

            // Act
            Func<Task> act = async () => await _service.AddGarageServiceAsync(id, request);

            // Assert
            act.Should().ThrowAsync<NotFoundException>();
        }

        [Test]
        public void AddGarageServiceAsync_WhenServiceNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var id = 1;
            var request = new ServiceTicketAddGarageServiceDto { GarageServiceId = 999 };
            var serviceTicket = new ServiceTicket { ServiceTicketId = id };

            _serviceTicketRepoMock.Setup(x => x.GetById(id)).ReturnsAsync(serviceTicket);
            _garageServiceRepoMock.Setup(x => x.GetById(999)).ReturnsAsync((GarageService)null!);

            // Act
            Func<Task> act = async () => await _service.AddGarageServiceAsync(id, request);

            // Assert
            act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Không tìm thấy garage service.");
        }
    }
}
