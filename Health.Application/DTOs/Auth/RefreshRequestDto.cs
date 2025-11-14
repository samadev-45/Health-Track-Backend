using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.DTOs.Auth
{
    public class RefreshRequestDto
    {
        public int UserId { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
    }
}
