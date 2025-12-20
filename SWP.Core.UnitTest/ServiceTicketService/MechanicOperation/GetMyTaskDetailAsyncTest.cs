using FluentAssertions;
using Moq;
using NUnit.Framework;
using SWP.Core.Dtos.SeriveTicketDto;
using SWP.Core.Entities;
using SWP.Core.Exceptions;
using SWP.Core.Interfaces.Repositories;

namespace SWP.Core.UnitTest.ServiceTicketService.MechanicOperation
{
    [TestFixture]
    public class GetMyTaskDetailAsyncTest
    {
        private Mock<IServiceTicketRepo> _serviceTicketRepoMock;
        private SWP.Core.Services.ServiceTicketService _service;
        
        // Unused mocks
        private Mock<IBaseRepo<User>> _userRepoMock;
        private Mock<IBaseRepo<Vehicle>> _vehicleRepoMock;
        private Mock<IBaseRepo<Part>> _partRepoMock;
        private Mock<IBaseRepo<Booking>> _bookingRepoMock;
        private Mock<IBaseRepo<Customer>> _customerRepoMock;
        private Mock<IBaseRepo<ServiceTicketDetail>> _serviceTicketDetailRepoMock;
        private Mock<IBaseRepo<TechnicalTask>> _technicalTaskRepoMock;
        private Mock<IBaseRepo<GarageService>> _garageServiceRepoMock;

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
        public async Task GetMyTaskDetailAsync_ShouldReturnTask_WhenFound()
        {
            // Arrange
            var taskId = 1;
            var mechanicId = 1;
            var expectedTask = new MechanicTaskDto 
            { 
                TechnicalTaskId = taskId, 
                ServiceTicketCode = "ST202517127686"
            };

            _serviceTicketRepoMock
                .Setup(x => x.GetMechanicTaskDetailAsync(taskId, mechanicId))
                .ReturnsAsync(expectedTask);

            // Act
            var result = await _service.GetMyTaskDetailAsync(taskId, mechanicId);

            // Assert
            result.Should().NotBeNull();
            result.TechnicalTaskId.Should().Be(taskId);
            result.ServiceTicketCode.Should().Be("ST202517127686");
            _serviceTicketRepoMock.Verify(x => x.GetMechanicTaskDetailAsync(taskId, mechanicId), Times.Once);
        }

        [Test]
        public void GetMyTaskDetailAsync_ShouldThrowNotFoundException_WhenResultIsNull()
        {
            // Arrange
            var taskId = 999;
            var mechanicId = 1;
            
            _serviceTicketRepoMock
                .Setup(x => x.GetMechanicTaskDetailAsync(taskId, mechanicId))
                .ReturnsAsync((MechanicTaskDto)null!);

            // Act
            Func<Task> act = async () => await _service.GetMyTaskDetailAsync(taskId, mechanicId);

            // Assert
            act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Không tìm thấy task.");
        }
    }
}
