using FluentAssertions;
using Moq;
using NUnit.Framework;
using SWP.Core.Dtos;
using SWP.Core.Dtos.SeriveTicketDto;
using SWP.Core.Entities;
using SWP.Core.Exceptions;
using SWP.Core.Interfaces.Repositories;

namespace SWP.Core.UnitTest.ServiceTicketService.StaffOperation
{
    [TestFixture]
    public class GetPagingAsyncTest
    {
        private Mock<IServiceTicketRepo> _serviceTicketRepoMock;
        private Mock<IBaseRepo<User>> _userRepoMock;
        private Mock<IBaseRepo<Vehicle>> _vehicleRepoMock;
        private Mock<IBaseRepo<Part>> _partRepoMock;
        private Mock<IBaseRepo<Booking>> _bookingRepoMock;
        private Mock<IBaseRepo<Customer>> _customerRepoMock;
        private Mock<IBaseRepo<ServiceTicketDetail>> _serviceTicketDetailRepoMock;
        private Mock<IBaseRepo<TechnicalTask>> _technicalTaskRepoMock;
        private Mock<IBaseRepo<GarageService>> _garageServiceRepoMock;
        private SWP.Core.Services.ServiceTicketService _service;

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
        public async Task GetPagingAsync_WhenDataExists_ShouldReturnPagedResult()
        {
            // Arrange
            var filter = new ServiceTicketFilterDtoRequest
            {
                Page = 1,
                PageSize = 10
            };

            var expectedResult = new PagedResult<ServiceTicketListItemDto>
            {
                Items = new List<ServiceTicketListItemDto>
                {
                    new ServiceTicketListItemDto
                    {
                        ServiceTicketId = 1,
                        ServiceTicketCode = "ST001"
                    }
                },
                Total = 1,
                Page = 1,
                PageSize = 10
            };

            _serviceTicketRepoMock
                .Setup(x => x.GetPagingAsync(filter))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _service.GetPagingAsync(filter);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(1);
            result.Total.Should().Be(1);
            _serviceTicketRepoMock.Verify(x => x.GetPagingAsync(filter), Times.Once);
        }

        [Test]
        public void GetPagingAsync_WhenRepoReturnsNull_ShouldThrowNotFoundException()
        {
            // Arrange
            var filter = new ServiceTicketFilterDtoRequest
            {
                Page = 1,
                PageSize = 10
            };

            _serviceTicketRepoMock
                .Setup(x => x.GetPagingAsync(filter))
                .ReturnsAsync((PagedResult<ServiceTicketListItemDto>)null!);

            // Act
            Func<Task> act = async () => await _service.GetPagingAsync(filter);

            // Assert
            act.Should().ThrowAsync<NotFoundException>();
            _serviceTicketRepoMock.Verify(x => x.GetPagingAsync(filter), Times.Once);
        }
    }
}
