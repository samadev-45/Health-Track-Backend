using Health.Application.DTOs.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.Interfaces.Dapper
{
    public interface IDashboardReadRepository
    {
        Task<DashboardSummaryDto> GetPatientDashboardAsync(int userId, DateTime? date = null, int upcomingDays = 7);
    }
}
