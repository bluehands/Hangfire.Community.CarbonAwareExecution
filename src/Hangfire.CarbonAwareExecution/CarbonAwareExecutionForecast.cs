using CarbonAwareComputing.ExecutionForecast;
using Hangfire;

namespace CarbonAwareComputing.Hangfire;

public class CarbonAwareExecutionForecast
{
    public static async Task<ExecutionTime> GetBestScheduleTime(DateTimeOffset earliestExecutionTime, DateTimeOffset latestExecutionTime, TimeSpan estimatedJobDuration)
    {
        try
        {
            var filter = GlobalJobFilters.Filters.FirstOrDefault(f => f.Instance is CarbonAwareOptions);
            var options = filter?.Instance as CarbonAwareOptions;
            if (options == null)
            {
                return ExecutionTime.NoForecast;
            }

            var provider = options.DataProvider;
            var location = options.ComputingLocation;
            return await provider.CalculateBestExecutionTime(location, earliestExecutionTime, latestExecutionTime - estimatedJobDuration, estimatedJobDuration);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return ExecutionTime.NoForecast;
        }
    }
}