using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.DTOs
{
    public class CaretakerEmailOtpRequestDto
    {
        public string Email { get; set; } = null!;
        public string FullName { get; set; }
    }
}
