using System;
using SWP.Core.Dtos;
using SWP.Core.Dtos.BookingDto;
using SWP.Core.Entities;
using SWP.Core.Exceptions;
using SWP.Core.Interfaces.Repositories;
using SWP.Core.Interfaces.Services;

namespace SWP.Core.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepo _bookingRepo;
        private readonly ICustomerRepo _customerRepo;
        private readonly IUserRepo _userRepo;

        public BookingService(IBookingRepo bookingRepo, ICustomerRepo customerRepo, IUserRepo userRepo)
        {
            _bookingRepo = bookingRepo;
            _customerRepo = customerRepo;
            _userRepo = userRepo;
        }

        public Task<PagedResult<BookingListItemDto>> GetPagingAsync(BookingFilterDtoRequest filter)
        {
            return _bookingRepo.GetPagingAsync(filter);
        }

        public async Task<BookingDetailDto> GetByIdAsync(int id)
        {
            var result = await _bookingRepo.GetDetailAsync(id);
            if (result == null)
            {
                throw new NotFoundException("Khong tim thay booking.");
            }
            return result;
        }

        public async Task<int> CreateForGuestAsync(BookingCreateGuestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.CustomerPhone))
            {
                throw new ValidateException("Customer phone is required.");
            }

            var customer = new Customer
            {
                CustomerName = request.CustomerName?.Trim(),
                CustomerPhone = request.CustomerPhone.Trim(),
                CustomerEmail = request.CustomerEmail?.Trim(),
                IsDeleted = 0
            };

            var customerId = Convert.ToInt32(await _customerRepo.InsertAsync(customer));

            return await CreateBookingInternal(
                customerId,
                request.BookingTime,
                request.VehicleName,
                request.Note);
        }

        public async Task<int> CreateForUserAsync(BookingCreateForUserDto request)
        {
            var user = await _userRepo.GetById(request.UserId);
            if (user == null)
            {
                throw new NotFoundException("Khong tim thay user.");
            }

            var customer = await _customerRepo.GetByUserIdAsync(request.UserId);
            if (customer == null)
            {
                if (string.IsNullOrWhiteSpace(user.Phone))
                {
                    throw new ValidateException("User phone is required to create booking.");
                }

                customer = new Customer
                {
                    UserId = request.UserId,
                    CustomerName = user.FullName?.Trim(),
                    CustomerPhone = user.Phone.Trim(),
                    CustomerEmail = user.Email?.Trim(),
                    IsDeleted = 0
                };

                var customerId = Convert.ToInt32(await _customerRepo.InsertAsync(customer));
                customer.CustomerId = customerId;
            }

            return await CreateBookingInternal(
                customer.CustomerId,
                request.BookingTime,
                request.VehicleName,
                request.Note);
        }

        public async Task<int> UpdateAsync(int id, BookingCreateDto request)
        {
            await EnsureCustomerExists(request.CustomerId);

            var existing = await _bookingRepo.GetById(id);
            if (existing == null)
            {
                throw new NotFoundException("Khong tim thay booking.");
            }

            existing.CustomerId = request.CustomerId;
            existing.BookingTime = request.BookingTime;
            existing.VehicleName = request.VehicleName?.Trim();
            existing.Note = request.Note?.Trim();
            existing.ModifiedDate = DateTime.UtcNow;

            return await _bookingRepo.UpdateAsync(id, existing);
        }

        public async Task<int> DeleteAsync(int id)
        {
            var existing = await _bookingRepo.GetById(id);
            if (existing == null)
            {
                throw new NotFoundException("Khong tim thay booking.");
            }
            return await _bookingRepo.DeleteAsync(id);
        }

        private async Task EnsureCustomerExists(int customerId)
        {
            var customer = await _customerRepo.GetById(customerId);
            if (customer == null)
            {
                throw new NotFoundException("Khong tim thay khach hang.");
            }
        }

        private async Task<int> CreateBookingInternal(int customerId, DateTime bookingTime, string? vehicleName, string? note)
        {
            var entity = new Booking
            {
                CustomerId = customerId,
                BookingTime = bookingTime,
                VehicleName = vehicleName?.Trim(),
                Note = note?.Trim(),
                BookingStatus = 0,
                CreatedDate = DateTime.UtcNow,
                IsDeleted = 0
            };

            return Convert.ToInt32(await _bookingRepo.InsertAsync(entity));
        }
    }
}
