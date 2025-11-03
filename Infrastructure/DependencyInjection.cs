using Health.Application.Interfaces;
using Health.Application.Services;
using Health.Infrastructure.Data;
using Health.Infrastructure.Repositories;
using Health.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Health.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            // Database registration
            services.AddDbContext<HealthDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            // Repository pattern
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Infrastructure services
            services.AddScoped<IEmailSenderService, EmailSenderService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<NormalRangeService>();
            services.AddScoped<IConsultationService, ConsultationService>();
            
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IAppointmentService, AppointmentService>();


            return services;
        }
    }
}
