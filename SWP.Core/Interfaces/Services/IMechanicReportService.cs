using SWP.Core.Dtos.MechanicReportDto;

namespace SWP.Core.Interfaces.Services
{
    public interface IMechanicReportService
    {
        /// <summary>
        /// Mechanic xem báo cáo tổng hợp của chính mình
        /// </summary>
        /// <param name="mechanicId">ID user của mechanic đang đăng nhập</param>
        Task<MechanicReportSummaryDto> GetMySummaryAsync(int mechanicId);
    }
}

