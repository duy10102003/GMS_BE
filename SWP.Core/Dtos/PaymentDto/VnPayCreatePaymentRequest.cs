using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWP.Core.Dtos.PaymentDto
{
    public class VnPayCreatePaymentRequest
    {
        public int InvoiceId { get; set; }
        public decimal Amount { get; set; }
    }
}
