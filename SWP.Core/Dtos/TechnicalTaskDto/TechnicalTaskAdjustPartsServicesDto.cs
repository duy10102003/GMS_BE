using SWP.Core.Dtos.SeriveTicketDto;

namespace SWP.Core.Dtos.TechnicalTaskDto
{
    /// <summary>
    /// DTO cho technical staff điều chỉnh Parts và Services
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public class TechnicalTaskAdjustPartsServicesDto
    {
        /// <summary>
        /// Danh sách Parts mới (thay thế toàn bộ)
        /// </summary>
        public List<ServiceTicketAddPartDto>? Parts { get; set; }

        /// <summary>
        /// Danh sách Garage Services mới (thay thế toàn bộ)
        /// </summary>
        public List<ServiceTicketAddGarageServiceDto>? GarageServices { get; set; }
    }
}

