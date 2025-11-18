using AutoMapper;
using Health.Application.DTOs.Common;
using Health.Application.DTOs.Medication;
using Health.Application.Interfaces;
using Health.Application.Interfaces.Dapper;
using Health.Application.Interfaces.EFCore;
using Health.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

public class MedicationService : IMedicationService
{
    private readonly IGenericRepository<Medication> _medRepo;
    private readonly IGenericRepository<MedicationReminder> _reminderRepo;
    private readonly IMedicationReadRepository _readRepo;
    private readonly IHttpContextAccessor _context;
    private readonly IMapper _mapper;

    public MedicationService(
        IGenericRepository<Medication> medRepo,
        IGenericRepository<MedicationReminder> reminderRepo,
        IMedicationReadRepository readRepo,
        IHttpContextAccessor context,
        IMapper mapper)
    {
        _medRepo = medRepo;
        _reminderRepo = reminderRepo;
        _readRepo = readRepo;
        _context = context;
        _mapper = mapper;
    }

    private int CurrentUserId =>
        int.Parse(_context.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    public async Task<MedicationDto> AddMedicationAsync(MedicationCreateDto dto, CancellationToken ct = default)
    {
        var med = new Medication
        {
            UserId = CurrentUserId,
            Name = dto.Name,
            DosageValue = dto.DosageValue,
            UnitId = (int)dto.UnitId,
            Frequency = dto.Frequency,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            DoseRangeLow = dto.DoseRangeLow,
            DoseRangeHigh = dto.DoseRangeHigh,
            DoseRangeUnitId = dto.DoseRangeUnitId,
            Instructions = dto.Instructions,
            CreatedOn = DateTime.UtcNow
        };

        await _medRepo.AddAsync(med);

        var result = _mapper.Map<MedicationDto>(med);
        result.RemainingDays = med.EndDate != null
            ? (long?)(med.EndDate.Value.Date - DateTime.UtcNow.Date).TotalDays
            : null;

        return result;
    }

    public async Task<MedicationDto> UpdateMedicationAsync(int id, MedicationUpdateDto dto, CancellationToken ct = default)
    {
        var med = await _medRepo.GetByIdAsync(id)
            ?? throw new InvalidOperationException("Medication not found");

        if (med.UserId != CurrentUserId && !_context.HttpContext!.User.IsInRole("Admin"))
            throw new UnauthorizedAccessException("You cannot edit this item");

        med.Name = dto.Name ?? med.Name;
        med.DosageValue = dto.DosageValue ?? med.DosageValue;
        med.UnitId = dto.UnitId ?? med.UnitId;
        med.Frequency = dto.Frequency ?? med.Frequency;
        med.StartDate = dto.StartDate ?? med.StartDate;
        med.EndDate = dto.EndDate ?? med.EndDate;
        med.DoseRangeLow = dto.DoseRangeLow ?? med.DoseRangeLow;
        med.DoseRangeHigh = dto.DoseRangeHigh ?? med.DoseRangeHigh;
        med.DoseRangeUnitId = dto.DoseRangeUnitId ?? med.DoseRangeUnitId;
        med.Instructions = dto.Instructions ?? med.Instructions;
        med.ModifiedOn = DateTime.UtcNow;

        await _medRepo.UpdateAsync(med);

        var result = _mapper.Map<MedicationDto>(med);
        result.RemainingDays = med.EndDate != null
            ? (long?)(med.EndDate.Value.Date - DateTime.UtcNow.Date).TotalDays
            : null;

        return result;
    }

    public async Task<bool> DeleteMedicationAsync(int id, CancellationToken ct = default)
    {
        var med = await _medRepo.GetByIdAsync(id)
            ?? throw new InvalidOperationException("Medication not found");

        if (med.UserId != CurrentUserId && !_context.HttpContext!.User.IsInRole("Admin"))
            throw new UnauthorizedAccessException("Not allowed");

        await _medRepo.DeleteAsync(med);

        return true;
    }

    public async Task<PagedResult<MedicationListDto>> GetMyMedicationsAsync(int page = 1, int pageSize = 20, CancellationToken ct = default)
    {
        return await _readRepo.GetForUserAsync(CurrentUserId, page, pageSize);
    }

    public async Task<IEnumerable<MedicationScheduleItemDto>> GetScheduleForTodayAsync(CancellationToken ct = default)
    {
        return await _readRepo.GetScheduleForUserAsync(CurrentUserId, DateTime.UtcNow.Date);
    }

    public async Task<int> GetActiveMedicationsCountAsync(CancellationToken ct = default)
    {
        return await _readRepo.GetActiveMedicationsCountAsync(CurrentUserId);
    }

    public async Task<MedicationDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var med = await _medRepo.GetByIdAsync(id);
        if (med == null) return null;

        if (med.UserId != CurrentUserId && !_context.HttpContext!.User.IsInRole("Admin"))
            throw new UnauthorizedAccessException("Unauthorized");

        var dto = _mapper.Map<MedicationDto>(med);
        dto.RemainingDays = med.EndDate != null
            ? (long?)(med.EndDate.Value.Date - DateTime.UtcNow.Date).TotalDays
            : null;

        return dto;
    }
}
