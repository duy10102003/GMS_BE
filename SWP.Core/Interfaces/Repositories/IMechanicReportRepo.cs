using SWP.Core.Dtos.MechanicReportDto;

namespace SWP.Core.Interfaces.Repositories
{
    public interface IMechanicReportRepo
    {
        /// <summary>
        /// Lấy báo cáo tổng hợp cho mechanic
        /// </summary>
        /// <param name="mechanicId">ID user của mechanic</param>
        Task<MechanicReportSummaryDto?> GetSummaryAsync(int mechanicId);
    }
}

