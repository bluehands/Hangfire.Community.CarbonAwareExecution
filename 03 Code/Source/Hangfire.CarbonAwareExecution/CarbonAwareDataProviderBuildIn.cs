using CarbonAware.DataSources.Memory;
using GSF.CarbonAware.Configuration;
using GSF.CarbonAware.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hangfire.CarbonAwareExecution;

public abstract class CarbonAwareDataProviderCachedData<T> : CarbonAwareDataProvider where T : class, IEmissionsDataProvider, new()
{
    private readonly ComputingLocation m_Location;
    protected readonly ServiceProvider Services;

    protected CarbonAwareDataProviderCachedData(ComputingLocation location)
    {
        m_Location = location;
        Services = InitializeSdk();
    }

    private static ServiceProvider InitializeSdk()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            {"LocationDataSourcesConfiguration:LocationSourceFiles:DataFileLocation", ""},
            {"LocationDataSourcesConfiguration:LocationSourceFiles:Prefix", "az"},
            {"LocationDataSourcesConfiguration:LocationSourceFiles:Delimiter", "-"},
            {"DataSources:EmissionsDataSource", ""},
            {"DataSources:ForecastDataSource", "Memory"},
            {"DataSources:Configurations:Memory:Type", "MEMORY"},
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();


        var services = new ServiceCollection()
            .AddLogging(loggerBuilder =>
            {
                loggerBuilder.ClearProviders();
                loggerBuilder.AddConsole();
            })
            .AddSingleton<HttpClient>()
            .AddSingleton<IEmissionsDataProvider, T>()
            .AddForecastServices(configuration)
            .BuildServiceProvider();
        return services;
    }

    public override async Task<(bool ForecastIsAvailable,DateTimeOffset BestExecutionTime)> CalculateBestExecutionTime(DateTimeOffset earliestExecutionTime, DateTimeOffset latestExecutionTime, TimeSpan estimatedJobDuration)
    {
        var handler = Services.GetService<IForecastHandler>();
        if (handler == null)
        {
            return (false,DateTimeOffset.Now);
        }

        var adjustedForecastBoundary = await TryAdjustForecastBoundary(earliestExecutionTime, latestExecutionTime - estimatedJobDuration);
        if (!adjustedForecastBoundary.ForecastIsAvailable)
        {
            return (false,DateTimeOffset.Now);
        }

        var lastStartTime = adjustedForecastBoundary.LastStartTime;
        var earliestStartTime = adjustedForecastBoundary.EarliestStartTime;
        var forecast = await handler.GetCurrentForecastAsync(new[] { m_Location.Name }, earliestStartTime, lastStartTime, Convert.ToInt32(estimatedJobDuration.TotalMinutes));
        var best = forecast.First().OptimalDataPoints.FirstOrDefault();
        if (best == null)
        {
            return (false,DateTimeOffset.Now);
        }

        return (true, best.Time);
    }

    protected abstract Task<DataBoundary> GetDataBoundary(ComputingLocation computingLocation);
    private async Task<(bool ForecastIsAvailable, DateTimeOffset EarliestStartTime, DateTimeOffset LastStartTime)> TryAdjustForecastBoundary(DateTimeOffset earliestStartTime, DateTimeOffset lastStartTime)
    {
        var provider = Services.GetService<IEmissionsDataProvider>() as JsonEmissionsDataProvider;
        if (provider == null)
        {
            return (false, DateTimeOffset.Now, DateTimeOffset.Now);
        }
        var boundary = await GetDataBoundary(m_Location);
        if (lastStartTime > boundary.EndTime)
        {
            lastStartTime = boundary.EndTime;
        }

        if (earliestStartTime < boundary.StartTime)
        {
            earliestStartTime = boundary.StartTime;
        }
        if (DateTimeOffset.Now < earliestStartTime || DateTimeOffset.Now > lastStartTime)
        {
            return (false, DateTimeOffset.Now, DateTimeOffset.Now);
        }
        return (true, earliestStartTime, lastStartTime);
    }
}
public class CarbonAwareDataProviderBuildIn : CarbonAwareDataProviderCachedData<JsonEmissionsDataProvider>
{
    public CarbonAwareDataProviderBuildIn(ComputingLocation location) : base(location)
    {
    }

    protected override async Task<DataBoundary> GetDataBoundary(ComputingLocation computingLocation)
    {
        var provider = Services.GetService<IEmissionsDataProvider>() as JsonEmissionsDataProvider;
        if (provider == null)
        {
            return new DataBoundary(DateTimeOffset.MaxValue, DateTimeOffset.MinValue);
        }
        return await provider.GetDataBoundary(computingLocation);
    }
}


