using SWP.Core.Dtos;
using SWP.Core.Dtos.WarrantyDto;
using SWP.Core.Entities;
using SWP.Core.Exceptions;
using SWP.Core.Interfaces.Repositories;
using SWP.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWP.Core.Services
{
    public class WarrantyService : IWarrantyService
    {
        private readonly IWarrantyRepo _warrantyRepo;
        //private readonly IServiceTicketRepo _serviceTicketRepo;
        private readonly IBaseRepo<Warranty> _warrantyBaseRepo;

        public WarrantyService(
            IWarrantyRepo warrantyRepo,
            IBaseRepo<Warranty> warrantyBaseRepo)
        {
            _warrantyRepo = warrantyRepo;
            _warrantyBaseRepo = warrantyBaseRepo;
        }

        
        public Task<PagedResult<WarrantyListItemDto>> GetByCustomerPagingAsync(int customerId, WarrantyFilterDtoRequest filter)
        {
            var result = _warrantyRepo.GetPagingByCustomerAsync(customerId, filter);
            if (result == null)
            {
                throw new NotFoundException("Không tìm thấy danh sách bảo hành.");
            }
            return result;
        }

        public Task<Warranty?> GetByIdAsync(int id)
        {
            var result = _warrantyRepo.GetById(id);
            if (result == null)
            {
                throw new NotFoundException("Không tìm thấy bảo hành.");
            }
            return result;
        }

        public Task<PagedResult<WarrantyListItemDto>> GetPagingAsync(WarrantyFilterDtoRequest filter)
        {
            var result = _warrantyRepo.GetPagingAsync(filter);
            if (result == null)
            {
                throw new NotFoundException("Không tìm thấy danh sách bảo hành.");
            }
            return result;
        }
    }
}
