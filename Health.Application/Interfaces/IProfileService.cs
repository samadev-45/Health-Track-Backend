using Health.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.Interfaces
{
    public interface IProfileService
    {
        Task<PatientProfileDto> GetPatientProfileAsync(int userId, CancellationToken ct = default);
        Task<PatientProfileDto> UpdatePatientProfileAsync(int userId, UpdatePatientProfileDto dto, CancellationToken ct = default);
    }
}
