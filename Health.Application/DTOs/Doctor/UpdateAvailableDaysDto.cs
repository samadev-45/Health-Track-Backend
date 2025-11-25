using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.DTOs.Doctor
{
    public class UpdateAvailableDaysDto
    {
        public List<string> Days { get; set; } = new();
    }
}
