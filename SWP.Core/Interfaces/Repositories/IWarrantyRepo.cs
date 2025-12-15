using SWP.Core.Dtos.WarrantyDto;
using SWP.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWP.Core.Entities;

namespace SWP.Core.Interfaces.Repositories
{
    public interface IWarrantyRepo: IBaseRepo<Warranty>
    {
        /// <summary>
        /// Manager xem toàn bộ warranty
        /// </summary>
        Task<PagedResult<WarrantyListItemDto>> GetPagingAsync(
            WarrantyFilterDtoRequest filter);

        /// <summary>
        /// Customer xem warranty của mình (có paging + search + sort)
        /// </summary>
        Task<PagedResult<WarrantyListItemDto>> GetPagingByCustomerAsync(
            int customerId,
            WarrantyFilterDtoRequest filter);

        /// <summary>
        /// Kiểm tra warranty đã tồn tại theo service_ticket_detail
        /// </summary>
        Task<Warranty?> GetByServiceTicketDetailIdAsync(int serviceTicketDetailId);

        Task CreateWarrantyForServiceTicketAsync(int serviceTicketId);

        Task<int> AutoUpdateExpiredAsync();
    }
}
