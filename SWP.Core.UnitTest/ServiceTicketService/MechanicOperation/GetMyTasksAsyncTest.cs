using FluentAssertions;
using Moq;
using NUnit.Framework;
using SWP.Core.Dtos;
using SWP.Core.Dtos.SeriveTicketDto;
using SWP.Core.Entities;
using SWP.Core.Exceptions;
using SWP.Core.Interfaces.Repositories;

namespace SWP.Core.UnitTest.ServiceTicketService.MechanicOperation
{
    [TestFixture]
    public class GetMyTasksAsyncTest
    {
        private Mock<IServiceTicketRepo> _serviceTicketRepoMock;
        private SWP.Core.Services.ServiceTicketService _service;
        
        // Unused mocks for constructor
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
        public async Task GetMyTasksAsync_ShouldReturnTasks_WhenFound()
        {
            // Arrange
            var mechanicId = 1;
            var filter = new ServiceTicketFilterDtoRequest { Page = 1, PageSize = 10 };
            var tasks = new List<MechanicTaskDto>
            {
                new MechanicTaskDto { TechnicalTaskId = 1, ServiceTicketCode = "ST202517127686" },
                new MechanicTaskDto { TechnicalTaskId = 2, ServiceTicketCode = "ST202517127682" }
            };
            var pagedResult = new PagedResult<MechanicTaskDto>
            {
                Items = tasks,
                Total = 2,
                Page = 1,
                PageSize = 10
            };

            _serviceTicketRepoMock
                .Setup(x => x.GetMechanicTasksAsync(mechanicId, filter))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _service.GetMyTasksAsync(mechanicId, filter);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
            result.Items.Should().BeEquivalentTo(tasks);
            _serviceTicketRepoMock.Verify(x => x.GetMechanicTasksAsync(mechanicId, filter), Times.Once);
        }

        [Test]
        public void GetMyTasksAsync_ShouldThrowNotFoundException_WhenResultIsNull()
        {
            // Arrange
            var mechanicId = 1;
            var filter = new ServiceTicketFilterDtoRequest();
            
            _serviceTicketRepoMock
                .Setup(x => x.GetMechanicTasksAsync(mechanicId, filter))
                .ReturnsAsync((PagedResult<MechanicTaskDto>)null!);

            // Act
            Func<Task> act = async () => await _service.GetMyTasksAsync(mechanicId, filter);

            // Assert
            act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Không tìm thấy danh sách tasks.");
        }
    }
}
