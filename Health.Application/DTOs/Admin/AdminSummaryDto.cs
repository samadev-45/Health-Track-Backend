using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.DTOs.Admin
{
    public class AdminSummaryDto
    {
        public int TotalUsers { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalPatients { get; set; }
        public int TodayAppointments { get; set; }
        public int WeekAppointments { get; set; }
        public int ActiveMedications { get; set; }
        public int TodayAbnormalMetrics { get; set; }
        public int TotalPrescriptions { get; set; }
    }
}
