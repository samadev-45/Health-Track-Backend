using AutoMapper;
using Health.Application.DTOs;
using Health.Application.DTOs.Auth;
using Health.Application.Interfaces;
using Health.Application.Interfaces.EFCore;
using Health.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Health.Domain.Enums;

public class ProfileService : IProfileService
{
    private readonly IUserRepository _userRepo;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _context;

    public ProfileService(IUserRepository userRepo, IMapper mapper, IHttpContextAccessor context)
    {
        _userRepo = userRepo;
        _mapper = mapper;
        _context = context;
    }

    private int GetCurrentUserId()
    {
        var id = _context.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(id))
            throw new UnauthorizedAccessException("User not logged in.");
        return int.Parse(id);
    }

    public async Task<PatientProfileDto> GetPatientProfileAsync(int userId, CancellationToken ct = default)
    {
        var user = await _userRepo.GetByIdAsync(userId, ct)
            ?? throw new InvalidOperationException("User not found.");

        return _mapper.Map<PatientProfileDto>(user);
    }

    public async Task<PatientProfileDto> UpdatePatientProfileAsync(
        int userId,
        UpdatePatientProfileDto dto,
        CancellationToken ct = default)
    {
        var user = await _userRepo.GetByIdAsync(userId, ct)
            ?? throw new InvalidOperationException("User not found.");

        // Update allowed fields
        if (!string.IsNullOrWhiteSpace(dto.FullName)) user.FullName = dto.FullName;
        if (!string.IsNullOrWhiteSpace(dto.PhoneNumber)) user.PhoneNumber = dto.PhoneNumber;
        if (dto.DateOfBirth.HasValue) user.DateOfBirth = dto.DateOfBirth;
        if (!string.IsNullOrWhiteSpace(dto.Gender))
        {
            if (Enum.TryParse<GenderType>(dto.Gender, true, out var genderEnum))
            {
                user.Gender = genderEnum;
            }
            else
            {
                throw new InvalidOperationException("Invalid gender value.");
            }
        }

        if (dto.BloodTypeId.HasValue)
            user.BloodTypeId = dto.BloodTypeId;
        if (!string.IsNullOrWhiteSpace(dto.Address)) user.Address = dto.Address;

        await _userRepo.UpdateAsync(user, ct);

        return _mapper.Map<PatientProfileDto>(user);
    }


}
