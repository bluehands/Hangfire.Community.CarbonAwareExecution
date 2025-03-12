
using CarbonAwareComputing;
using Hangfire.Community.CarbonAwareExecution.Internal;
using Microsoft.Extensions.Logging;

namespace Hangfire.Community.CarbonAwareExecution;

public static class CarbonAwareExecutionForecastExtensions
{
    internal static async Task<ExecutionTime> GetBestScheduleTime(this CarbonAwareServices services, ComputingLocation location, DateTimeOffset earliestExecutionTime, DateTimeOffset latestExecutionTime, TimeSpan estimatedJobDuration)
    {
        var provider = services.DataProvider.Invoke();
        try
        {
            var bestExecutionTime = await provider.DataProvider.CalculateBestExecutionTime(location,
                earliestExecutionTime, latestExecutionTime - estimatedJobDuration, estimatedJobDuration);
            return bestExecutionTime;
        }
        catch (Exception ex)
        {
            services.Logger?.LogWarning(ex, $"Failed to get forecast for location {location}");
            return ExecutionTime.NoForecast;
        }
        finally
        {
            provider.Scope?.Dispose();
        }
    }
}