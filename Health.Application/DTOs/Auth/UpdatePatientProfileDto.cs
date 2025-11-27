using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.DTOs.Auth
{
    public class UpdatePatientProfileDto
    {
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public int? BloodTypeId { get; set; }
        public string? Address { get; set; }
    }
}
