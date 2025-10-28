using System.ComponentModel.DataAnnotations;
using Health.Domain.Enums;
using System;

public class RegisterDto
{
    [Required, MaxLength(100)]
    public string FullName { get; set; } = null!;

    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    [Required, MinLength(6)]
    public string Password { get; set; } = null!;

    [Phone]
    public string? PhoneNumber { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public GenderType? Gender { get; set; }

    [MaxLength(10)]
    public string? BloodType { get; set; }

    [MaxLength(250)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? EmergencyContactName { get; set; }

    [Phone]
    public string? EmergencyContactPhone { get; set; }

    public RoleType Role { get; set; } = RoleType.Patient;

    // Doctor-only fields
    public int? SpecialtyId { get; set; }
    public string? LicenseNumber { get; set; }
}
