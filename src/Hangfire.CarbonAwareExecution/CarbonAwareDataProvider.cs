namespace Hangfire.CarbonAwareExecution;

public abstract class CarbonAwareDataProvider
{
    public abstract Task<(bool ForecastIsAvailable, DateTimeOffset BestExecutionTime)> CalculateBestExecutionTime(DateTimeOffset earliestExecutionTime, DateTimeOffset latestExecutionTime, TimeSpan estimatedJobDuration);
}

