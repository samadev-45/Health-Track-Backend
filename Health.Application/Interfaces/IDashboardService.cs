using Health.Application.DTOs.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetPatientDashboardAsync(
            DateTime? date = null,
            int upcomingDays = 7,
            CancellationToken ct = default);

    }
}
