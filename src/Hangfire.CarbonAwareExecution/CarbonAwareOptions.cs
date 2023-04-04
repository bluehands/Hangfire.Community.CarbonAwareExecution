using CarbonAwareComputing.ExecutionForecast;
using Hangfire.Client;

namespace Hangfire.CarbonAwareExecution;

internal class CarbonAwareOptions : IClientFilter
{
    public CarbonAwareOptions(CarbonAwareDataProvider dataProvider)
    {
        DataProvider = dataProvider;
    }

    public CarbonAwareDataProvider DataProvider { get; init; }
    public void OnCreating(CreatingContext filterContext)
    {

    }

    public void OnCreated(CreatedContext filterContext)
    {

    }
}