using CarbonAwareComputing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hangfire.Community.CarbonAwareExecution.Internal;

internal class CarbonAwareServices(
    Func<ScopedDataProvider> dataProvider, 
    ILogger? logger)
{
    public ILogger? Logger {get; } = logger;
    public Func<ScopedDataProvider> DataProvider { get; } = dataProvider;
}

internal record ScopedDataProvider(CarbonAwareDataProvider DataProvider, IServiceScope? Scope);