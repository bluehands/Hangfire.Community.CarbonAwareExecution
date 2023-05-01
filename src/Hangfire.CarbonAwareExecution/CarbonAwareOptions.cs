using CarbonAwareComputing.ExecutionForecast;
using Hangfire.Client;

namespace Hangfire.Community.CarbonAwareExecution;

internal class CarbonAwareOptions : IClientFilter
{
    public CarbonAwareOptions(CarbonAwareDataProvider dataProvider, ComputingLocation computingLocation)
    {
        DataProvider = dataProvider;
        ComputingLocation = computingLocation;
    }

    public CarbonAwareDataProvider DataProvider { get; init; }
    public ComputingLocation ComputingLocation { get; }

    public void OnCreating(CreatingContext filterContext)
    {

    }

    public void OnCreated(CreatedContext filterContext)
    {

    }
}