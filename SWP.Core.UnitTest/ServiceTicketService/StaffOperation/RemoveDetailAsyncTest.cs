using FluentAssertions;
using Moq;
using NUnit.Framework;
using SWP.Core.Constants.ServiceTicketStatus;
using SWP.Core.Entities;
using SWP.Core.Exceptions;
using SWP.Core.Interfaces.Repositories;

namespace SWP.Core.UnitTest.ServiceTicketService.StaffOperation
{
    [TestFixture]
    public class RemoveDetailAsyncTest
    {
        private Mock<IServiceTicketRepo> _serviceTicketRepoMock;
        private Mock<IBaseRepo<ServiceTicketDetail>> _serviceTicketDetailRepoMock;
        private Mock<IBaseRepo<Part>> _partRepoMock;
        private SWP.Core.Services.ServiceTicketService _service;
        
        // Mock unused
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
            _serviceTicketDetailRepoMock = new Mock<IBaseRepo<ServiceTicketDetail>>();
            _partRepoMock = new Mock<IBaseRepo<Part>>();
            
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
        public async Task RemoveDetailAsync_ValidRequest_ShouldDeleteSuccessfully()
        {
            // Arrange
            var ticketId = 1;
            var detailId = 1;
            
            var ticket = new ServiceTicket 
            { 
                ServiceTicketId = ticketId, 
                ServiceTicketStatus = ServiceTicketStatus.InProgress 
            };
            
            var detail = new ServiceTicketDetail 
            { 
                ServiceTicketDetailId = detailId, 
                ServiceTicketId = ticketId // Matches ticket
            };

            _serviceTicketRepoMock.Setup(x => x.GetById(ticketId)).ReturnsAsync(ticket);
            _serviceTicketDetailRepoMock.Setup(x => x.GetById(detailId)).ReturnsAsync(detail);
            _serviceTicketDetailRepoMock.Setup(x => x.DeleteAsync(detailId)).ReturnsAsync(1);

            // Act
            var result = await _service.RemoveDetailAsync(ticketId, detailId);

            // Assert
            result.Should().Be(1);
            _serviceTicketDetailRepoMock.Verify(x => x.DeleteAsync(detailId), Times.Once);
        }

        [Test]
        public void RemoveDetailAsync_WhenTicketNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var ticketId = 999;
            var detailId = 1;

            _serviceTicketRepoMock.Setup(x => x.GetById(ticketId)).ReturnsAsync((ServiceTicket)null!);

            // Act
            Func<Task> act = async () => await _service.RemoveDetailAsync(ticketId, detailId);

            // Assert
            act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Không tìm thấy service ticket.");
        }

        [Test]
        public void RemoveDetailAsync_WhenDetailNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var ticketId = 1;
            var detailId = 999;
            var ticket = new ServiceTicket { ServiceTicketId = ticketId };

            _serviceTicketRepoMock.Setup(x => x.GetById(ticketId)).ReturnsAsync(ticket);
            _serviceTicketDetailRepoMock.Setup(x => x.GetById(detailId)).ReturnsAsync((ServiceTicketDetail)null!);

            // Act
            Func<Task> act = async () => await _service.RemoveDetailAsync(ticketId, detailId);

            // Assert
            act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Không tìm thấy service ticket detail.");
        }

        [Test]
        public void RemoveDetailAsync_WhenDetailNotBelongToTicket_ShouldThrowValidateException()
        {
            // Arrange
            var ticketId = 1;
            var detailId = 1;
            
            var ticket = new ServiceTicket { ServiceTicketId = ticketId };
            var detail = new ServiceTicketDetail 
            { 
                ServiceTicketDetailId = detailId,
                ServiceTicketId = 2 // Different ticket ID
            };

            _serviceTicketRepoMock.Setup(x => x.GetById(ticketId)).ReturnsAsync(ticket);
            _serviceTicketDetailRepoMock.Setup(x => x.GetById(detailId)).ReturnsAsync(detail);

            // Act
            Func<Task> act = async () => await _service.RemoveDetailAsync(ticketId, detailId);

            // Assert
            act.Should().ThrowAsync<ValidateException>()
                .WithMessage("Service ticket detail không thuộc service ticket này.");
        }

        [Test]
        public void RemoveDetailAsync_WhenStatusIsCompleted_ShouldThrowValidateException()
        {
            // Arrange
            var ticketId = 1;
            var detailId = 1;
            
            var ticket = new ServiceTicket 
            { 
                ServiceTicketId = ticketId, 
                ServiceTicketStatus = ServiceTicketStatus.Completed // Completed Status
            };
            
            var detail = new ServiceTicketDetail 
            { 
                ServiceTicketDetailId = detailId, 
                ServiceTicketId = ticketId
            };

            _serviceTicketRepoMock.Setup(x => x.GetById(ticketId)).ReturnsAsync(ticket);
            _serviceTicketDetailRepoMock.Setup(x => x.GetById(detailId)).ReturnsAsync(detail);

            // Act
            Func<Task> act = async () => await _service.RemoveDetailAsync(ticketId, detailId);

            // Assert
            act.Should().ThrowAsync<ValidateException>()
                .WithMessage("Không thể xóa khi service ticket đã hoàn thành.");
        }
    }
}
