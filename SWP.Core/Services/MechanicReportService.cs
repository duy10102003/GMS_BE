using SWP.Core.Dtos.MechanicReportDto;
using SWP.Core.Interfaces.Repositories;
using SWP.Core.Interfaces.Services;

namespace SWP.Core.Services
{
    public class MechanicReportService : IMechanicReportService
    {
        private readonly IMechanicReportRepo _mechanicReportRepo;

        public MechanicReportService(IMechanicReportRepo mechanicReportRepo)
        {
            _mechanicReportRepo = mechanicReportRepo;
        }

        public async Task<MechanicReportSummaryDto> GetMySummaryAsync(int mechanicId)
        {
            var summary = await _mechanicReportRepo.GetSummaryAsync(mechanicId);

            if (summary == null)
            {
                return new MechanicReportSummaryDto
                {
                    MechanicId = mechanicId
                };
            }

            return summary;
        }
    }
}

