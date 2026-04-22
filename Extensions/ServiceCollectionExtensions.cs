using Microsoft.EntityFrameworkCore;
using WAPI.Options;
using WAPI.Services.Hosted;
using WPFPoliclinic.Models;

namespace WAPI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProjectInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var resilienceSection = configuration.GetSection("Resilience");
        services.Configure<ResilienceOptions>(resilienceSection);

        var resilience = resilienceSection.Get<ResilienceOptions>() ?? new ResilienceOptions();

        services.AddDbContext<PoliclinicContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("Policlinic"),
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: resilience.MaxRetryCount,
                        maxRetryDelay: TimeSpan.FromSeconds(resilience.MaxRetryDelaySeconds),
                        errorNumbersToAdd: null);
                }));

        services.AddHealthChecks()
            .AddDbContextCheck<PoliclinicContext>(name: "sqlserver");

        services.AddHostedService<DatabasePingService>();
        services.AddHostedService<AdminSeedBackgroundService>();

        return services;
    }
}