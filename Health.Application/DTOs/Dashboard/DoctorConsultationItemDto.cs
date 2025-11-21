using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.DTOs.Dashboard
{
    public class DoctorConsultationItemDto
    {
        public int ConsultationId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = "";
        public DateTime CreatedOn { get; set; }
        public string Diagnosis { get; set; } = "";
    }
}
