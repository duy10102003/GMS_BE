using SWP.Core.Dtos.WarrantyDto;
using SWP.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWP.Core.Entities;

namespace SWP.Core.Interfaces.Services
{
    public interface IWarrantyService
    {
        /// <summary>
        /// Manager: Lấy danh sách warranty có phân trang / sort / filter
        /// </summary>
        Task<PagedResult<WarrantyListItemDto>> GetPagingAsync(WarrantyFilterDtoRequest filter);

        /// <summary>
        /// Customer: Lấy danh sách warranty theo customer
        /// </summary>
        Task<PagedResult<WarrantyListItemDto>> GetByCustomerPagingAsync(
            int customerId,
            WarrantyFilterDtoRequest filter);

        /// <summary>
        /// Lấy chi tiết warranty
        /// </summary>
        Task<Warranty?> GetByIdAsync(int id);



    }
}
