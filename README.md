# Carbon Aware Computing Hangfire Extension

## Overview

A [Hangfire](https://www.hangfire.io/) extension to schedule tasks with **carbon awareness** in mind. The best point in time is calculated based on emission forecasts to get a window with a minimal grid carbon intensity.

## Installation

Hangfire.Community.CarbonAwareExecution is available as a NuGet package. You can install it using the NuGet Package Console window:

``` powershell
Install-Package Hangfire.Community.CarbonAwareExecution
```

After installation add the extension to the Hangfire configuration. It extends the *AddHangfire*-Extension to add additional dependencies.

``` csharp
builder.Services.AddHangfire(configuration => configuration
    .UseCarbonAwareExecution(new CarbonAwareDataProviderOpenData(), ComputingLocations.Germany)
);
```

## Usage

To enable carbon awareness for a specific job, add a CarbonAwareExecution parameter to the method called by hangfire. The parameter defines the maximum acceptable delay for the job. Pass null to disable carbon aware shifting. 
This works for all kind of hangfire jobs, namly fire and forget, scheduled and recurring jobs. See [JobController](https://github.com/bluehands/Hangfire.Community.CarbonAwareExecution/blob/main/samples/Usage/Controllers/JobsController.cs) for more examples.

``` csharp
public static class HangfireActions
{
    public static void CarbonAwareJob(/*your functional parameters*/ CarbonAwareExecution? carbonAware) => ...;
}
```

### Fire and Forget tasks

Setup maximum delay and the estimated task duration. The extension will do a best effort to get a window with the estimated task duration and minimal grid carbon intensity. When no window can be detected, the task is enqueued immediately.

``` csharp
//use the extension methods
IBackgroundJobClient client = GetBackgroundJobClient();
client.Enqueue(
    () => HangfireActions.CarbonAwareJob(
        "Hello world from Hangfire!. Enqueue carbon aware jobs.",
        new CarbonAwareExecution(TimeSpan.FromHours(2), TimeSpan.FromMinutes(5))
));

//or use the static versions
BackgroundJob.Enqueue(
    () => HangfireActions.CarbonAwareJob(
        "Hello world from Hangfire!. Enqueue carbon aware jobs.",
        new CarbonAwareExecution(TimeSpan.FromHours(2), TimeSpan.FromMinutes(5))
));    
```

### Delayed tasks

Setup the earliest execution time, the maximum delay and the estimated task duration. The extension will do a best effort to get a window with the estimated task duration and minimal grid carbon intensity. When no window can be detected, the task is scheduled as desired.

``` csharp
//use the extension methods
IBackgroundJobClient client = GetBackgroundJobClient();
client.Schedule(
    () => HangfireActions.CarbonAwareJob(
            "Hello world from Hangfire!. Schedule carbon aware jobs",
            new CarbonAwareExecution(TimeSpan.FromHours(2), TimeSpan.FromMinutes(5))),
        TimeSpan.FromMinutes(20)
);

//or use the static versions
BackgroudJob.Schedule(
    () => HangfireActions.CarbonAwareJob(
            "Hello world from Hangfire!. Schedule carbon aware jobs",
            new CarbonAwareExecution(TimeSpan.FromHours(2), TimeSpan.FromMinutes(5))),
        TimeSpan.FromMinutes(20)
);
```

### Recurring tasks

Setup the maximum execution delay after planned schedule time and the estimated task duration. The extension will do a best effort to get a window with the estimated task duration and minimal grid carbon intensity. When no window can be detected, the task is scheduled as desired.

``` csharp
//use the extension methods
IRecurringJobManager manager = GetRecurringJobManager();
manager.AddOrUpdate(
    "daily",
    () => HangfireActions.CarbonAwareJobAsync("Hello world from Hangfire!. Recurring carbon aware jobs",
        new CarbonAwareExecution(TimeSpan.FromHours(3), TimeSpan.FromMinutes(5))),
    "2 0 * * *"
);

//or use the static versions
RecurringJob.AddOrUpdate(
    "daily",
    () => HangfireActions.CarbonAwareJobAsync("Hello world from Hangfire!. Recurring carbon aware jobs",
        new CarbonAwareExecution(TimeSpan.FromHours(3), TimeSpan.FromMinutes(5))),
    "2 0 * * *"
);
```

The Hangfire Carbon Aware Extension will prevent the execution of the current instance of the recurring job. It is calculation a execution window with minimal carbon impact and the schedule that task. In the dashboard you will see the notice that the job was executed and a newly planned task.

## Fallback

If your computing location is outside Europe or you need other forecasts the WattTime data provider may be useful. You need a valid WattTime account to use the data provider.

``` csharp
builder.Services.AddHangfire(configuration => configuration
    .UseCarbonAwareExecution(
        () => new CarbonAwareExecutionOptions(
            new CarbonAwareDataProviderWattTime(userName, password), 
            ComputingLocations.Germany))
        );
```

## Extensibility

For custom forecasts or scenarios you don't want the build in provider add a own data provider. You may extend the abstract base class *CarbonAwareDataProvider* or use the *CarbonAwareDataProviderWithCustomForecast*. A WattTime data provider is implemented as well.

## Methodology

**Hangfire.Community.CarbonAwareExecution Extension** makes use of the [Carbon Aware SDK](https://github.com/Green-Software-Foundation/carbon-aware-sdk) a [Green Software Foundation](https://greensoftware.foundation/) Project. There are some extensions to the SDK to use cached offline data sources in our [fork](https://github.com/bluehands/carbon-aware-sdk).

The emission forecast data are uploaded periodically to a Azure Blob Storage for a given grid region and are public (e.g. for Germany <https://carbonawarecomputing.blob.core.windows.net/forecasts/de.json>).

To avoid unnecessary processing only a few grid regions are active. Currently de, fr, at, ch

* **Europe (without UK)**: The API from <https://www.energy-charts.info/> are used. The data is the share of renewable energy to the total power production. The origin data source is from [entso-e](https://www.entsoe.eu/). The data is generated every day at 19:00+01 for the next day. After 19:00+01 the maximum forecast is next day 24:00+01.

* **United Kingdom**: The API from <https://carbonintensity.org.uk/> are used. The data is the carbon intensity. The data is generated periodically .



We will provide data for the european grid regions. Please send a mail to am@bluehands.de if you need for one of the inactive regions.

For forecasts outside of europe you may use the WattTime provider with an active account.

To shift job executions in Hangfire an `IElectStateFilter` is used to calculate the delay at enqueue time and reschedule the background job once if it was not rescheduled before. Use can actually use the [`ShiftJobFilter<TParameter>`](https://github.com/bluehands/Hangfire.Community.CarbonAwareExecution/blob/main/src/Hangfire.CarbonAwareExecution/ShiftJobFilter.cs) filter to implement custom shifting logic in a very simple way.
