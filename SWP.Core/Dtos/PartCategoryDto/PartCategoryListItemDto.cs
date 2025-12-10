namespace SWP.Core.Dtos.PartCategoryDto
{
    /// <summary>
    /// DTO cho danh sách Part Category
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public class PartCategoryListItemDto
    {
        public int PartCategoryId { get; set; }
        public string? PartCategoryName { get; set; }
        public string? PartCategoryCode { get; set; }
        public string? PartCategoryDiscription { get; set; }
        public string? PartCategoryPhone { get; set; }
        public string? Status { get; set; }
    }
}



