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
            var services = new CarbonAwareServices(() => new (dataProvider, null), logger);

            GlobalJobFilters.Filters.Add(new ShiftJobCarbonAwareFilter(services, location));
            return configuration;
        }

        public static IGlobalConfiguration UseCarbonAwareExecution(
            this IGlobalConfiguration configuration,
            Func<CarbonAwareExecutionOptions> configure,
            ILogger? logger = null)
        {
            var o = configure.Invoke();
            var services = new CarbonAwareServices(() => new (o.DataProvider, null), logger);

            GlobalJobFilters.Filters.Add(services);
            GlobalJobFilters.Filters.Add(new ShiftJobCarbonAwareFilter(services, o.Location));
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
            var services = new CarbonAwareServices(() =>
            {
                var scope = scopeFactory.CreateScope();
                var dataProvider = scope.ServiceProvider.GetService<CarbonAwareDataProvider>();
                if (dataProvider == null)
                {
                    scope.Dispose();
                    throw new ArgumentException($"No service of type {nameof(CarbonAwareDataProvider)} registered. Please register or pass {nameof(CarbonAwareDataProvider)} to UseCarbonAwareExecution.");
                }
                return new(dataProvider, scope);
            }, serviceProvider.GetService<ILogger<ShiftJobCarbonAwareFilter>>());

            GlobalJobFilters.Filters.Add(
                new ShiftJobCarbonAwareFilter(services, location)
            );
            return configuration;
        }
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed record CarbonAwareExecutionOptions(CarbonAwareDataProvider DataProvider, ComputingLocation Location);
}