using AutoMapper;
using Health.Application.DTOs.Common;
using Health.Application.DTOs.HealthMetric;
using Health.Application.Interfaces;
using Health.Application.Interfaces.Dapper;
using Health.Application.Interfaces.EFCore;
using Health.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Health.Application.Services
{
    public class HealthMetricService : IHealthMetricService
    {
        private readonly IGenericRepository<HealthMetric> _metricRepo;
        private readonly IGenericRepository<MetricType> _metricTypeRepo;
        private readonly IHealthMetricReadRepository _readRepo;
        private readonly IHttpContextAccessor _context;
        private readonly IMapper _mapper;

        public HealthMetricService(
            IGenericRepository<HealthMetric> metricRepo,
            IGenericRepository<MetricType> metricTypeRepo,
            IHealthMetricReadRepository readRepo,
            IHttpContextAccessor context,
            IMapper mapper)
        {
            _metricRepo = metricRepo;
            _metricTypeRepo = metricTypeRepo;
            _readRepo = readRepo;
            _context = context;
            _mapper = mapper;
        }

        private int CurrentUserId =>
            int.Parse(_context.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

       
        public async Task<HealthMetricDetailsDto> CreateAsync(CreateHealthMetricDto dto, CancellationToken ct = default)
        {
            int userId = CurrentUserId;

            var metricType = await _metricTypeRepo.GetByIdAsync(dto.MetricTypeId)
                ?? throw new InvalidOperationException("Metric type not found.");

            bool isAbnormal = false;
            if (metricType.MinValue.HasValue && dto.Value < metricType.MinValue.Value)
                isAbnormal = true;
            if (metricType.MaxValue.HasValue && dto.Value > metricType.MaxValue.Value)
                isAbnormal = true;

            var entity = new HealthMetric
            {
                UserId = userId,
                MetricTypeId = dto.MetricTypeId,
                Value = dto.Value,
                MeasuredAt = dto.MeasuredAt ?? DateTime.UtcNow,
                Notes = dto.Notes,
                IsAbnormal = isAbnormal,
                CreatedOn = DateTime.UtcNow
            };

            await _metricRepo.AddAsync(entity);

            return new HealthMetricDetailsDto
            {
                HealthMetricId = entity.HealthMetricId,
                MetricTypeId = metricType.MetricTypeId,
                MetricCode = metricType.MetricCode,
                DisplayName = metricType.DisplayName,
                Unit = metricType.Unit,
                Value = entity.Value,
                MeasuredAt = entity.MeasuredAt,
                Notes = entity.Notes,
                IsAbnormal = entity.IsAbnormal
            };
        }

       
        public async Task<HealthMetricDetailsDto> UpdateAsync(int id, HealthMetricUpdateDto dto, CancellationToken ct = default)
        {
            var entity = await _metricRepo.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Metric not found");

            if (entity.UserId != CurrentUserId && !_context.HttpContext!.User.IsInRole("Admin"))
                throw new UnauthorizedAccessException("Unauthorized");

            // Update fields
            entity.Value = dto.Value ?? entity.Value;
            entity.MeasuredAt = dto.MeasuredAt ?? entity.MeasuredAt;
            entity.Notes = dto.Notes ?? entity.Notes;

            // Load metric type
            var metricType = await _metricTypeRepo.GetByIdAsync(entity.MetricTypeId)
                ?? throw new InvalidOperationException("Metric type missing.");

            bool isAbnormal = false;

            if (metricType.MinValue.HasValue && entity.Value < metricType.MinValue.Value)
                isAbnormal = true;
            if (metricType.MaxValue.HasValue && entity.Value > metricType.MaxValue.Value)
                isAbnormal = true;

            entity.IsAbnormal = isAbnormal;
            entity.ModifiedOn = DateTime.UtcNow;

            await _metricRepo.UpdateAsync(entity);

            return new HealthMetricDetailsDto
            {
                HealthMetricId = entity.HealthMetricId,
                MetricTypeId = metricType.MetricTypeId,
                MetricCode = metricType.MetricCode,
                DisplayName = metricType.DisplayName,
                Unit = metricType.Unit,
                Value = entity.Value,
                MeasuredAt = entity.MeasuredAt,
                Notes = entity.Notes,
                IsAbnormal = entity.IsAbnormal
            };
        }


        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _metricRepo.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Metric not found");

            if (entity.UserId != CurrentUserId && !_context.HttpContext!.User.IsInRole("Admin"))
                throw new UnauthorizedAccessException("Unauthorized");

            await _metricRepo.DeleteAsync(entity);
            return true;
        }

        public Task<PagedResult<HealthMetricListDto>> GetForUserAsync(
            int page, int pageSize, int? metricTypeId, DateTime? from, DateTime? to, CancellationToken ct = default)
        {
            return _readRepo.GetForUserAsync(CurrentUserId, page, pageSize, metricTypeId, from, to);
        }

        
        public async Task<HealthMetricDetailsDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var dto = await _readRepo.GetByIdAsync(id);
            return dto;
        }

        
        public Task<IEnumerable<HealthMetricListDto>> GetTodayAbnormalAsync(CancellationToken ct = default)
        {
            return _readRepo.GetTodayAbnormalAsync(CurrentUserId, DateTime.UtcNow.Date);
        }
        public async Task<MetricTrendDto> GetTrendAsync(int metricTypeId, int days, CancellationToken ct = default)
        {
            int userId = CurrentUserId;

            
            var metricType = await _metricTypeRepo.GetByIdAsync(metricTypeId)
                ?? throw new InvalidOperationException("Metric type not found");

          
            var history = await _readRepo.GetTrendHistoryAsync(userId, metricTypeId, days);

            
            var points = history
                .OrderBy(x => x.MeasuredAt)
                .Select(x => new TrendPoint
                {
                    MeasuredAt = x.MeasuredAt,
                    Value = x.Value,
                    IsAbnormal = x.IsAbnormal
                })
                .ToList();

            bool hasAbnormal = points.Any(p => p.IsAbnormal);

            
            return new MetricTrendDto
            {
                MetricTypeId = metricTypeId,
                MetricCode = metricType.MetricCode,
                DisplayName = metricType.DisplayName,
                Unit = metricType.Unit,
                MinValue = metricType.MinValue,
                MaxValue = metricType.MaxValue,
                Points = points,
                HasAbnormalReadings = hasAbnormal
            };
        }


    }
}
