namespace SWP.Core.Dtos.PartDto
{
    /// <summary>
    /// DTO cho chi tiết Part
    /// Created by: DuyLC(02/12/2025)
    /// </summary>
    public class PartDetailDto
    {
        public int PartId { get; set; }
        public string PartName { get; set; } = string.Empty;
        public string PartCode { get; set; } = string.Empty;
        public int PartQuantity { get; set; }
        public string PartUnit { get; set; } = string.Empty;
        public int PartCategoryId { get; set; }
        public string PartCategoryName { get; set; } = string.Empty;
        public string PartCategoryCode { get; set; } = string.Empty;
        public string? PartCategoryDiscription { get; set; }
        public string? PartCategoryPhone { get; set; }
        public string? Status { get; set; }
        public decimal? PartPrice { get; set; }
        public int? WarrantyMonth { get; set; }
    }
}

