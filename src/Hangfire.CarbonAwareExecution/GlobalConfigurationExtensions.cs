using CarbonAwareComputing;
using Hangfire.Community.CarbonAwareExecution;
using Hangfire.Community.CarbonAwareExecution.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Hangfire
{
    public static class GlobalConfigurationExtensions
    {
        public static IGlobalConfiguration UseCarbonAwareExecution(
            this IGlobalConfiguration configuration, 
            CarbonAwareDataProvider dataProvider,
            ComputingLocation location,
            ILogger? logger = null)
        {
            var options = new CarbonAwareOptions(() => new (dataProvider, null), location);

            GlobalJobFilters.Filters.Add(options);
            GlobalJobFilters.Filters.Add(new ShiftJobCarbonAwareFilter(logger));
            return configuration;
        }

        public static IGlobalConfiguration UseCarbonAwareExecution(
            this IGlobalConfiguration configuration,
            Func<CarbonAwareExecutionOptions> configure,
            ILogger? logger = null)
        {
            var o = configure.Invoke();
            var options = new CarbonAwareOptions(() => new (o.DataProvider, null), o.Location);

            GlobalJobFilters.Filters.Add(options);
            GlobalJobFilters.Filters.Add(new ShiftJobCarbonAwareFilter(logger));
            return configuration;
        }

        /// <summary>
        /// This overload requires a service of type CarbonAwareDataProvider to be registered with your IServiceCollection.
        /// </summary>
        public static IGlobalConfiguration UseCarbonAwareExecution(
            this IGlobalConfiguration configuration,
            ComputingLocation location,
            IServiceProvider serviceProvider)
        {
            var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
            var options = new CarbonAwareOptions(() =>
            {
                var scope = scopeFactory.CreateScope();
                var dataProvider = scope.ServiceProvider.GetService<CarbonAwareDataProvider>();
                if (dataProvider == null)
                {
                    scope.Dispose();
                    throw new ArgumentException($"No service of type {nameof(CarbonAwareDataProvider)} registered. Please register or pass {nameof(CarbonAwareDataProvider)} to UseCarbonAwareExecution.");
                }
                return new(dataProvider, scope);
            }, location);

            GlobalJobFilters.Filters.Add(options);
            GlobalJobFilters.Filters.Add(new ShiftJobCarbonAwareFilter(serviceProvider.GetService<ILogger<ShiftJobCarbonAwareFilter>>()));
            return configuration;
        }
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed record CarbonAwareExecutionOptions(CarbonAwareDataProvider DataProvider, ComputingLocation Location);
}