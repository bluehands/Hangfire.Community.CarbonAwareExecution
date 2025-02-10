using CarbonAwareComputing;
using Hangfire.Community.CarbonAwareExecution;

// ReSharper disable once CheckNamespace
namespace Hangfire
{
    public static class GlobalConfigurationExtensions
    {
        public static IGlobalConfiguration UseCarbonAwareExecution(this IGlobalConfiguration configuration, CarbonAwareDataProvider dataProvider, ComputingLocation location)
        {
            var options = new CarbonAwareOptions(dataProvider, location);

            GlobalJobFilters.Filters.Add(options);
            GlobalJobFilters.Filters.Add(new ShiftJobCarbonAwareFilter());
            return configuration;
        }
        public static IGlobalConfiguration UseCarbonAwareExecution(this IGlobalConfiguration configuration, Func<CarbonAwareExecutionOptions> configure)
        {
            var o = configure.Invoke();
            var options = new CarbonAwareOptions(o.DataProvider, o.Location);

            GlobalJobFilters.Filters.Add(options);
            GlobalJobFilters.Filters.Add(new ShiftJobCarbonAwareFilter());
            return configuration;
        }
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed record CarbonAwareExecutionOptions(CarbonAwareDataProvider DataProvider, ComputingLocation Location);
}