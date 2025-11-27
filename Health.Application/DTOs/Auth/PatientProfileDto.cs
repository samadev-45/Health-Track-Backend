using Health.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.DTOs.Auth
{
    public class PatientProfileDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public DateTime? DateOfBirth { get; set; }
        public GenderType Gender { get; set; }
        public int? BloodTypeId { get; set; }
        public string? BloodTypeName { get; set; }
        public string? Address { get; set; }
    }

}
