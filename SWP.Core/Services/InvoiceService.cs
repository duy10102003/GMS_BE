using SWP.Core.Constants.ServiceTicketStatus;
using SWP.Core.Dtos;
using SWP.Core.Dtos.InvoiceDto;
using SWP.Core.Dtos.SeriveTicketDto;
using SWP.Core.Entities;
using SWP.Core.Exceptions;
using SWP.Core.Interfaces.Repositories;
using SWP.Core.Interfaces.Services;

namespace SWP.Core.Services
{
    /// <summary>
    /// Service cho Invoice
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepo _invoiceRepo;
        private readonly IServiceTicketRepo _serviceTicketRepo;
        private readonly IBaseRepo<Invoice> _invoiceBaseRepo;
        private readonly IWarrantyRepo _warrantyRepo;

        public InvoiceService(
            IInvoiceRepo invoiceRepo,
            IServiceTicketRepo serviceTicketRepo,
            IBaseRepo<Invoice> invoiceBaseRepo,
            IWarrantyRepo warrantyRepo)
        {
            _invoiceRepo = invoiceRepo;
            _serviceTicketRepo = serviceTicketRepo;
            _invoiceBaseRepo = invoiceBaseRepo;
            _warrantyRepo = warrantyRepo;
        }

        /// <summary>
        /// Lấy danh sách Invoice có phân trang
        /// </summary>
        public Task<PagedResult<InvoiceListItemDto>> GetPagingAsync(InvoiceFilterDtoRequest filter)
        {
            var result = _invoiceRepo.GetPagingAsync(filter);
            if (result == null)
            {
                throw new NotFoundException("Không tìm thấy danh sách invoice.");
            }
            return result;
        }

        /// <summary>
        /// Lấy Invoice theo ID
        /// </summary>
        public async Task<InvoiceDetailDto> GetByIdAsync(int id)
        {
            var result = await _invoiceRepo.GetDetailAsync(id);
            if (result == null)
            {
                throw new NotFoundException("Không tìm thấy invoice.");
            }
            return result;
        }

        /// <summary>
        /// Tạo Invoice từ Service Ticket đã hoàn thành
        /// </summary>
        public async Task<int> CreateFromServiceTicketAsync(InvoiceCreateDto request)
        {
            // Kiểm tra service ticket tồn tại và đã hoàn thành
            var serviceTicket = await _serviceTicketRepo.GetById(request.ServiceTicketId);
            if (serviceTicket == null)
            {
                throw new NotFoundException("Không tìm thấy service ticket.");
            }

            if (serviceTicket.ServiceTicketStatus != 3) // Completed
            {
                throw new ValidateException("Chỉ có thể tạo hóa đơn khi service ticket đã hoàn thành.");
            }

            // Kiểm tra invoice đã tồn tại chưa
            var existingInvoice = await _invoiceRepo.GetByServiceTicketIdAsync(request.ServiceTicketId);
            if (existingInvoice != null)
            {
                throw new ConflictException("Hóa đơn cho service ticket này đã tồn tại.");
            }

            // Lấy chi tiết service ticket để tính toán
            var serviceTicketDetail = await _serviceTicketRepo.GetDetailAsync(request.ServiceTicketId);
            if (serviceTicketDetail == null)
            {
                throw new NotFoundException("Không tìm thấy chi tiết service ticket.");
            }

            // Tính toán parts amount
            decimal partsAmount = 0;
            if (serviceTicketDetail.Parts != null && serviceTicketDetail.Parts.Any())
            {
                partsAmount = serviceTicketDetail.Parts.Sum(p => 
                    (p.Part.PartPrice ?? 0) * p.Quantity);
            }

            // Tính toán garage service amount (không có quantity)
            decimal garageServiceAmount = 0;
            if (serviceTicketDetail.GarageServices != null && serviceTicketDetail.GarageServices.Any())
            {
                garageServiceAmount = serviceTicketDetail.GarageServices.Sum(s => 
                    s.GarageService.GarageServicePrice ?? 0);
            }

            // Tính tổng tiền
            var subtotal = partsAmount + garageServiceAmount;
            var taxAmount = request.TaxAmount ?? 0;
            var discountAmount = request.DiscountAmount ?? 0;
            var totalAmount = subtotal + taxAmount - discountAmount;

            // Tạo invoice
            var invoice = new Invoice
            {
                ServiceTicketId = request.ServiceTicketId,
                CustomerId = request.CustomerId,
                IssueDate = DateTime.Now,
                PartsAmount = partsAmount,
                GarageServiceAmount = garageServiceAmount,
                TaxAmount = taxAmount,
                DiscountAmount = discountAmount,
                TotalAmount = totalAmount,
                InvoiceStatus = 0, // Pending
                InvoiceCode = request.InvoiceCode
            };

            // Chỉ cho phép chuyển từ Completed
            //if (serviceTicket.ServiceTicketStatus != ServiceTicketStatus.Completed)
            //{
            //    throw new ValidateException("Chỉ có thể chuyển sang trạng thái này từ Completed.");
            //}

            // tao bao hanh



            //if (serviceTicket.ServiceTicketStatus != ServiceTicketStatus.Completed)
            //{
            //    serviceTicket.ServiceTicketStatus = ServiceTicketStatus.CompletedPayment;
            //}
            //else
            //{
            //    serviceTicket.ServiceTicketStatus = ServiceTicketStatus.Closed;
            //}
            serviceTicket.ModifiedDate = DateTime.UtcNow;

            await _serviceTicketRepo.UpdateAsync(request.ServiceTicketId, serviceTicket);
            var idObj = await _invoiceBaseRepo.InsertAsync(invoice);
            return idObj is int id ? id : (int)idObj;
        }

