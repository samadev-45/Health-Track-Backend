using Health.Application.Interfaces;
using Health.Application.Services;
using Health.Infrastructure.Data;
using Health.Infrastructure.Repositories;
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

            // Register repositories & services
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
