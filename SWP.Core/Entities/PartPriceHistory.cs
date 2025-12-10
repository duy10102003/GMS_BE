using SWP.Core.GMSAttribute;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP.Core.Entities
{
    [GMSTableName("part_price_history")]
    public class PartPriceHistory
    {
        [Column("part_id")]
        public int PartId { get; set; }

        [Column("history_price", TypeName = "decimal(24,2)")]
        public decimal? HistoryPrice { get; set; }

        [Column("date_history")]
        public DateTime? DateHistory { get; set; }

        [Column("modified_by")]
        public int? ModifiedBy { get; set; }

        [Column("is_deleted")]
        public int IsDeleted { get; set; } = 0;

        // Navigation properties
        public Part Part { get; set; } = null!;
        public User? ModifiedByUser { get; set; }
    }
}





