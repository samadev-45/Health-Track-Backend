using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.DTOs
{
    public class ConsultationUpdateDto
    {
        public string? ChiefComplaint { get; set; }
        public string? Diagnosis { get; set; }
        public string? Advice { get; set; }
        public string? DoctorNotes { get; set; }
        public Dictionary<string, object>? HealthValues { get; set; }

        public DateTime? FollowUpDate { get; set; }
    }
}
