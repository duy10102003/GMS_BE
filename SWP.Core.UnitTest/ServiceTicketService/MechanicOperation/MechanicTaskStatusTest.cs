using FluentAssertions;
using Moq;
using NUnit.Framework;
using SWP.Core.Entities;
using SWP.Core.Exceptions;
using SWP.Core.Interfaces.Repositories;

namespace SWP.Core.UnitTest.ServiceTicketService.MechanicOperation
{
    [TestFixture]
    public class MechanicTaskStatusTest
    {
        private Mock<IServiceTicketRepo> _serviceTicketRepoMock;
        private Mock<IBaseRepo<TechnicalTask>> _technicalTaskRepoMock;
        private Mock<IBaseRepo<ServiceTicketDetail>> _serviceTicketDetailRepoMock;
        private SWP.Core.Services.ServiceTicketService _service;
        
        // Unused
        private Mock<IBaseRepo<User>> _userRepoMock;
        private Mock<IBaseRepo<Vehicle>> _vehicleRepoMock;
        private Mock<IBaseRepo<Part>> _partRepoMock;
        private Mock<IBaseRepo<Booking>> _bookingRepoMock;
        private Mock<IBaseRepo<Customer>> _customerRepoMock;
        private Mock<IBaseRepo<GarageService>> _garageServiceRepoMock;

        [SetUp]
        public void SetUp()
        {
            _serviceTicketRepoMock = new Mock<IServiceTicketRepo>();
            _technicalTaskRepoMock = new Mock<IBaseRepo<TechnicalTask>>();
            _serviceTicketDetailRepoMock = new Mock<IBaseRepo<ServiceTicketDetail>>();
            
            _userRepoMock = new Mock<IBaseRepo<User>>();
            _vehicleRepoMock = new Mock<IBaseRepo<Vehicle>>();
            _partRepoMock = new Mock<IBaseRepo<Part>>();
            _bookingRepoMock = new Mock<IBaseRepo<Booking>>();
            _customerRepoMock = new Mock<IBaseRepo<Customer>>();
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
        public async Task StartTaskAsync_ShouldUpdateStatus()
        {
            // Arrange
            var taskId = 1;
            var mechanicId = 1;
            var task = new TechnicalTask { TechnicalTaskId = taskId, TaskStatus = 0, AssignedToTechnical = mechanicId, ServiceTicketId = 1 }; 
            var ticket = new ServiceTicket { ServiceTicketId = 1 };

            _technicalTaskRepoMock.Setup(x => x.GetById(taskId)).ReturnsAsync(task);
            _technicalTaskRepoMock.Setup(x => x.UpdateAsync(taskId, It.IsAny<TechnicalTask>())).ReturnsAsync(1);
            _serviceTicketRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(ticket);
            _serviceTicketRepoMock.Setup(x => x.UpdateAsync(1, It.IsAny<ServiceTicket>())).ReturnsAsync(1);

            // Act
            var result = await _service.StartTaskAsync(taskId, mechanicId);

            // Assert
            result.Should().Be(1);
            _technicalTaskRepoMock.Verify(x => x.UpdateAsync(taskId, It.Is<TechnicalTask>(t => t.TaskStatus == 1)), Times.Once);
            _serviceTicketRepoMock.Verify(x => x.UpdateAsync(1, It.Is<ServiceTicket>(s => s.ServiceTicketStatus == SWP.Core.Constants.ServiceTicketStatus.ServiceTicketStatus.InProgress)), Times.Once);
        }

        [Test]
        public async Task ConfirmTaskAsync_ShouldUpdateStatus()
        {
             // Arrange
            var taskId = 1;
            var mechanicId = 1;
            var task = new TechnicalTask { TechnicalTaskId = taskId, TaskStatus = 1, AssignedToTechnical = mechanicId, ServiceTicketId = 1 };
            var ticket = new ServiceTicket { ServiceTicketId = 1 };
            
            var allTasks = new List<TechnicalTask> { task };

            _technicalTaskRepoMock.Setup(x => x.GetById(taskId)).ReturnsAsync(task);
            _technicalTaskRepoMock.Setup(x => x.UpdateAsync(taskId, It.IsAny<TechnicalTask>())).ReturnsAsync(1);
            _serviceTicketRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(ticket);
            _serviceTicketDetailRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<ServiceTicketDetail>());
            _technicalTaskRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(allTasks);
            _serviceTicketRepoMock.Setup(x => x.UpdateAsync(1, It.IsAny<ServiceTicket>())).ReturnsAsync(1);

            // Act
            var result = await _service.ConfirmTaskAsync(taskId, mechanicId);

            // Assert
            result.Should().Be(1);
            _technicalTaskRepoMock.Verify(x => x.UpdateAsync(taskId, It.Is<TechnicalTask>(t => t.TaskStatus == 2)), Times.Once);
             _serviceTicketRepoMock.Verify(x => x.UpdateAsync(1, It.Is<ServiceTicket>(s => s.ServiceTicketStatus == SWP.Core.Constants.ServiceTicketStatus.ServiceTicketStatus.Completed)), Times.Once);
        }
    }
}
