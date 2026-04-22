namespace WAPI.Options;

public sealed class ResilienceOptions
{
    public int MaxRetryCount { get; set; } = 5;
    public int MaxRetryDelaySeconds { get; set; } = 30;
    public int SeedRetryDelaySeconds { get; set; } = 10;
    public int PingIntervalSeconds { get; set; } = 60;
}