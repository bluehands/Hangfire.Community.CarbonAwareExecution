# Hangfire.CarbonAwareExecution Extension

## Overview

A [Hangfire](https://www.hangfire.io/) extension to schedule tasks with **carbon awareness** in mind. The best point in time is calculated based on emission forecasts to get a window with a minimal grid carbon intensity.

## Installation

HangfireCarbonAwareExecution is available as a NuGet package. You can install it using the NuGet Package Console window:

``` powershell
Install-Package Hangfire.CarbonAwareExecution
```

After installation add the extension to the Hangfire configuration.

``` csharp
builder.Services.AddHangfire(configuration => configuration
    .UseCarbonAwareExecution(new CarbonAwareDataProviderBuildIn(ComputingLocations.Germany))
);
```

## Usage

There are extension to **Enqueue** and **Schedule** with *WithCarbonAwarenessAsync*.

### Fire and Forget tasks

Setup the latest execution time and the estimated task duration. The extension will do a best effort to get a window with the estimated task duration and minimal grid carbon intensity. When no window can be detected, the task is enqueued immediately.

``` csharp
await BackgroundJobs.EnqueueWithCarbonAwarenessAsync(
    () => Console.WriteLine("Hello world from Hangfire!. Enqueue carbon aware jobs"),
    DateTimeOffset.Now + TimeSpan.FromHours(2),
    TimeSpan.FromMinutes(5));
```

### Delayed tasks

Setup the earliest and latest execution time and the estimated task duration. The extension will do a best effort to get a window with the estimated task duration and minimal grid carbon intensity. When no window can be detected, the task is scheduled as desired.

``` csharp
await m_BackgroundJobs.ScheduleWithCarbonAwarenessAsync(
    () => Console.WriteLine("Hello world from Hangfire!. Schedule carbon aware jobs"),
    DateTimeOffset.Now + TimeSpan.FromHours(2),
    TimeSpan.FromMinutes(20),
    TimeSpan.FromMinutes(30), TimeSpan.FromMinutes(5));

```

## Methodology

**Hangfire.CarbonAwareExecution Extension** make use of the [Carbon Aware SDK](https://github.com/Green-Software-Foundation/carbon-aware-sdk) a [Green Software Foundation](https://greensoftware.foundation/) Project. There are some extensions to the SDK to use cached offline data sources in our [fork](https://github.com/bluehands/carbon-aware-sdk).

The emission forecast data are uploaded periodically to a Azure Blob Storage for a given grid region and are public (e.g. for Germany <https://carbonawarecomputing.blob.core.windows.net/forecasts/de.json>).

For every grid region a data provider is needed:

* **Germany**: The API from <https://www.energy-charts.info/> are used. The data is the share of renewable energy to the total power production. The origin data source is from [entso-e](https://www.entsoe.eu/). The data is generated every day at 19:00+02 for the next day. After 19:00+02 the maximum forecast is next day 24:00+02.

Other data sources and regions are added soon.
