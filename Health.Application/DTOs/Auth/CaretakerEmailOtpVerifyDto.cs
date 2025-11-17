using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.DTOs.Auth
{
    public class CaretakerEmailOtpVerifyDto
    {
        public string Email { get; set; } = null!;
        public string Otp { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}
