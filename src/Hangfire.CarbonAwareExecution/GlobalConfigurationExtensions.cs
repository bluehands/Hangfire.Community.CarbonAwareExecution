using Hangfire.Common;

namespace Hangfire.CarbonAwareExecution
{
    public static class GlobalConfigurationExtensions
    {
        public static IGlobalConfiguration UseCarbonAwareExecution(this IGlobalConfiguration configuration, CarbonAwareDataProvider dataProvider)
        {
            var options = new CarbonAwareOptions(dataProvider);

            GlobalJobFilters.Filters.Add(options);
            return configuration;
        }
    }
}