namespace Hangfire.CarbonAwareExecution;

public record DataBoundary(DateTimeOffset StartTime, DateTimeOffset EndTime);