
using CarbonAwareComputing;
using Hangfire.Community.CarbonAwareExecution.Internal;
using Microsoft.Extensions.Logging;

namespace Hangfire.Community.CarbonAwareExecution;

public static class CarbonAwareExecutionForecast
{
    public static async Task<ExecutionTime> GetBestScheduleTime(DateTimeOffset earliestExecutionTime, DateTimeOffset latestExecutionTime, TimeSpan estimatedJobDuration, ILogger? logger = null)
    {
        var filter = GlobalJobFilters.Filters.FirstOrDefault(f => f.Instance is CarbonAwareOptions);
        if (filter?.Instance is not CarbonAwareOptions options)
        {
            return ExecutionTime.NoForecast;
        }

        var provider = options.DataProvider.Invoke();
        var location = options.ComputingLocation;
        try
        {
            var bestExecutionTime = await provider.DataProvider.CalculateBestExecutionTime(location,
                earliestExecutionTime, latestExecutionTime - estimatedJobDuration, estimatedJobDuration);
            return bestExecutionTime;

        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, $"Failed to get forecast for location {location}");
            return ExecutionTime.NoForecast;
        }
        finally
        {
            provider.Scope?.Dispose();
        }
    }
}