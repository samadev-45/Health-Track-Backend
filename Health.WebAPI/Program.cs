using AutoMapper;
using Health.Application.Common;
using Health.Application.Configuration;
using Health.Application.Helpers;
using Health.Application.Interfaces;
using Health.Application.Services;
using Health.Infrastructure;
using Health.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Infrastructure;
using System.Text;
using System.Text.Json;


namespace Health.WebAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;
            builder.Services.AddHttpContextAccessor();
            // Database Configuration
            builder.Services.AddDbContext<HealthDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("Health.Infrastructure")));

            // Controllers + Swagger
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new() { Title = "HealthTrack API", Version = "v1" });

                // Add Bearer Authentication to Swagger
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your token.\n\nExample: **Bearer eyJhbGciOi...**"
                });

                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
            }
            });
            });
            //QuestPdf Free
            QuestPDF.Settings.License = LicenseType.Community;

            // AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            var rangesPath = Path.Combine(builder.Environment.ContentRootPath, "Data", "NormalRanges.json");

            if (!File.Exists(rangesPath))
                throw new FileNotFoundException("NormalRanges.json missing.", rangesPath);

            var json = File.ReadAllText(rangesPath);
            var normalRanges = JsonSerializer.Deserialize<Dictionary<string, decimal>>(json)!;

            // Register as Singleton
            builder.Services.AddSingleton(normalRanges);

            // Register HealthMetricEngine using the injected dictionary
            builder.Services.AddSingleton<HealthMetricEngine>();

            // in Program.cs or Startup.cs
            builder.Services.Configure<AppointmentPolicyConfig>(configuration.GetSection("AppointmentPolicy"));

            // Infrastructure Dependencies (Repositories, Email, OTP, etc.)
            builder.Services.AddInfrastructure(configuration);

            // Application-level Services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<JwtHelper>();

            // JWT Authentication
            // JWT Authentication
            var jwtSection = configuration.GetSection("Jwt");
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = true;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtSection["Issuer"],
                        ValidateAudience = true,
                        ValidAudience = jwtSection["Audience"],
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSection["Key"]!)
                        ),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromSeconds(30),

                        // 🔹 These two lines FIX user id & role claims mapping
                        NameClaimType = System.Security.Claims.ClaimTypes.NameIdentifier,
                        RoleClaimType = System.Security.Claims.ClaimTypes.Role
                    };
                });


            builder.Services.AddAuthorization();

            var app = builder.Build();

            // 🔹 Apply migrations & seed admin
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<HealthDbContext>();
                await context.Database.MigrateAsync();
                await SeedData.SeedAdminAsync(context);
            }

            // Middleware
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            await app.RunAsync();
        }
    }
}
