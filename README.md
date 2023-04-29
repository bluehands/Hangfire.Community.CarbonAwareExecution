# Carbon Aware Computing Hangfire Extension

## Overview

A [Hangfire](https://www.hangfire.io/) extension to schedule tasks with **carbon awareness** in mind. The best point in time is calculated based on emission forecasts to get a window with a minimal grid carbon intensity.

## Installation

CarbonAwareComputing.Hangfire is available as a NuGet package. You can install it using the NuGet Package Console window:

``` powershell
Install-Package CarbonAwareComputing.Hangfire
```

After installation add the extension to the Hangfire configuration.

``` csharp
builder.Services.AddHangfireCarbonAwareExecution(configuration => configuration
    .UseCarbonAwareExecution(new CarbonAwareDataProviderOpenData(), ComputingLocations.Germany)
);
```

## Usage

There are extension to **Enqueue** and **Schedule** with *WithCarbonAwarenessAsync*.

### Fire and Forget tasks

Setup the latest execution time and the estimated task duration. The extension will do a best effort to get a window with the estimated task duration and minimal grid carbon intensity. When no window can be detected, the task is enqueued immediately.

``` csharp
//use the extension methods
IBackgroundJobClient client = GetBackgroundJobClient();
await client.EnqueueWithCarbonAwarenessAsync(
    () => Console.WriteLine("Enqueue carbon aware jobs"),
    DateTimeOffset.Now + TimeSpan.FromHours(2),
    TimeSpan.FromMinutes(5));

//or use the static versions
await CarbonAwareBackgroundJob.EnqueueAsync(
    () => Console.WriteLine("Enqueue carbon aware jobs"),
    DateTimeOffset.Now + TimeSpan.FromHours(2),
    TimeSpan.FromMinutes(5));    
```

### Delayed tasks

Setup the earliest and latest execution time and the estimated task duration. The extension will do a best effort to get a window with the estimated task duration and minimal grid carbon intensity. When no window can be detected, the task is scheduled as desired.

``` csharp
//use the extension methods
IBackgroundJobClient client = GetBackgroundJobClient();
await client.ScheduleWithCarbonAwarenessAsync(
    () => Console.WriteLine("Schedule carbon aware jobs"),
    DateTimeOffset.Now + TimeSpan.FromHours(2),
    TimeSpan.FromMinutes(20),
    TimeSpan.FromMinutes(30), TimeSpan.FromMinutes(5));

//or use the static versions
await CarbonAwareBackgroundJob.ScheduleAsync(
        () => Console.WriteLine("Schedule carbon aware jobs"),
        DateTimeOffset.Now + TimeSpan.FromHours(2),
        TimeSpan.FromMinutes(20),
        TimeSpan.FromMinutes(30), TimeSpan.FromMinutes(5));
```

### Recurring tasks

Setup the maximum execution delay after planned schedule time and the estimated task duration. The extension will do a best effort to get a window with the estimated task duration and minimal grid carbon intensity. When no window can be detected, the task is scheduled as desired.

``` csharp
//use the extension methods
IRecurringJobManager manager = GetRecurringJobManager();
manager.AddOrUpdateCarbonAware(
    "daily", 
    () => Console.WriteLine("Hello, world!"), 
    Cron.Daily, 
    TimeSpan.FromHours(2),
    TimeSpan.FromMinutes(20));

//or use the static versions
CarbonAwareRecurringJob.AddOrUpdate(
    "daily", 
    () => Console.WriteLine("Hello, world!"), 
    Cron.Daily, TimeSpan.FromHours(2), 
    TimeSpan.FromMinutes(20));
```

The Hangfire Carbon Aware Extension will prevent the execution of the current instance of the recurring job. It is calculation a execution window with minimal carbon impact and the schedule that task. In the dashboard you will see the notice that the job was executed and a newly planned task.

## Fallback

If your computing location is outside Europe or you need other forecasts the WattTime data provider may be useful. You need a valid WattTime account to use the data provider.

``` csharp
builder.Services.AddHangfireCarbonAwareExecution(configuration => configuration
    .UseCarbonAwareExecution(
        () => new CarbonAwareExecutionOptions(
            new CarbonAwareDataProviderWattTime(userName, password), 
            ComputingLocations.Germany))
        );
```

## Extensibility

For custom forecasts or scenarios you don't want the build in provider add a own data provider. You may extend the abstract base class *CarbonAwareDataProvider* or use the *CarbonAwareDataProviderWithCustomForecast*.

## Methodology

**Hangfire.CarbonAwareExecution Extension** make use of the [Carbon Aware SDK](https://github.com/Green-Software-Foundation/carbon-aware-sdk) a [Green Software Foundation](https://greensoftware.foundation/) Project. There are some extensions to the SDK to use cached offline data sources in our [fork](https://github.com/bluehands/carbon-aware-sdk).

The emission forecast data are uploaded periodically to a Azure Blob Storage for a given grid region and are public (e.g. for Germany <https://carbonawarecomputing.blob.core.windows.net/forecasts/de.json>).

For every grid region a data provider is needed:

* **Germany**: The API from <https://www.energy-charts.info/> are used. The data is the share of renewable energy to the total power production. The origin data source is from [entso-e](https://www.entsoe.eu/). The data is generated every day at 19:00+02 for the next day. After 19:00+02 the maximum forecast is next day 24:00+02.

Other data sources and regions are added soon.
