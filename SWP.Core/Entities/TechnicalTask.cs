using SWP.Core.GMSAttribute;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP.Core.Entities
{
    [GMSTableName("technical_task")]
    public class TechnicalTask
    {
        [Column("technical_task_id")]
        [Key]
        public Guid TechnicalTaskId { get; set; }

        [Column("service_ticket_id")]
        [Required]
        public Guid ServiceTicketId { get; set; }

        [Column("description")]
        [Required]
        [MaxLength(255)]
        public string Description { get; set; } = string.Empty;

        [Column("assigned_to_technical")]
        public Guid? AssignedToTechnical { get; set; }

        [Column("assigned_at")]
        public DateTime? AssignedAt { get; set; }

        [Column("task_status")]
        public byte? TaskStatus { get; set; }

        [Column("confirmed_by")]
        public Guid? ConfirmedBy { get; set; }

        [Column("confirmed_at")]
        public DateTime? ConfirmedAt { get; set; }

        // Navigation properties
        public ServiceTicket ServiceTicket { get; set; } = null!;
        public User? AssignedToTechnicalUser { get; set; }
        public User? ConfirmedByUser { get; set; }
    }
}

