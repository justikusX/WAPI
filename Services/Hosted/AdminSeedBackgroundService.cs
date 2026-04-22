using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WAPI.Options;
using WPFPoliclinic.Models;

namespace WAPI.Services.Hosted;

public sealed class AdminSeedBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AdminSeedBackgroundService> _logger;
    private readonly ResilienceOptions _options;

    public AdminSeedBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<AdminSeedBackgroundService> logger,
        IOptions<ResilienceOptions> options)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<PoliclinicContext>();

                if (!await db.Database.CanConnectAsync(stoppingToken))
                {
                    _logger.LogWarning("Database unavailable. Admin seeding postponed.");
                    await Task.Delay(TimeSpan.FromSeconds(_options.SeedRetryDelaySeconds), stoppingToken);
                    continue;
                }

                if (!await db.Admins.AnyAsync(stoppingToken))
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
                    await db.SaveChangesAsync(stoppingToken);

                    _logger.LogInformation("Default admin created successfully");
                }
                else
                {
                    _logger.LogInformation("Admin already exists, seeding skipped");
                }

                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Admin seeding failed. Next retry later.");
                await Task.Delay(TimeSpan.FromSeconds(_options.SeedRetryDelaySeconds), stoppingToken);
            }
        }
    }
}