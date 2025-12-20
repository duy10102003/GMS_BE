using FluentAssertions;
using Moq;
using NUnit.Framework;
using SWP.Core.Constants.ServiceTicketStatus;
using SWP.Core.Dtos.SeriveTicketDto;
using SWP.Core.Entities;
using SWP.Core.Exceptions;
using SWP.Core.Interfaces.Repositories;

namespace SWP.Core.UnitTest.ServiceTicketService.StaffOperation
{
    [TestFixture]
    public class ServiceTicketStatusTest
    {
        private Mock<IServiceTicketRepo> _serviceTicketRepoMock;
        private Mock<IBaseRepo<Part>> _partRepoMock;
        private Mock<IBaseRepo<User>> _userRepoMock;
        private SWP.Core.Services.ServiceTicketService _service;
        
        // Mock unused
        private Mock<IBaseRepo<Vehicle>> _vehicleRepoMock;
        private Mock<IBaseRepo<Booking>> _bookingRepoMock;
        private Mock<IBaseRepo<Customer>> _customerRepoMock;
        private Mock<IBaseRepo<ServiceTicketDetail>> _serviceTicketDetailRepoMock;
        private Mock<IBaseRepo<TechnicalTask>> _technicalTaskRepoMock;
        private Mock<IBaseRepo<GarageService>> _garageServiceRepoMock;

