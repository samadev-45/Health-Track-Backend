using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.DTOs
{
    public class ConsultationCreateDto
    {
        public int UserId { get; set; }
        // Accept either a JSON string or a strongly typed object serialized to JSON by client
        public string HealthValuesJson { get; set; } = "{}";
    }

}
