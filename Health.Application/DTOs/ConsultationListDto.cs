using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.DTOs
{
    public class ConsultationListDto
    {
        public int ConsultationId { get; set; }
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public DateTime? FollowUpDate { get; set; }
    }
}
