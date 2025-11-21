using Health.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.DTOs.Dashboard
{
    public class DoctorPatientItemDto
    {
        public int PatientId { get; set; }
        public string PatientName { get; set; } = "";
        public int Age { get; set; }
        public GenderType? Gender { get; set; }
        public DateTime LastVisitDate { get; set; }
    }
}
