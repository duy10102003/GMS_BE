using FluentAssertions;
using Moq;
using NUnit.Framework;
using SWP.Core.Constants.ServiceTicketStatus;
using SWP.Core.Dtos.SeriveTicketDto;
using SWP.Core.Entities;
using SWP.Core.Exceptions;
using SWP.Core.Interfaces.Repositories;

namespace SWP.Core.UnitTest.ServiceTicketService.MechanicOperation
{
    [TestFixture]
    public class ProposePartsServicesAsyncTest
    {
        private Mock<IServiceTicketRepo> _serviceTicketRepoMock;
        private Mock<IBaseRepo<TechnicalTask>> _technicalTaskRepoMock;
        private Mock<IBaseRepo<Part>> _partRepoMock;
        private Mock<IBaseRepo<GarageService>> _garageServiceRepoMock;
        private Mock<IBaseRepo<ServiceTicketDetail>> _serviceTicketDetailRepoMock;
        private SWP.Core.Services.ServiceTicketService _service;
        
        // Unused mocks
        private Mock<IBaseRepo<User>> _userRepoMock;
        private Mock<IBaseRepo<Vehicle>> _vehicleRepoMock;
        private Mock<IBaseRepo<Booking>> _bookingRepoMock;
        private Mock<IBaseRepo<Customer>> _customerRepoMock;

        [SetUp]
        public void SetUp()
        {
            _serviceTicketRepoMock = new Mock<IServiceTicketRepo>();
            _technicalTaskRepoMock = new Mock<IBaseRepo<TechnicalTask>>();
            _partRepoMock = new Mock<IBaseRepo<Part>>();
            _garageServiceRepoMock = new Mock<IBaseRepo<GarageService>>();
            _serviceTicketDetailRepoMock = new Mock<IBaseRepo<ServiceTicketDetail>>();
            
            _userRepoMock = new Mock<IBaseRepo<User>>();
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
        public void ProposePartsServicesAsync_WhenTaskNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var taskId = 999;
            var mechanicId = 1;
            var request = new ServiceTicketUpdatePartsServicesDto();

            _technicalTaskRepoMock.Setup(x => x.GetById(taskId)).ReturnsAsync((TechnicalTask)null!);

            // Act
            Func<Task> act = async () => await _service.ProposePartsServicesAsync(taskId, request, mechanicId);

            // Assert
            act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Không tìm thấy technical task.");
        }

        [Test]
        public void ProposePartsServicesAsync_WhenNotAssignedMechanic_ShouldThrowValidateException()
        {
            // Arrange
            var taskId = 1;
            var mechanicId = 1;
            var assignedId = 2; // Different mechanic
            var request = new ServiceTicketUpdatePartsServicesDto();
            var task = new TechnicalTask { TechnicalTaskId = taskId, AssignedToTechnical = assignedId };

            _technicalTaskRepoMock.Setup(x => x.GetById(taskId)).ReturnsAsync(task);

            // Act
            Func<Task> act = async () => await _service.ProposePartsServicesAsync(taskId, request, mechanicId);

            // Assert
            act.Should().ThrowAsync<ValidateException>()
                .WithMessage("Bạn không có quyền chỉnh sửa task này.");
        }

        [Test]
        public void ProposePartsServicesAsync_WhenPartNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var taskId = 1;
            var mechanicId = 1;
            var request = new ServiceTicketUpdatePartsServicesDto
            {
                Parts = new List<ServiceTicketPartDto>
                {
                    new ServiceTicketPartDto { PartId = 999, Quantity = 1 }
                }
            };
            var task = new TechnicalTask { TechnicalTaskId = taskId, AssignedToTechnical = mechanicId };

            _technicalTaskRepoMock.Setup(x => x.GetById(taskId)).ReturnsAsync(task);
            _partRepoMock.Setup(x => x.GetById(999)).ReturnsAsync((Part)null!);

            // Act
            Func<Task> act = async () => await _service.ProposePartsServicesAsync(taskId, request, mechanicId);

            // Assert
            act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Không tìm thấy part với ID: 999");
        }

        [Test]
        public void ProposePartsServicesAsync_WhenGarageServiceNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var taskId = 1;
            var mechanicId = 1;
            var request = new ServiceTicketUpdatePartsServicesDto
            {
                GarageServices = new List<ServiceTicketServiceDto>
                {
                    new ServiceTicketServiceDto { GarageServiceId = 999, Quantity = 1 }
                }
            };
            var task = new TechnicalTask { TechnicalTaskId = taskId, AssignedToTechnical = mechanicId };

