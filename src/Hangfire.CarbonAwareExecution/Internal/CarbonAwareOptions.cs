using CarbonAwareComputing;
using Hangfire.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Hangfire.Community.CarbonAwareExecution.Internal;

internal class CarbonAwareOptions(Func<ScopedDataProvider> dataProvider, ComputingLocation computingLocation)
    : IClientFilter
{
    public Func<ScopedDataProvider> DataProvider { get; } = dataProvider;
    public ComputingLocation ComputingLocation { get; } = computingLocation;

    public void OnCreating(CreatingContext filterContext)
    {
    }

    public void OnCreated(CreatedContext filterContext)
    {
    }
}

internal record ScopedDataProvider(CarbonAwareDataProvider DataProvider, IServiceScope? Scope);