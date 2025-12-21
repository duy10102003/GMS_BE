using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SWP.Core.Constants.BookingStatus;
using SWP.Core.Dtos.BookingDto;
using SWP.Core.Entities;
using SWP.Core.Exceptions;
using SWP.Core.Interfaces.Repositories;

namespace SWP.Core.UnitTest.BookingService
{
    [TestFixture]
    public class CreateForGuestAsyncTest
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
        public async Task CreateForGuestAsync_WhenCustomerExists_ShouldReuseCustomer()
        {
            var request = new BookingCreateGuestDto
            {
                CustomerName = "Nguyen Van A",
                CustomerPhone = "0123456789",
                CustomerEmail = "a@test.com",
                BookingTime = new DateTime(2025, 1, 1),
                VehicleName = "Sedan",
                Reason = "Fix",
                Note = "Note"
            };

            var existingCustomer = new Customer { CustomerId = 5 };

            _customerRepoMock
                .Setup(x => x.FindByIdentityAsync("Nguyen Van A", "0123456789", "a@test.com"))
                .ReturnsAsync(existingCustomer);

            _bookingRepoMock
                .Setup(x => x.InsertAsync(It.IsAny<Booking>()))
                .ReturnsAsync(100);

            var result = await _service.CreateForGuestAsync(request);

            result.Should().Be(100);
            _customerRepoMock.Verify(x => x.InsertAsync(It.IsAny<Customer>()), Times.Never);
            _bookingRepoMock.Verify(
                x => x.InsertAsync(It.Is<Booking>(b =>
                    b.CustomerId == 5 &&
                    b.BookingTime == request.BookingTime &&
                    b.VehicleName == "Sedan" &&
                    b.Reason == "Fix" &&
                    b.Note == "Note" &&
                    b.BookingStatus == BookingStatus.Pending &&
                    b.IsDeleted == 0
                )),
                Times.Once);
        }

        [Test]
        public async Task CreateForGuestAsync_WhenCustomerNotFound_ShouldCreateCustomer()
        {
            var request = new BookingCreateGuestDto
            {
                CustomerName = "B",
                CustomerPhone = "0123456987",
                CustomerEmail = "b@test.com",
                BookingTime = new DateTime(2025, 2, 2),
                VehicleName = "SUV",
                Reason = "Check"
            };

            _customerRepoMock
                .Setup(x => x.FindByIdentityAsync("B", "0123456987", "b@test.com"))
                .ReturnsAsync((Customer)null!);

            _customerRepoMock
                .Setup(x => x.InsertAsync(It.IsAny<Customer>()))
                .ReturnsAsync(77);

            _bookingRepoMock
                .Setup(x => x.InsertAsync(It.IsAny<Booking>()))
                .ReturnsAsync(200);

            var result = await _service.CreateForGuestAsync(request);

            result.Should().Be(200);
            _customerRepoMock.Verify(
                x => x.InsertAsync(It.Is<Customer>(c =>
                    c.CustomerName == "B" &&
                    c.CustomerPhone == "0123456987" &&
                    c.CustomerEmail == "b@test.com" &&
                    c.IsDeleted == 0
                )),
                Times.Once);
            _bookingRepoMock.Verify(
                x => x.InsertAsync(It.Is<Booking>(b =>
                    b.CustomerId == 77 &&
                    b.BookingTime == request.BookingTime &&
                    b.VehicleName == "SUV" &&
                    b.Reason == "Check" &&
                    b.BookingStatus == BookingStatus.Pending &&
                    b.IsDeleted == 0
                )),
                Times.Once);
        }

        [Test]
        public void CreateForGuestAsync_WhenMissingCustomerName_ShouldThrowValidateException()
        {
            var request = new BookingCreateGuestDto
            {
                CustomerName = " ",
                CustomerPhone = "0123456789",
                CustomerEmail = "a@test.com",
                BookingTime = new DateTime(2025, 3, 3),
                VehicleName = "SUV",
                Reason = "Check",
                Note = "Note"
            };

            Func<Task> act = async () => await _service.CreateForGuestAsync(request);

            act.Should().ThrowAsync<ValidateException>();
            _customerRepoMock.Verify(x => x.FindByIdentityAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _bookingRepoMock.Verify(x => x.InsertAsync(It.IsAny<Booking>()), Times.Never);
        }

        [Test]
        public void CreateForGuestAsync_WhenEmailInvalid_ShouldThrowValidateException()
        {
            var request = new BookingCreateGuestDto
            {
                CustomerName = "A",
                CustomerPhone = "0123456987",
                CustomerEmail = "not-an-email",
                BookingTime = new DateTime(2025, 4, 4),
                VehicleName = "SUV",
                Reason = "Check",
                Note = "Note"
            };

            Func<Task> act = async () => await _service.CreateForGuestAsync(request);

            act.Should().ThrowAsync<ValidateException>();
            _customerRepoMock.Verify(x => x.FindByIdentityAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _bookingRepoMock.Verify(x => x.InsertAsync(It.IsAny<Booking>()), Times.Never);
        }

        [Test]
        public void CreateForGuestAsync_WhenBookingTimeMissing_ShouldThrowValidateException()
        {
            var request = new BookingCreateGuestDto
            {
                CustomerName = "A",
                CustomerPhone = "0123456987",
                CustomerEmail = "a@test.com",
                BookingTime = default,
                VehicleName = "SUV",
                Reason = "Check",
                Note = "Note"
            };

            Func<Task> act = async () => await _service.CreateForGuestAsync(request);

            act.Should().ThrowAsync<ValidateException>();
            _customerRepoMock.Verify(x => x.FindByIdentityAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _bookingRepoMock.Verify(x => x.InsertAsync(It.IsAny<Booking>()), Times.Never);
        }

        [Test]
        public async Task CreateForGuestAsync_WhenInputHasSpaces_ShouldTrimAndReuseCustomer()
        {
            var request = new BookingCreateGuestDto
            {
                CustomerName = "  Nguyen Van A  ",
                CustomerPhone = " 0123456789 ",
                CustomerEmail = "a@test.com",
                BookingTime = new DateTime(2025, 5, 5),
                VehicleName = "  Sedan ",
                Reason = "  Fix  ",
                Note = "  Note  "
            };

            var existingCustomer = new Customer { CustomerId = 9 };

            _customerRepoMock
                .Setup(x => x.FindByIdentityAsync("Nguyen Van A", "0123456789", "a@test.com"))
                .ReturnsAsync(existingCustomer);

            _bookingRepoMock
                .Setup(x => x.InsertAsync(It.IsAny<Booking>()))
                .ReturnsAsync(101);

            var result = await _service.CreateForGuestAsync(request);

            result.Should().Be(101);
            _customerRepoMock.Verify(x => x.InsertAsync(It.IsAny<Customer>()), Times.Never);
            _bookingRepoMock.Verify(
                x => x.InsertAsync(It.Is<Booking>(b =>
                    b.CustomerId == 9 &&
                    b.BookingTime == request.BookingTime &&
                    b.VehicleName == "Sedan" &&
                    b.Reason == "Fix" &&
                    b.Note == "Note" &&
                    b.BookingStatus == BookingStatus.Pending &&
                    b.IsDeleted == 0
                )),
                Times.Once);
        }
    }
}
