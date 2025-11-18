using AutoMapper;
using Health.Application.DTOs.Common;
using Health.Application.DTOs.MedicalRecord;
using Health.Application.Interfaces;
using Health.Application.Interfaces.Dapper;
using Health.Application.Interfaces.EFCore;
using Health.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Health.Application.Services
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IGenericRepository<MedicalRecord> _recordRepo;
        private readonly IMedicalRecordReadRepository _readRepo;
        private readonly IGenericRepository<FileStorage> _fileRepo;
        private readonly IHttpContextAccessor _context;
        private readonly IMapper _mapper;

        public MedicalRecordService(
            IGenericRepository<MedicalRecord> recordRepo,
            IMedicalRecordReadRepository readRepo,
            IGenericRepository<FileStorage> fileRepo,
            IHttpContextAccessor context,
            IMapper mapper)
        {
            _recordRepo = recordRepo;
            _readRepo = readRepo;
            _fileRepo = fileRepo;
            _context = context;
            _mapper = mapper;
        }

        private int GetCurrentUserId()
        {
            var userId = _context.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException("User not logged in.");
            return int.Parse(userId);
        }

        // ---------------------------- CREATE ----------------------------
        public async Task<MedicalRecordDto> CreateAsync(CreateMedicalRecordDto dto, CancellationToken ct = default)
        {
            int currentUserId = GetCurrentUserId();

            byte[] bytes = Convert.FromBase64String(dto.Base64Data);

            var file = new FileStorage
            {
                FileName = dto.FileName,
                FileExtension = Path.GetExtension(dto.FileName),
                ContentType = dto.ContentType,
                FileData = bytes,
                FileSize = dto.FileSize,
                UploadedByUserId = currentUserId,
                CreatedOn = DateTime.UtcNow
            };

            await _fileRepo.AddAsync(file);   // FIX ✔


            var record = new MedicalRecord
            {
                UserId = currentUserId,
                RecordTypeId = dto.RecordTypeId,
                Title = dto.Title,
                Description = dto.Description,
                RecordDate = dto.RecordDate,
                DoctorName = dto.DoctorName,
                Hospital = dto.Hospital,
                FileStorageId = file.FileStorageId
            };

            await _recordRepo.AddAsync(record);   // FIX ✔


            var result = _mapper.Map<MedicalRecordDto>(record);

            // Manual mapping for file info
            result.FileName = file.FileName;
            result.FileSize = file.FileSize;
            result.ContentType = file.ContentType;

            return result;
        }

        // ---------------------------- LIST ----------------------------
        public async Task<PagedResult<MedicalRecordListDto>> GetMyRecordsAsync(
            int page, int pageSize, int? recordTypeId, DateTime? from, DateTime? to, CancellationToken ct = default)
        {
            int currentUserId = GetCurrentUserId();

            return await _readRepo.GetForUserAsync(
                currentUserId,
                page,
                pageSize,
                recordTypeId,
                from,
                to
            );
        }

        // ---------------------------- DETAILS ----------------------------
        public async Task<MedicalRecordDto?> GetByIdAsync(int recordId, CancellationToken ct = default)
        {
            var dto = await _readRepo.GetByIdAsync(recordId);
            if (dto == null) return null;

            int currentUserId = GetCurrentUserId();
            if (dto.UserId != currentUserId && !_context.HttpContext.User.IsInRole("Admin"))
                throw new UnauthorizedAccessException("You are not allowed to view this record.");

            return dto;
        }

        // ---------------------------- DELETE ----------------------------
        public async Task<bool> DeleteAsync(int recordId, CancellationToken ct = default)
        {
            var record = await _recordRepo.GetByIdAsync(recordId)
                ?? throw new InvalidOperationException("Record not found.");

            int currentUserId = GetCurrentUserId();
            if (record.UserId != currentUserId && !_context.HttpContext.User.IsInRole("Admin"))
                throw new UnauthorizedAccessException("You cannot delete this record.");

            if (record.FileStorageId != null)
            {
                var file = await _fileRepo.GetByIdAsync(record.FileStorageId.Value);
                if (file != null)
                    await _fileRepo.DeleteAsync(file);
            }

            await _recordRepo.DeleteAsync(record);
            return true;
        }
    }
}
