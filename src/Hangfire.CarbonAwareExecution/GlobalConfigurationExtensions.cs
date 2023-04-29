using CarbonAwareComputing.ExecutionForecast;
using CarbonAwareComputing.Hangfire;
using Hangfire.Client;
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace Hangfire
{
    public static class GlobalConfigurationExtensions
    {
        public static IGlobalConfiguration UseCarbonAwareDataProvider(this IGlobalConfiguration configuration, CarbonAwareDataProvider dataProvider, ComputingLocation location)
        {
            var options = new CarbonAwareOptions(dataProvider, location);

            GlobalJobFilters.Filters.Add(options);
            GlobalJobFilters.Filters.Add(new JobFilter());
            return configuration;
        }
        public static IGlobalConfiguration UseCarbonAwareDataProvider(this IGlobalConfiguration configuration, Func<CarbonAwareExecutionOptions> configure)
        {
            var o = configure.Invoke();
            var options = new CarbonAwareOptions(o.DataProvider, o.Location);

            GlobalJobFilters.Filters.Add(options);
            return configuration;
        }
    }

    public record CarbonAwareExecutionOptions(CarbonAwareDataProvider DataProvider, ComputingLocation Location);

    public class JobFilter : IClientFilter
    {
        public void OnCreating(CreatingContext filterContext)
        {
            
        }

        public void OnCreated(CreatedContext filterContext)
        {
            
        }
    }
}

