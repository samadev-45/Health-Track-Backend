using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.DTOs.Admin
{
    public class ChartPointDto
    {
        public DateTime Day { get; set; }
        public int Count { get; set; }
    }
}
