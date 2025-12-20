using FluentAssertions;
using Moq;
using NUnit.Framework;
using SWP.Core.Constants.ServiceTicketStatus;
using SWP.Core.Entities;
using SWP.Core.Exceptions;
using SWP.Core.Interfaces.Repositories;

namespace SWP.Core.UnitTest.ServiceTicketService.CustomerOperation
{
    [TestFixture]
    public class ChangeToCustomerConfirmationAsyncTest
    {
        private Mock<IServiceTicketRepo> _serviceTicketRepoMock;
        private SWP.Core.Services.ServiceTicketService _service;
        
        // Unused
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
        public async Task ChangeToCustomerConfirmationAsync_ShouldUpdateStatus()
        {
            // Arrange
            var id = 1;
            var ticket = new ServiceTicket { ServiceTicketId = id, ServiceTicketStatus = ServiceTicketStatus.InProgress };

            _serviceTicketRepoMock.Setup(x => x.GetById(id)).ReturnsAsync(ticket);
            _serviceTicketRepoMock.Setup(x => x.UpdateAsync(id, It.IsAny<ServiceTicket>())).ReturnsAsync(1);

            // Act
            var result = await _service.ChangeToCustomerConfirmationAsync(id);

            // Assert
            result.Should().Be(1);
            _serviceTicketRepoMock.Verify(x => x.UpdateAsync(id, It.Is<ServiceTicket>(st => st.ServiceTicketStatus == ServiceTicketStatus.ConfirmByCustomer)), Times.Once);
        }

        [Test]
        public void ChangeToCustomerConfirmationAsync_WhenNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var id = 999;
            _serviceTicketRepoMock.Setup(x => x.GetById(id)).ReturnsAsync((ServiceTicket)null!);

            // Act
            Func<Task> act = async () => await _service.ChangeToCustomerConfirmationAsync(id);

            // Assert
            act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Không tìm thấy service ticket.");
        }
    }
}