        /// <summary>
        /// Chuyển trạng thái Invoice sang đã thanh toán (status = 1)
        /// </summary>
        public async Task ChangeStatusToPaidAsync(int id, int modifiedBy)
        {
            // Kiểm tra invoice tồn tại
            var invoice = await _invoiceBaseRepo.GetById(id);
            if (invoice == null)
            {
                throw new NotFoundException("Không tìm thấy invoice.");
            }

            // Kiểm tra invoice chưa bị xóa
            if (invoice.IsDeleted == 1)
            {
                throw new NotFoundException("Invoice đã bị xóa.");
            }

            // Kiểm tra invoice chưa thanh toán
            if (invoice.InvoiceStatus == 1)
            {
                throw new ValidateException("Invoice đã được thanh toán rồi.");
            }

            // Lấy service ticket
            var serviceTicket = await _serviceTicketRepo.GetById(invoice.ServiceTicketId);
            if (serviceTicket == null || serviceTicket.IsDeleted == 1)
                throw new NotFoundException("Không tìm thấy service ticket.");
            // Cập nhật trạng thái
            invoice.InvoiceStatus = 1; // Paid
            await _invoiceBaseRepo.UpdateAsync(id, invoice);

            // Xử lý theo trạng thái service ticket
            if (serviceTicket.ServiceTicketStatus == ServiceTicketStatus.Completed)
            {
                // Closed
                serviceTicket.ServiceTicketStatus = ServiceTicketStatus.Closed;
                serviceTicket.ModifiedBy = modifiedBy;
                serviceTicket.ModifiedDate = DateTime.Now;

                await _serviceTicketRepo.UpdateAsync(serviceTicket.ServiceTicketId, serviceTicket);

                // Tạo bảo hành
                await _warrantyRepo.CreateWarrantyForServiceTicketAsync(serviceTicket.ServiceTicketId);
            }
            else
            {
                // Chỉ chuyển sang đã thanh toán
                serviceTicket.ServiceTicketStatus = ServiceTicketStatus.CompletedPayment;
                serviceTicket.ModifiedBy = modifiedBy;
                serviceTicket.ModifiedDate = DateTime.Now;

                await _serviceTicketRepo.UpdateAsync(serviceTicket.ServiceTicketId, serviceTicket);
            }
        }

        public Task<PagedResult<InvoiceListItemDto>> GetPagingInvoiceForCustomerAsync(int userId, InvoiceFilterDtoRequest filter)
        {
            var result = _invoiceRepo.GetPagingInvoiceForCustomerAsync(userId,filter);
            if (result == null)
            {
                throw new NotFoundException("Không tìm thấy danh sách invoice.");
            }
            return result;
        }
    }
}

