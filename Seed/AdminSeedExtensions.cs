using Microsoft.EntityFrameworkCore;
using WPFPoliclinic.Models;

namespace WAPI.Seed;

public static class AdminSeedExtensions
{
    public static async Task SeedAdminsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PoliclinicContext>();

        if (!await db.Admins.AnyAsync())
        {
            var admin = new Admin
            {
                Login = "admin",
                FirstName = "Иван",
                LastName = "Петров",
                CreatedAt = DateTime.Now,
                IsActive = true,
                Role = "superadmin"
            };
            admin.SetPassword("admin123");

            db.Admins.Add(admin);
            await db.SaveChangesAsync();
        }
    }
}