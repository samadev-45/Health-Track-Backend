using Health.Application.DTOs.File;
using Health.Application.Interfaces;
using Health.Application.Interfaces.EFCore;
using Health.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Health.Application.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IGenericRepository<FileStorage> _fileRepo;
        private readonly IGenericRepository<MedicalRecord> _recordRepo;
        private readonly IHttpContextAccessor _context;

        public FileStorageService(
            IGenericRepository<FileStorage> fileRepo,
            IGenericRepository<MedicalRecord> recordRepo,
            IHttpContextAccessor context)
        {
            _fileRepo = fileRepo;
            _recordRepo = recordRepo;
            _context = context;
        }

        private int CurrentUserId =>
            int.Parse(_context.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        public async Task<FileDownloadDto> DownloadAsync(int fileStorageId, CancellationToken ct = default)
        {
            // 1. Load file from FileStorage
            var file = await _fileRepo.GetByIdAsync(fileStorageId);
            if (file == null)
                throw new InvalidOperationException("File not found.");

            // 2. Try to find a MedicalRecord that owns this file
            var record = (await _recordRepo.FindAsync(r => r.FileStorageId == fileStorageId))
                                .FirstOrDefault();

            // 3. If file is part of a MedicalRecord → verify owner OR admin
            if (record != null)
            {
                if (record.UserId != CurrentUserId &&
                    !_context.HttpContext!.User.IsInRole("Admin"))
                    throw new UnauthorizedAccessException("You are not allowed to access this medical record file.");
            }
            else
            {
                // 4. If not part of MedicalRecord → fallback: check uploader
                if (file.UploadedByUserId != CurrentUserId &&
                    !_context.HttpContext!.User.IsInRole("Admin"))
                    throw new UnauthorizedAccessException("You cannot access this file.");
            }

            // 5. Return file content
            return new FileDownloadDto
            {
                FileBytes = file.FileData,
                FileName = file.FileName,
                ContentType = file.ContentType ?? "application/octet-stream"
            };
        }
    }
}
