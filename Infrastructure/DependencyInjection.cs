using Health.Application.Common;
using Health.Application.Interfaces;
using Health.Application.Interfaces.Dapper;
using Health.Application.Interfaces.EFCore;
using Health.Application.Services;
using Health.Domain.Entities;
using Health.Infrastructure.Data;
using Health.Infrastructure.Repositories;
using Health.Infrastructure.Repositories.Dapper;
using Health.Infrastructure.Repositories.EFCore;
using Health.Infrastructure.Services;
using Health.Infrastructure.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Health.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            //  Database context
            services.AddDbContext<HealthDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            // EF Core Repositories
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Dapper Repositories
            services.AddScoped<IAppointmentReadRepository, AppointmentReadRepository>();
            services.AddScoped<IAppointmentWriteRepository, AppointmentWriteRepository>();
            // this is your Dapper repo

            // Infrastructure-level services
            services.AddScoped<IEmailSenderService, EmailSenderService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IConsultationService, ConsultationService>();
            services.AddScoped<NormalRangeService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IConsultationWriteRepository, ConsultationRepository>();
            services.AddScoped<IConsultationReadRepository, ConsultationReadRepository>();
            services.AddScoped<IPdfGenerator, QuestPdfGenerator>();
            services.AddScoped<IMedicalRecordReadRepository, MedicalRecordReadRepository>();
            
            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddScoped<IMedicalRecordService, MedicalRecordService>();

            // EF write repos
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddSingleton<Application.Common.HealthMetricEngine>();
            services.AddScoped<IGenericRepository<Medication>, GenericRepository<Medication>>();
            services.AddScoped<IGenericRepository<MedicationReminder>, GenericRepository<MedicationReminder>>();

            services.AddScoped<IMedicationReadRepository, MedicationReadRepository>();
            services.AddScoped<IMedicationService, MedicationService>();

            services.AddScoped<IMedicalRecordService, MedicalRecordService>();
            services.AddScoped<IMedicalRecordReadRepository, MedicalRecordReadRepository>();
            services.AddScoped<IHealthMetricService, HealthMetricService>();

            services.AddScoped<IHealthMetricWriteRepository, HealthMetricWriteRepository>();
            services.AddScoped<IHealthMetricReadRepository, HealthMetricReadRepository>();

            services.AddScoped<IGenericRepository<HealthMetric>, GenericRepository<HealthMetric>>();
            services.AddScoped<IGenericRepository<MetricType>, GenericRepository<MetricType>>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IDashboardReadRepository, DashboardReadRepository>();
            services.AddScoped<IDoctorDashboardService, DoctorDashboardService>();
            services.AddScoped<IDoctorDashboardReadRepository, DoctorDashboardReadRepository>();
            services.AddScoped<IAdminAnalyticsReadRepository, AdminAnalyticsReadRepository>();
            services.AddScoped<IAdminAnalyticsService, AdminAnalyticsService>();

            return services;
        }
    }
}
