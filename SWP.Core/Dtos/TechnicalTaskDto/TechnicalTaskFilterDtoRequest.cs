using SWP.Core.Dtos;

namespace SWP.Core.Dtos.TechnicalTaskDto
{
    /// <summary>
    /// DTO cho filter Technical Task
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public class TechnicalTaskFilterDtoRequest : PagedRequest
    {
        /// <summary>
        /// ID của technical staff (để lọc task của technical đó)
        /// </summary>
        public int? AssignedToTechnical { get; set; }

        /// <summary>
        /// Trạng thái task
        /// </summary>
        public byte? TaskStatus { get; set; }

        /// <summary>
        /// Trạng thái service ticket
        /// </summary>
        public byte? ServiceTicketStatus { get; set; }
    }
}

