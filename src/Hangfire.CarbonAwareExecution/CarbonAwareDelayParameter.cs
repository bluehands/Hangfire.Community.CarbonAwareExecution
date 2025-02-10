namespace Hangfire.Community.CarbonAwareExecution;

public sealed record CarbonAwareExecution(TimeSpan MaxExecutionDelay, TimeSpan EstimatedJobDuration)
{
    public static CarbonAwareExecution ShiftCarbonAware(TimeSpan maxExecutionDelay, TimeSpan estimatedJobDuration)
        => new(maxExecutionDelay, estimatedJobDuration);
}