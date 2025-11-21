using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.DTOs.Dashboard
{
    public class DoctorDashboardDto
    {
        public IEnumerable<DoctorAppointmentItemDto> TodayAppointments { get; set; }
            = new List<DoctorAppointmentItemDto>();

        public IEnumerable<DoctorAppointmentItemDto> UpcomingAppointments { get; set; }
            = new List<DoctorAppointmentItemDto>();

        public IEnumerable<DoctorConsultationItemDto> RecentConsultations { get; set; }
            = new List<DoctorConsultationItemDto>();

        public IEnumerable<DoctorPatientItemDto> Patients { get; set; }
            = new List<DoctorPatientItemDto>();

        public IEnumerable<AbnormalMetricDto> AbnormalMetrics { get; set; }
            = new List<AbnormalMetricDto>();

        public IEnumerable<NotificationDto> Notifications { get; set; }
            = new List<NotificationDto>();
    }
}
