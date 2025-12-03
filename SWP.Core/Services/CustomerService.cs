using SWP.Core.Dtos.CustomerDto;
using SWP.Core.Interfaces.Repositories;
using SWP.Core.Interfaces.Services;

namespace SWP.Core.Services
{
    /// <summary>
    /// Service cho Customer
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepo _customerRepo;

        public CustomerService(ICustomerRepo customerRepo)
        {
            _customerRepo = customerRepo;
        }

        /// <summary>
        /// Tìm kiếm Customer cho select (với search keyword)
        /// </summary>
        public Task<List<CustomerSelectDto>> SearchForSelectAsync(CustomerSearchRequest request)
        {
            return _customerRepo.SearchForSelectAsync(request);
        }
    }
}