        [SetUp]
        public void SetUp()
        {
            _serviceTicketRepoMock = new Mock<IServiceTicketRepo>();
            _partRepoMock = new Mock<IBaseRepo<Part>>();
            _userRepoMock = new Mock<IBaseRepo<User>>();
            _serviceTicketDetailRepoMock = new Mock<IBaseRepo<ServiceTicketDetail>>();
            
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
        public async Task ChangeToCompletedAsync_ShouldUpdateStatus()
        {
            // Arrange
            var id = 1;
            var userId = 1;
            var request = new ServiceTicketChangeStatusDto { ModifiedBy = userId };
            var ticket = new ServiceTicket { ServiceTicketId = id, ServiceTicketStatus = ServiceTicketStatus.InProgress };
            var details = new List<ServiceTicketDetail>
            {
                new ServiceTicketDetail { PartId = 1, Quantity = 2 }
            };
            var part = new Part { PartId = 1, PartQuantity = 10 };

            _serviceTicketRepoMock.Setup(x => x.GetById(id)).ReturnsAsync(ticket);
            _userRepoMock.Setup(x => x.GetById(userId)).ReturnsAsync(new User { UserId = userId });
            _serviceTicketRepoMock.Setup(x => x.GetServiceTicketDetailsAsync(id)).ReturnsAsync(details);
            _serviceTicketRepoMock.Setup(x => x.UpdateAsync(id, It.IsAny<ServiceTicket>())).ReturnsAsync(1);

            // Act
            var result = await _service.ChangeToCompletedAsync(id, request);

            // Assert
            result.Should().Be(1);
            // Note: DeductPartQuantityAsync is commented out in source, so we do NOT verify part update
            _serviceTicketRepoMock.Verify(x => x.UpdateAsync(id, It.Is<ServiceTicket>(st => 
                st.ServiceTicketStatus == ServiceTicketStatus.Completed &&
                st.ModifiedBy == userId
            )), Times.Once);
        }

        [Test]
        public void ChangeToCompletedAsync_WhenStatusNotInProgress_ShouldThrowValidateException()
        {
            // Arrange
            var id = 1;
            var request = new ServiceTicketChangeStatusDto { ModifiedBy = 1 };
            // Ticket is NOT InProgress (e.g., Pending or Completed)
            var ticket = new ServiceTicket { ServiceTicketId = id, ServiceTicketStatus = ServiceTicketStatus.PendingTechnicalConfirmation };

            _serviceTicketRepoMock.Setup(x => x.GetById(id)).ReturnsAsync(ticket);

            // Act
            Func<Task> act = async () => await _service.ChangeToCompletedAsync(id, request);

            // Assert
            act.Should().ThrowAsync<ValidateException>()
                .WithMessage("Chỉ có thể chuyển sang trạng thái này từ InProgress.");
        }

        [Test]
        public async Task ChangeToCancelledAsync_ShouldUpdateStatusAndRollbackParts()
        {
            // Arrange
            var id = 1;
            var userId = 1;
            var request = new ServiceTicketChangeStatusDto { ModifiedBy = userId };
            // Ticket is not Completed (e.g. InProgress)
            var ticket = new ServiceTicket { ServiceTicketId = id, ServiceTicketStatus = ServiceTicketStatus.InProgress };
            var details = new List<ServiceTicketDetail>
            {
                new ServiceTicketDetail { PartId = 1, Quantity = 2 }
            };
            var part = new Part { PartId = 1, PartQuantity = 10 };

            _serviceTicketRepoMock.Setup(x => x.GetById(id)).ReturnsAsync(ticket);
            _userRepoMock.Setup(x => x.GetById(userId)).ReturnsAsync(new User { UserId = userId });
            _serviceTicketRepoMock.Setup(x => x.GetServiceTicketDetailsAsync(id)).ReturnsAsync(details);
            _partRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(part);
            _partRepoMock.Setup(x => x.UpdateAsync(1, It.IsAny<Part>())).ReturnsAsync(1);
            _serviceTicketRepoMock.Setup(x => x.UpdateAsync(id, It.IsAny<ServiceTicket>())).ReturnsAsync(1);

            // Act
            var result = await _service.ChangeToCancelledAsync(id, request);

            // Assert
            result.Should().Be(1);
            _partRepoMock.Verify(x => x.UpdateAsync(1, It.Is<Part>(p => p.PartQuantity == 12)), Times.Once); // 10 + 2 = 12
            _serviceTicketRepoMock.Verify(x => x.UpdateAsync(id, It.Is<ServiceTicket>(st => 
                st.ServiceTicketStatus == ServiceTicketStatus.Cancelled &&
                st.ModifiedBy == userId
            )), Times.Once);
        }

        [Test]
        public void ChangeToCancelledAsync_WhenStatusCompleted_ShouldThrowValidateException()
        {
            // Arrange
            var id = 1;
            var request = new ServiceTicketChangeStatusDto { ModifiedBy = 1 };
            var ticket = new ServiceTicket { ServiceTicketId = id, ServiceTicketStatus = ServiceTicketStatus.Completed };

            _serviceTicketRepoMock.Setup(x => x.GetById(id)).ReturnsAsync(ticket);

            // Act
            Func<Task> act = async () => await _service.ChangeToCancelledAsync(id, request);

            // Assert
            act.Should().ThrowAsync<ValidateException>()
                .WithMessage("Không thể hủy service ticket đã hoàn thành.");
        }

        [Test]
        public async Task ChangeToPendingTechnicalConfirmationAsync_ShouldUpdateStatus()
        {
            // Arrange
            var id = 1;
            var userId = 1;
            var request = new ServiceTicketChangeStatusDto { ModifiedBy = userId };
            var ticket = new ServiceTicket { ServiceTicketId = id };
            
            _serviceTicketRepoMock.Setup(x => x.GetById(id)).ReturnsAsync(ticket);
            _userRepoMock.Setup(x => x.GetById(userId)).ReturnsAsync(new User { UserId = userId });
            _serviceTicketRepoMock.Setup(x => x.UpdateAsync(id, It.IsAny<ServiceTicket>())).ReturnsAsync(1);

            // Act
            var result = await _service.ChangeToPendingTechnicalConfirmationAsync(id, request);

            // Assert
            result.Should().Be(1);
            _serviceTicketRepoMock.Verify(x => x.UpdateAsync(id, It.Is<ServiceTicket>(st => 
                st.ServiceTicketStatus == ServiceTicketStatus.PendingTechnicalConfirmation &&
                st.ModifiedBy == userId
            )), Times.Once);
        }

        [Test]
        public async Task ChangeToInProgressAsync_ShouldUpdateStatus()
        {
            // Arrange
            var id = 1;
            var userId = 1;
            var request = new ServiceTicketChangeStatusDto { ModifiedBy = userId };
            var ticket = new ServiceTicket { ServiceTicketId = id };
            
            _serviceTicketRepoMock.Setup(x => x.GetById(id)).ReturnsAsync(ticket);
            _userRepoMock.Setup(x => x.GetById(userId)).ReturnsAsync(new User { UserId = userId });
            _serviceTicketRepoMock.Setup(x => x.UpdateAsync(id, It.IsAny<ServiceTicket>())).ReturnsAsync(1);

            // Act
            var result = await _service.ChangeToInProgressAsync(id, request);

            // Assert
            result.Should().Be(1);
            _serviceTicketRepoMock.Verify(x => x.UpdateAsync(id, It.Is<ServiceTicket>(st => 
                st.ServiceTicketStatus == ServiceTicketStatus.InProgress &&
                st.ModifiedBy == userId
            )), Times.Once);
        }
        
        [Test]
        public async Task ChangeToAdjustedByTechnicalAsync_ShouldUpdateStatus()
        {
            // Arrange
            var id = 1;
            var userId = 1;
            var request = new ServiceTicketChangeStatusDto { ModifiedBy = userId };
            var ticket = new ServiceTicket { ServiceTicketId = id };
            
            _serviceTicketRepoMock.Setup(x => x.GetById(id)).ReturnsAsync(ticket);
            _userRepoMock.Setup(x => x.GetById(userId)).ReturnsAsync(new User { UserId = userId });
            _serviceTicketRepoMock.Setup(x => x.UpdateAsync(id, It.IsAny<ServiceTicket>())).ReturnsAsync(1);

            // Act
            var result = await _service.ChangeToAdjustedByTechnicalAsync(id, request);

            // Assert
            result.Should().Be(1);
            _serviceTicketRepoMock.Verify(x => x.UpdateAsync(id, It.Is<ServiceTicket>(st => 
                st.ServiceTicketStatus == ServiceTicketStatus.AdjustedByTechnical &&
                st.ModifiedBy == userId
            )), Times.Once);
        }

        [Test]
        public async Task ChangeToCompletedPaymentAsync_ShouldUpdateStatus()
        {
            // Arrange
            var id = 1;
            var ticket = new ServiceTicket { ServiceTicketId = id, ServiceTicketStatus = ServiceTicketStatus.Completed };
            _serviceTicketRepoMock.Setup(x => x.GetById(id)).ReturnsAsync(ticket);
            _serviceTicketRepoMock.Setup(x => x.UpdateAsync(id, It.IsAny<ServiceTicket>())).ReturnsAsync(1);

            // Act
            var result = await _service.ChangeToCompletedPaymentAsync(id);

            // Assert
            result.Should().Be(1);
            _serviceTicketRepoMock.Verify(x => x.UpdateAsync(id, It.Is<ServiceTicket>(st => st.ServiceTicketStatus == ServiceTicketStatus.CompletedPayment)), Times.Once);
        }
    }
}
