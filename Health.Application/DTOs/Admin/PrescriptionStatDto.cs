using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.DTOs.Admin
{
    public class PrescriptionStatDto
    {
        public string Month { get; set; } = null!;
        public int Count { get; set; }
    }
}
