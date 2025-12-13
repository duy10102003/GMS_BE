namespace SWP.Core.Dtos.PartDto
{
    /// <summary>
    /// DTO cho select Part (dropdown)
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public class PartSelectDto
    {
        public int PartId { get; set; }
        public string PartName { get; set; } = string.Empty;
        public string PartCode { get; set; } = string.Empty;
        public int PartQuantity { get; set; }
        public string PartUnit { get; set; } = string.Empty;
        public int PartPrice { get; set; }
    }
}



