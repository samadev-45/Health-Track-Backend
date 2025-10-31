using Health.Domain.Entities;
using Health.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Health.Infrastructure.Data
{
    public static class SeedData
    {
        public static async Task SeedAdminAsync(HealthDbContext context)
        {
            // Ensure BloodType table has data
            var bloodType = await context.BloodTypes
                .FirstOrDefaultAsync(bt => bt.Name == "O+");

            if (bloodType == null)
            {
                bloodType = new BloodType { Name = "O+" };
                await context.BloodTypes.AddAsync(bloodType);
                await context.SaveChangesAsync();
            }

            // Check if admin already exists
            if (await context.Users.AnyAsync(u => u.Role == RoleType.Admin))
                return;

            var passwordHasher = new PasswordHasher<User>();

            var adminUser = new User
            {
                FullName = "System Administrator",
                Email = "admin@healthtrack.com",
                PhoneNumber = "9999999999",
                DateOfBirth = new DateTime(1990, 1, 1),
                Gender = GenderType.Male,
                BloodTypeId = bloodType?.BloodTypeId ?? 1,  
                Address = "Head Office",
                EmergencyContactName = "Support",
                EmergencyContactPhone = "1234567890",
                Role = RoleType.Admin,
                CreatedOn = DateTime.UtcNow,
                IsActive = true,
                IsEmailVerified = true,
                Status = AccountStatus.Approved
            };

            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Admin@123");

            await context.Users.AddAsync(adminUser);
            await context.SaveChangesAsync();
        }
    }
}
