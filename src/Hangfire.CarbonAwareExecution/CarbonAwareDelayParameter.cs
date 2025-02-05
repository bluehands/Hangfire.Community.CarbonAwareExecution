namespace Hangfire.Community.CarbonAwareExecution;

public sealed record CarbonAwareDelayParameter(TimeSpan MaxExecutionDelay, TimeSpan EstimatedJobDuration)
{
    public string? ParentRecurringJobId { get; set; }
    public string JobUniqueId { get; set; } = Guid.NewGuid().ToString("N");
}