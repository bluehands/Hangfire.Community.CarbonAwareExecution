
using CarbonAwareComputing;

namespace Hangfire.Community.CarbonAwareExecution;

public class CarbonAwareExecutionForecast
{
    public static Task<ExecutionTime> GetBestScheduleTime(
        DateTimeOffset earliestExecutionTime,
        DateTimeOffset latestExecutionTime,
        TimeSpan estimatedJobDuration,
        Action<string> logError)
    {
        try
        {
            var filter = GlobalJobFilters.Filters.FirstOrDefault(f => f.Instance is CarbonAwareOptions);
            var options = filter?.Instance as CarbonAwareOptions;
            if (options == null)
            {
                return Task.FromResult(ExecutionTime.NoForecast);
            }

            var provider = options.DataProvider;
            var location = options.ComputingLocation;
            return provider.CalculateBestExecutionTime(location, earliestExecutionTime, latestExecutionTime - estimatedJobDuration, estimatedJobDuration);
        }
        catch (Exception ex)
        {
            logError?.Invoke(ex.Message);
            return Task.FromResult(ExecutionTime.NoForecast);
        }
    }
}