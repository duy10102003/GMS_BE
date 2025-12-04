namespace SWP.Core.Dtos.GarageServiceDto
{
    /// <summary>
    /// DTO cho Garage Service trong select (với search)
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public class GarageServiceSelectDto
    {
        public int GarageServiceId { get; set; }
        public string? GarageServiceName { get; set; }
        public decimal? GarageServicePrice { get; set; }
    }
}

