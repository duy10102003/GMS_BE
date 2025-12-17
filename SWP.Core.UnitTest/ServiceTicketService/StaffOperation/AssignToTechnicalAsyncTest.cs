using FluentAssertions;
using Moq;
using NUnit.Framework;
using SWP.Core.Dtos.SeriveTicketDto;
using SWP.Core.Entities;
using SWP.Core.Exceptions;
using SWP.Core.Interfaces.Repositories;

namespace SWP.Core.UnitTest.ServiceTicketService.StaffOperation
{
    [TestFixture]
    public class AssignToTechnicalAsyncTest
    {
        private Mock<IServiceTicketRepo> _serviceTicketRepoMock;
        private Mock<IBaseRepo<User>> _userRepoMock;
        private Mock<IBaseRepo<TechnicalTask>> _technicalTaskRepoMock;
        private SWP.Core.Services.ServiceTicketService _service;
        
        // Mock other dependencies as null or minimal since they aren't used in this method
        private Mock<IBaseRepo<Vehicle>> _vehicleRepoMock;
        private Mock<IBaseRepo<Part>> _partRepoMock;
        private Mock<IBaseRepo<Booking>> _bookingRepoMock;
        private Mock<IBaseRepo<Customer>> _customerRepoMock;
        private Mock<IBaseRepo<ServiceTicketDetail>> _serviceTicketDetailRepoMock;
        private Mock<IBaseRepo<GarageService>> _garageServiceRepoMock;

        [SetUp]
        public void SetUp()
        {
            _serviceTicketRepoMock = new Mock<IServiceTicketRepo>();
            _userRepoMock = new Mock<IBaseRepo<User>>();
            _technicalTaskRepoMock = new Mock<IBaseRepo<TechnicalTask>>();
            
            _vehicleRepoMock = new Mock<IBaseRepo<Vehicle>>();
            _partRepoMock = new Mock<IBaseRepo<Part>>();
            _bookingRepoMock = new Mock<IBaseRepo<Booking>>();
            _customerRepoMock = new Mock<IBaseRepo<Customer>>();
            _serviceTicketDetailRepoMock = new Mock<IBaseRepo<ServiceTicketDetail>>();
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
        public async Task AssignToTechnicalAsync_ShouldAssignSuccessfully()
        {
            // Arrange
            var id = 1;
            var request = new ServiceTicketAssignDto
            {
                AssignedToTechnical = 2,
                Description = "Task Description"
            };

            var serviceTicket = new ServiceTicket { ServiceTicketId = id };
            var staff = new User { UserId = 2 };

            _serviceTicketRepoMock.Setup(x => x.GetById(id)).ReturnsAsync(serviceTicket);
            _userRepoMock.Setup(x => x.GetById(2)).ReturnsAsync(staff);
            _technicalTaskRepoMock.Setup(x => x.InsertAsync(It.IsAny<TechnicalTask>())).ReturnsAsync(1);
            _serviceTicketRepoMock.Setup(x => x.UpdateAsync(id, It.IsAny<ServiceTicket>())).ReturnsAsync(1);

            // Act
            var result = await _service.AssignToTechnicalAsync(id, request);

            // Assert
            result.Should().Be(1);
            _technicalTaskRepoMock.Verify(x => x.InsertAsync(It.Is<TechnicalTask>(t => 
                t.ServiceTicketId == id && 
                t.AssignedToTechnical == 2 && 
                t.Description == "Task Description")), Times.Once);
            
            _serviceTicketRepoMock.Verify(x => x.UpdateAsync(id, It.Is<ServiceTicket>(st => 
                st.ServiceTicketStatus == SWP.Core.Constants.ServiceTicketStatus.ServiceTicketStatus.PendingTechnicalConfirmation)), Times.Once);
        }

        [Test]
        public void AssignToTechnicalAsync_WhenTicketNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var id = 999;
            var request = new ServiceTicketAssignDto { AssignedToTechnical = 2 };

            _serviceTicketRepoMock.Setup(x => x.GetById(id)).ReturnsAsync((ServiceTicket)null!);

            // Act
            Func<Task> act = async () => await _service.AssignToTechnicalAsync(id, request);

            // Assert
            act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Không tìm thấy service ticket.");
        }

        [Test]
        public void AssignToTechnicalAsync_WhenUserNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var id = 1;
            var request = new ServiceTicketAssignDto { AssignedToTechnical = 999 };

            var serviceTicket = new ServiceTicket { ServiceTicketId = id };
            _serviceTicketRepoMock.Setup(x => x.GetById(id)).ReturnsAsync(serviceTicket);
            _userRepoMock.Setup(x => x.GetById(999)).ReturnsAsync((User)null!);

            // Act
            Func<Task> act = async () => await _service.AssignToTechnicalAsync(id, request);

            // Assert
            act.Should().ThrowAsync<NotFoundException>();
        }
    }
}