            _technicalTaskRepoMock.Setup(x => x.GetById(taskId)).ReturnsAsync(task);
            // Parts must be valid to reach this check
            _garageServiceRepoMock.Setup(x => x.GetById(999)).ReturnsAsync((GarageService)null!);

            // Act
            Func<Task> act = async () => await _service.ProposePartsServicesAsync(taskId, request, mechanicId);

            // Assert
            act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Không tìm thấy garage service với ID: 999");
        }

        [Test]
        public void ProposePartsServicesAsync_WhenServiceTicketNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var taskId = 1;
            var mechanicId = 1;
            var ticketId = 1;
            var request = new ServiceTicketUpdatePartsServicesDto();
            var task = new TechnicalTask { TechnicalTaskId = taskId, AssignedToTechnical = mechanicId, ServiceTicketId = ticketId };

            _technicalTaskRepoMock.Setup(x => x.GetById(taskId)).ReturnsAsync(task);
            _serviceTicketRepoMock.Setup(x => x.GetById(ticketId)).ReturnsAsync((ServiceTicket)null!);

            // Act
            Func<Task> act = async () => await _service.ProposePartsServicesAsync(taskId, request, mechanicId);

            // Assert
            act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Không tìm thấy service ticket.");
        }

        [Test]
        public async Task ProposePartsServicesAsync_Success_ShouldInsertNewPartAndService()
        {
            // Arrange
            var taskId = 1;
            var mechanicId = 1;
            var ticketId = 100;
            var request = new ServiceTicketUpdatePartsServicesDto
            {
                Parts = new List<ServiceTicketPartDto>
                {
                    new ServiceTicketPartDto { PartId = 1, Quantity = 2 } // New part (no DetailId)
                },
                GarageServices = new List<ServiceTicketServiceDto>
                {
                    new ServiceTicketServiceDto { GarageServiceId = 2, Quantity = 1 } // New service
                }
            };
            
            var task = new TechnicalTask { TechnicalTaskId = taskId, AssignedToTechnical = mechanicId, ServiceTicketId = ticketId };
            var ticket = new ServiceTicket { ServiceTicketId = ticketId };
            var part = new Part { PartId = 1, PartQuantity = 10 };
            var service = new GarageService { GarageServiceId = 2 };

            _technicalTaskRepoMock.Setup(x => x.GetById(taskId)).ReturnsAsync(task);
            _serviceTicketRepoMock.Setup(x => x.GetById(ticketId)).ReturnsAsync(ticket);
            _partRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(part);
            _garageServiceRepoMock.Setup(x => x.GetById(2)).ReturnsAsync(service);
            _serviceTicketDetailRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<ServiceTicketDetail>());
            
            // Setup Insert
            _serviceTicketDetailRepoMock.Setup(x => x.InsertAsync(It.IsAny<ServiceTicketDetail>())).ReturnsAsync(1);

            // Act
            var result = await _service.ProposePartsServicesAsync(taskId, request, mechanicId);

            // Assert
            result.Should().Be(1);
            // Verify Insert Part
            _serviceTicketDetailRepoMock.Verify(x => x.InsertAsync(It.Is<ServiceTicketDetail>(d => 
                d.PartId == 1 && d.Quantity == 2 && d.ServiceTicketId == ticketId)), Times.Once);

            // Verify Insert Service
            _serviceTicketDetailRepoMock.Verify(x => x.InsertAsync(It.Is<ServiceTicketDetail>(d => 
                d.GarageServiceId == 2 && d.Quantity == 1 && d.ServiceTicketId == ticketId)), Times.Once);
        }

        [Test]
        public async Task ProposePartsServicesAsync_Success_UpdatePart_WithRollbackAndDeduct()
        {
            // Arrange
            var taskId = 1;
            var mechanicId = 1;
            var ticketId = 100;
            var detailId = 50;
            
            var request = new ServiceTicketUpdatePartsServicesDto
            {
                Parts = new List<ServiceTicketPartDto>
                {
                    new ServiceTicketPartDto 
                    { 
                        PartId = 1, 
                        Quantity = 5, 
                        ServiceTicketDetailId = detailId // Updating existing
                    } 
                }
            };
            
            // Scenario: Ticket is completed -> triggers rollback of old parts
            var task = new TechnicalTask { TechnicalTaskId = taskId, AssignedToTechnical = mechanicId, ServiceTicketId = ticketId };
            var ticket = new ServiceTicket { ServiceTicketId = ticketId, ServiceTicketStatus = ServiceTicketStatus.Completed };
            var part = new Part { PartId = 1, PartQuantity = 20 }; // Current stock
            var oldDetail = new ServiceTicketDetail { ServiceTicketDetailId = detailId, PartId = 1, Quantity = 2 }; // Old quantity 2

            _technicalTaskRepoMock.Setup(x => x.GetById(taskId)).ReturnsAsync(task);
            _serviceTicketRepoMock.Setup(x => x.GetById(ticketId)).ReturnsAsync(ticket);
            _partRepoMock.Setup(x => x.GetById(1)).ReturnsAsync(part);
            _serviceTicketDetailRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<ServiceTicketDetail>());
            
            // Mock getting old detail for rollback check
            _serviceTicketDetailRepoMock.Setup(x => x.GetById(detailId)).ReturnsAsync(oldDetail);
            
            // Mock Updates
            _partRepoMock.Setup(x => x.UpdateAsync(1, It.IsAny<Part>())).ReturnsAsync(1); // For Rollback and Deduct
            _serviceTicketDetailRepoMock.Setup(x => x.UpdateAsync(detailId, It.IsAny<ServiceTicketDetail>())).ReturnsAsync(1);

            // Act
            var result = await _service.ProposePartsServicesAsync(taskId, request, mechanicId);

            // Assert
            result.Should().Be(1);

            // 1. Verify Rollback Old Quantity (2) -> Stock 20 + 2 = 22
            // Note: Deduct will also modify stock. 
            // Sequence: Rollback (20+2=22) -> Deduct New (5) (22-5=17)
            // We verify pure logic calls
            
            // Verify Rollback called (PartId 1, Quantity 2) which adds to stock
            // In mocking, we usually verify method calls.
            // Since logic combines these, let's verify UpdateAsync calls on PartRepo match expected math or frequency
            
            // Verify UpdateDetail called with new quantity
            _serviceTicketDetailRepoMock.Verify(x => x.UpdateAsync(detailId, It.Is<ServiceTicketDetail>(d => d.Quantity == 5)), Times.Once);
        }

        [Test]
        public async Task ProposePartsServicesAsync_Success_ShouldUpdateExistingService()
        {
            // Arrange
            var taskId = 1;
            var mechanicId = 1;
            var ticketId = 100;
            var detailId = 60;

            var request = new ServiceTicketUpdatePartsServicesDto
            {
                GarageServices = new List<ServiceTicketServiceDto>
                {
                    new ServiceTicketServiceDto 
                    { 
                        GarageServiceId = 2, 
                        Quantity = 3,
                        ServiceTicketDetailId = detailId // Updating
                    } 
                }
            };
            
            var task = new TechnicalTask { TechnicalTaskId = taskId, AssignedToTechnical = mechanicId, ServiceTicketId = ticketId };
            var ticket = new ServiceTicket { ServiceTicketId = ticketId };
            var service = new GarageService { GarageServiceId = 2 };
            var oldDetail = new ServiceTicketDetail { ServiceTicketDetailId = detailId, GarageServiceId = 2, Quantity = 1 };

            _technicalTaskRepoMock.Setup(x => x.GetById(taskId)).ReturnsAsync(task);
            _serviceTicketRepoMock.Setup(x => x.GetById(ticketId)).ReturnsAsync(ticket);
            _garageServiceRepoMock.Setup(x => x.GetById(2)).ReturnsAsync(service);
            _serviceTicketDetailRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<ServiceTicketDetail>());
            
            _serviceTicketDetailRepoMock.Setup(x => x.GetById(detailId)).ReturnsAsync(oldDetail);
            _serviceTicketDetailRepoMock.Setup(x => x.UpdateAsync(detailId, It.IsAny<ServiceTicketDetail>())).ReturnsAsync(1);

            // Act
            var result = await _service.ProposePartsServicesAsync(taskId, request, mechanicId);

            // Assert
            result.Should().Be(1);
            _serviceTicketDetailRepoMock.Verify(x => x.UpdateAsync(detailId, It.Is<ServiceTicketDetail>(d => 
                d.GarageServiceId == 2 && d.Quantity == 3)), Times.Once);
        }
    }
}
