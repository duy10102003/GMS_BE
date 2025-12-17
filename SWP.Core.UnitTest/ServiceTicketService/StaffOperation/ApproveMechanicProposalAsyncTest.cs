using FluentAssertions;
using Moq;
using NUnit.Framework;
using SWP.Core.Dtos.SeriveTicketDto;
using SWP.Core.Dtos.PartDto;
using SWP.Core.Dtos.GarageServiceDto;
using SWP.Core.Entities;
using SWP.Core.Exceptions;
using SWP.Core.Interfaces.Repositories;

namespace SWP.Core.UnitTest.ServiceTicketService.StaffOperation
{
    [TestFixture]
    public class ApproveMechanicProposalAsyncTest
    {
        private Mock<IServiceTicketRepo> _serviceTicketRepoMock;
        private Mock<IBaseRepo<TechnicalTask>> _technicalTaskRepoMock;
        private Mock<IBaseRepo<User>> _userRepoMock;
        private Mock<IBaseRepo<Part>> _partRepoMock;
        private Mock<IBaseRepo<ServiceTicketDetail>> _serviceTicketDetailRepoMock;
        private Mock<IBaseRepo<GarageService>> _garageServiceRepoMock;
        private SWP.Core.Services.ServiceTicketService _service;
        
        // Mock unused
        private Mock<IBaseRepo<Vehicle>> _vehicleRepoMock;
        private Mock<IBaseRepo<Booking>> _bookingRepoMock;
        private Mock<IBaseRepo<Customer>> _customerRepoMock;

        [SetUp]
        public void SetUp()
        {
            _serviceTicketRepoMock = new Mock<IServiceTicketRepo>();
            _technicalTaskRepoMock = new Mock<IBaseRepo<TechnicalTask>>();
            _userRepoMock = new Mock<IBaseRepo<User>>();
            _partRepoMock = new Mock<IBaseRepo<Part>>();
            _serviceTicketDetailRepoMock = new Mock<IBaseRepo<ServiceTicketDetail>>();
            _garageServiceRepoMock = new Mock<IBaseRepo<GarageService>>();
            
            _vehicleRepoMock = new Mock<IBaseRepo<Vehicle>>();
            _bookingRepoMock = new Mock<IBaseRepo<Booking>>();
            _customerRepoMock = new Mock<IBaseRepo<Customer>>();

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
        public async Task ApproveMechanicProposalAsync_ShouldApproveWithNewPartsAndServices()
        {
            // Arrange
            var taskId = 1;
            var staffId = 1;
            var request = new ServiceTicketUpdatePartsServicesDto
            {
                Parts = new List<ServiceTicketPartDto>
                {
                    new ServiceTicketPartDto { PartId = 1, Quantity = 2 } // New part
                },
                GarageServices = new List<ServiceTicketServiceDto>
                {
                    new ServiceTicketServiceDto { GarageServiceId = 1, Quantity = 1 } // New service
                }
            };
            
            var task = new TechnicalTask { TechnicalTaskId = taskId, ServiceTicketId = 1 };
            var ticket = new ServiceTicket { ServiceTicketId = 1 };
            var part = new Part { PartId = 1, PartQuantity = 10 };
            var service = new GarageService { GarageServiceId = 1 };

            _technicalTaskRepoMock.Setup(x => x.GetById(taskId)).ReturnsAsync(task);
            _serviceTicketRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(ticket);
            _userRepoMock.Setup(x => x.GetById(staffId)).ReturnsAsync(new User { UserId = staffId });
            _serviceTicketDetailRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<ServiceTicketDetail>());
            
            _partRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(part);
            _partRepoMock.Setup(x => x.UpdateAsync(1, It.IsAny<Part>())).ReturnsAsync(1);
            _serviceTicketDetailRepoMock.Setup(x => x.InsertAsync(It.IsAny<ServiceTicketDetail>())).ReturnsAsync(1);
            
            _garageServiceRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(service);
            
            _serviceTicketRepoMock.Setup(x => x.UpdateAsync(1, It.IsAny<ServiceTicket>())).ReturnsAsync(1);

            // Act
            var result = await _service.ApproveMechanicProposalAsync(taskId, request, staffId);

            // Assert
            result.Should().Be(1);
            // Verify deduction
             _partRepoMock.Verify(x => x.UpdateAsync(1, It.Is<Part>(p => p.PartQuantity == 8)), Times.Once);
            // Verify status update
            _serviceTicketRepoMock.Verify(x => x.UpdateAsync(1, It.Is<ServiceTicket>(st => 
                st.ServiceTicketStatus == SWP.Core.Constants.ServiceTicketStatus.ServiceTicketStatus.InProgress &&
                st.ModifiedBy == staffId)), Times.Once);
        }

        [Test]
        public void ApproveMechanicProposalAsync_WhenTaskNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var taskId = 999;
            var request = new ServiceTicketUpdatePartsServicesDto();
            
            _technicalTaskRepoMock.Setup(x => x.GetById(taskId)).ReturnsAsync((TechnicalTask)null!);

            // Act
            Func<Task> act = async () => await _service.ApproveMechanicProposalAsync(taskId, request, 1);

            // Assert
            act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Không tìm thấy technical task.");
        }
    }
}
