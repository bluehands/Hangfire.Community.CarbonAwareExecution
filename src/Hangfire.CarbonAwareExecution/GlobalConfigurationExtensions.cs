using CarbonAwareComputing.ExecutionForecast;
using Hangfire.Common;

namespace Hangfire.CarbonAwareExecution
{
    public static class GlobalConfigurationExtensions
    {
        public static IGlobalConfiguration UseCarbonAwareExecution(this IGlobalConfiguration configuration, CarbonAwareDataProvider dataProvider, ComputingLocation location)
        {
            var options = new CarbonAwareOptions(dataProvider, location);

            GlobalJobFilters.Filters.Add(options);
            return configuration;
        }
    }
}