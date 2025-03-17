
using CarbonAwareComputing;
using Hangfire.Community.CarbonAwareExecution.Internal;
using Microsoft.Extensions.Logging;

namespace Hangfire.Community.CarbonAwareExecution;

public static class CarbonAwareExecutionForecastExtensions
{
    internal static async Task<ShiftInfoJobParameter?> GetBestScheduleTime(this CarbonAwareServices services, ComputingLocation location, DateTimeOffset earliestExecutionTime, DateTimeOffset latestExecutionTime, TimeSpan estimatedJobDuration)
    {
        var provider = services.DataProvider.Invoke();
        try
        {
            var bestExecutionTime = await provider.DataProvider.CalculateBestExecutionTime(location,
                earliestExecutionTime, latestExecutionTime, estimatedJobDuration);
            return bestExecutionTime
                .Match<ShiftInfoJobParameter?>(
                    noForecast: _ => null,
                    bestExecutionTime: b => new ShiftInfoJobParameter(
                        OriginalExecutionTime: new(earliestExecutionTime, b.CarbonIntensityAtEarliestExecutionTime),
                        ShiftedExecutionTime: new(b.ExecutionTime, b.CarbonIntensity))
                );


        }
        catch (Exception ex)
        {
            services.Logger?.LogWarning(ex, $"Failed to get forecast for location {location}");
            return null;
        }
        finally
        {
            provider.Scope?.Dispose();
        }
    }
}