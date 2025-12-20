using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SWP.Core.Dtos;
using SWP.Core.Dtos.BookingDto;
using SWP.Core.Interfaces.Repositories;

namespace SWP.Core.UnitTest.BookingService
{
    [TestFixture]
    public class GetPagingAsyncTest
    {
        private Mock<IBookingRepo> _bookingRepoMock;
        private Mock<ICustomerRepo> _customerRepoMock;
        private Mock<IUserRepo> _userRepoMock;
        private SWP.Core.Services.BookingService _service;

        [SetUp]
        public void SetUp()
        {
            _bookingRepoMock = new Mock<IBookingRepo>();
            _customerRepoMock = new Mock<ICustomerRepo>();
            _userRepoMock = new Mock<IUserRepo>();

            _service = new SWP.Core.Services.BookingService(
                _bookingRepoMock.Object,
                _customerRepoMock.Object,
                _userRepoMock.Object
            );
        }

        [Test]
        public async Task GetPagingAsync_WhenDataExists_ShouldReturnPagedResult()
        {
            // Arrange
            var filter = new BookingFilterDtoRequest
            {
                Page = 1,
                PageSize = 10
            };

            var expectedResult = new PagedResult<BookingListItemDto>
            {
                Items = new List<BookingListItemDto>
                {
                    new BookingListItemDto
                    {
                        BookingId = 1,
                        BookingTime = DateTime.UtcNow,
                        BookingStatus = 0,
                        VehicleName = "Sedan"
                    }
                },
                Total = 1,
                Page = 1,
                PageSize = 10
            };

            _bookingRepoMock
                .Setup(x => x.GetPagingAsync(filter))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _service.GetPagingAsync(filter);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(1);
            result.Total.Should().Be(1);
            _bookingRepoMock.Verify(x => x.GetPagingAsync(filter), Times.Once);
        }

        [Test]
        public async Task GetPagingAsync_WhenRepoReturnsNull_ShouldReturnNull()
        {
            // Arrange
            var filter = new BookingFilterDtoRequest
            {
                Page = 1,
                PageSize = 10
            };

            _bookingRepoMock
                .Setup(x => x.GetPagingAsync(filter))
                .ReturnsAsync((PagedResult<BookingListItemDto>)null!);

            // Act
            var result = await _service.GetPagingAsync(filter);

            // Assert
            result.Should().BeNull();
            _bookingRepoMock.Verify(x => x.GetPagingAsync(filter), Times.Once);
        }
    }
}
