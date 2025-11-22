using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.DTOs.Admin
{
    public class AbnormalMetricStatDto
    {
        public string DisplayName { get; set; } = null!;
        public int AbnormalCount { get; set; }
    }
}
