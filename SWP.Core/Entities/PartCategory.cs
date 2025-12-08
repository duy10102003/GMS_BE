using SWP.Core.GMSAttribute;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP.Core.Entities
{
    [GMSTableName("part_category")]
    public class PartCategory
    {
        [Column("part_category_id")]
        [Key]
        public int PartCategoryId { get; set; }

        [Column("part_category_name")]
        [MaxLength(100)]
        public string? PartCategoryName { get; set; }

        [Column("part_category_code")]
        [MaxLength(20)]
        public string? PartCategoryCode { get; set; }

        [Column("part_category_discription")]
        [MaxLength(255)]
        public string? PartCategoryDiscription { get; set; }

        [Column("part_category_phone")]
        [MaxLength(50)]
        public string? PartCategoryPhone { get; set; }

        [Column("status")]
        [MaxLength(20)]
        public string? Status { get; set; }

        //
        public ICollection<Part> Parts { get; set; } = new List<Part>();
    }
}


