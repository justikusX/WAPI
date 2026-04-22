using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WAPI.Options;
using WPFPoliclinic.Models;

namespace WAPI.Services.Hosted;

public sealed class DatabasePingService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DatabasePingService> _logger;
    private readonly ResilienceOptions _options;

    public DatabasePingService(
        IServiceScopeFactory scopeFactory,
        ILogger<DatabasePingService> logger,
        IOptions<ResilienceOptions> options)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(_options.PingIntervalSeconds));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<PoliclinicContext>();

                var canConnect = await db.Database.CanConnectAsync(stoppingToken);

                if (canConnect)
                    _logger.LogInformation("Database ping succeeded");
                else
                    _logger.LogWarning("Database ping failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database ping service error");
            }

            await timer.WaitForNextTickAsync(stoppingToken);
        }
    }
}