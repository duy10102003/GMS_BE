using SWP.Core.GMSAttribute;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP.Core.Entities
{
    [GMSTableName("invoice")]
    public class Invoice
    {
        [Column("invoice_id")]
        [Key]
        public int InvoiceId { get; set; }

        [Column("service_ticket_id")]
        [Required]
        public int ServiceTicketId { get; set; }

        [Column("customer_id")]
        [Required]
        public int CustomerId { get; set; }

        [Column("issue_date")]
        public DateTime? IssueDate { get; set; }

        [Column("parts_amount", TypeName = "decimal(24,2)")]
        public decimal PartsAmount { get; set; } = 0.00m;

        [Column("garage_service_amount", TypeName = "decimal(24,2)")]
        public decimal GarageServiceAmount { get; set; } = 0.00m;

        [Column("tax_amount", TypeName = "decimal(24,2)")]
        public decimal TaxAmount { get; set; } = 0.00m;

        [Column("discount_amount", TypeName = "decimal(24,2)")]
        public decimal DiscountAmount { get; set; } = 0.00m;

        [Column("total_amount", TypeName = "decimal(24,2)")]
        [Required]
        public decimal TotalAmount { get; set; }

        [Column("invoice_status")]
        public byte? InvoiceStatus { get; set; }

        [Column("invoice_code")]
        [MaxLength(20)]
        public string? InvoiceCode { get; set; }

        [Column("is_deleted")]
        public int IsDeleted { get; set; } = 0;

        // 
        public ServiceTicket ServiceTicket { get; set; } = null!;
        public Customer Customer { get; set; } = null!;
    }
}
