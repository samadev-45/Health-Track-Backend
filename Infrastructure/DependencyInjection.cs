using Health.Application.Interfaces;
using Health.Application.Interfaces.Dapper;
using Health.Application.Interfaces.EFCore;
using Health.Application.Services;
using Health.Infrastructure.Data;
using Health.Infrastructure.Repositories;
using Health.Infrastructure.Repositories.Dapper;
using Health.Infrastructure.Repositories.EFCore;
using Health.Infrastructure.Services;
using Health.Infrastructure.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Health.Application.Common;

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
            return services;
        }
    }
}
