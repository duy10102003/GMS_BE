using FluentAssertions;
using Moq;
using NUnit.Framework;
using SWP.Core.Dtos;
using SWP.Core.Dtos.SeriveTicketDto;
using SWP.Core.Entities;
using SWP.Core.Exceptions;
using SWP.Core.Interfaces.Repositories;

namespace SWP.Core.UnitTest.ServiceTicketService.CustomerOperation
{
    [TestFixture]
    public class GetPagingServiceTicketForCustomerAsyncTest
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
        public async Task GetPagingServiceTicketForCustomerAsync_ShouldReturnResult()
        {
            // Arrange
            var id = 1;
            var filter = new ServiceTicketFilterDtoRequest();
            var pagedResult = new PagedResult<ServiceTicketListItemDto>
            {
                Items = new List<ServiceTicketListItemDto> { new ServiceTicketListItemDto { ServiceTicketId = 1 } }
            };

            _serviceTicketRepoMock
                .Setup(x => x.GetPagingServiceTicketForCustomerAsync(id, filter))
                .Returns(Task.FromResult(pagedResult));

            // Act
            var result = await _service.GetPagingServiceTicketForCustomerAsync(id, filter);

            // Assert
            result.Should().BeEquivalentTo(pagedResult);
        }

        [Test]
        public void GetPagingServiceTicketForCustomerAsync_WhenResultIsNull_ShouldThrowNotFoundException()
        {
            // Arrange
            var id = 1;
            var filter = new ServiceTicketFilterDtoRequest();
            
            _serviceTicketRepoMock
                .Setup(x => x.GetPagingServiceTicketForCustomerAsync(id, filter))
                .Returns((Task<PagedResult<ServiceTicketListItemDto>>)null!);

            // Act
            Action act = () => _service.GetPagingServiceTicketForCustomerAsync(id, filter);

            // Assert
            act.Should().Throw<NotFoundException>()
                .WithMessage("Không tìm thấy danh sách service ticket.");
        }
    }
}
