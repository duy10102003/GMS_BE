namespace SWP.Core.Dtos.VehicleDto
{
    /// <summary>
    /// DTO cho Vehicle trong select (với search)
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public class VehicleSelectDto
    {
        public int VehicleId { get; set; }
        public string VehicleName { get; set; } = string.Empty;
        public string VehicleLicensePlate { get; set; } = string.Empty;
        public int? CurrentKm { get; set; }
        public string? Make { get; set; }
        public string? Model { get; set; }
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
    }
}


