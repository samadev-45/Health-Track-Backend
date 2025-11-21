using System.Collections.Generic;
using Health.Application.DTOs.Appointment;
using Health.Application.DTOs.Dashboard;
using Health.Application.DTOs.HealthMetric;
using Health.Application.DTOs.Medication;

namespace Health.Application.DTOs.Dashboard
{
    public class DashboardSummaryDto
    {
        public NextAppointmentDto? NextAppointment { get; set; }   // Next upcoming appointment
        public int ActiveMedicationCount { get; set; }             // Count of active meds
        public IEnumerable<AbnormalMetricDto> TodayAbnormalMetrics { get; set; }
            = new List<AbnormalMetricDto>();                       // List of abnormal vitals today
        public LatestVitalsDto LatestVitals { get; set; }
            = new LatestVitalsDto();                               // Most recent vitals
        public IEnumerable<NotificationDto> RecentNotifications { get; set; }
            = new List<NotificationDto>();                         // Last 10 notifications
        public int HealthScore { get; set; }                       // 0–100 calculated score
    }
}
