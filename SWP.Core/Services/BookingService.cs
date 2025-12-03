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
        private readonly IBaseRepo<Customer> _customerRepo;

        public BookingService(IBookingRepo bookingRepo, IBaseRepo<Customer> customerRepo)
        {
            _bookingRepo = bookingRepo;
            _customerRepo = customerRepo;
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
                throw new NotFoundException("Không tìm thấy booking.");
            }
            return result;
        }

        public async Task<int> CreateAsync(BookingCreateDto request)
        {
            await EnsureCustomerExists(request.CustomerId);

            var entity = new Booking
            {
                CustomerId = request.CustomerId,
                BookingTime = request.BookingTime,
                VehicleName = request.VehicleName?.Trim(),
                Note = request.Note?.Trim(),
                BookingStatus = 0,
                CreatedDate = DateTime.UtcNow,
                IsDeleted = 0
            };

            // BaseRepo returns object (boxed key), convert to int for Booking
            return Convert.ToInt32(await _bookingRepo.InsertAsync(entity));
        }

        public async Task<int> UpdateAsync(int id, BookingCreateDto request)
        {
            await EnsureCustomerExists(request.CustomerId);

            var existing = await _bookingRepo.GetById(id);
            if (existing == null)
            {
                throw new NotFoundException("Không tìm thấy booking.");
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
                throw new NotFoundException("Không tìm thấy booking.");
            }
            return await _bookingRepo.DeleteAsync(id);
        }

        private async Task EnsureCustomerExists(int customerId)
        {
            var customer = await _customerRepo.GetById(customerId);
            if (customer == null)
            {
                throw new NotFoundException("Không tìm thấy khách hàng.");
            }
        }
    }
}
