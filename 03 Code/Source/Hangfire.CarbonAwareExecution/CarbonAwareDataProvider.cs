namespace Hangfire.CarbonAwareExecution;

public abstract class CarbonAwareDataProvider
{
    public abstract Task<DateTimeOffset> CalculateBestExecutionTime(DateTimeOffset earliestExecutionTime, DateTimeOffset latestExecutionTime, TimeSpan estimatedJobDuration);
}

