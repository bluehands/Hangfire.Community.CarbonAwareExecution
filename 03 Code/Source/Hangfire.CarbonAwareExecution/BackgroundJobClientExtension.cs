using Hangfire.Annotations;
using Hangfire.States;
using System.Linq.Expressions;
// ReSharper disable UnusedMember.Global

namespace Hangfire.CarbonAwareExecution;

public static class BackgroundJobClientExtension
{
    /// <summary>
    /// Creates a background job based on a specified lambda expression
    /// and places it into its actual queue.
    /// Please, see the <see cref="T:Hangfire.QueueAttribute" /> to learn how to
    /// place the job on a non-default queue.
    /// </summary>
    /// <param name="client">A job client instance.</param>
    /// <param name="methodCall">Static method call expression that will be marshalled to the Server.</param>
    /// <param name="latestExecutionTime">The latest possible time that the job should be finished</param>
    /// <param name="estimatedJobDuration">Estimated duration the job will take. The latest starting time is calculated as latestExecutionTime - estimatedJobDuration</param>
    /// <returns>Unique identifier of the created job.</returns>
    public static async Task<string> EnqueueWithCarbonAwarenessAsync(this IBackgroundJobClient client, Expression<Action> methodCall, DateTimeOffset latestExecutionTime, TimeSpan estimatedJobDuration)
    {
        var scheduleOptions = await GetBestScheduleTime(latestExecutionTime, estimatedJobDuration);
        if (!scheduleOptions.HasForecast)
        {
            return client.Enqueue(methodCall);
        }
        return client.Schedule(methodCall, scheduleOptions.BestScheduleTime);
    }

    /// <summary>
    /// Creates a background job based on a specified lambda expression
    /// and places it into its actual queue.
    /// Please, see the <see cref="T:Hangfire.QueueAttribute" /> to learn how to
    /// place the job on a non-default queue.
    /// </summary>
    /// <param name="client">A job client instance.</param>
    /// <param name="methodCall">Static method call expression that will be marshalled to the Server.</param>
    /// <param name="latestExecutionTime">The latest possible time that the job should be finished</param>
    /// <param name="estimatedJobDuration">Estimated duration the job will take. The latest starting time is calculated as latestExecutionTime - estimatedJobDuration</param>
    /// <returns>Unique identifier of the created job.</returns>
    public static async Task<string> EnqueueWithCarbonAwarenessAsync(this IBackgroundJobClient client, Expression<Func<Task>> methodCall, DateTimeOffset latestExecutionTime, TimeSpan estimatedJobDuration)
    {
        var scheduleOptions = await GetBestScheduleTime(latestExecutionTime, estimatedJobDuration);
        if (!scheduleOptions.HasForecast)
        {
            return client.Enqueue(methodCall);
        }
        return client.Schedule(methodCall, scheduleOptions.BestScheduleTime);
    }

    /// <summary>
    /// Creates a background job based on a specified lambda expression
    /// and places it into its actual queue.
    /// Please, see the <see cref="T:Hangfire.QueueAttribute" /> to learn how to
    /// place the job on a non-default queue.
    /// </summary>
    /// <typeparam name="T">Type whose method will be invoked during job processing.</typeparam>
    /// <param name="client">A job client instance.</param>
    /// <param name="methodCall">Instance method call expression that will be marshalled to the Server.</param>
    /// <param name="latestExecutionTime">The latest possible time that the job should be finished</param>
    /// <param name="estimatedJobDuration">Estimated duration the job will take. The latest starting time is calculated as latestExecutionTime - estimatedJobDuration</param>
    /// <returns>Unique identifier of the created job.</returns>
    public static async Task<string> EnqueueWithCarbonAwarenessAsync<T>(this IBackgroundJobClient client, Expression<Action<T>> methodCall, DateTimeOffset latestExecutionTime, TimeSpan estimatedJobDuration)
    {
        var scheduleOptions = await GetBestScheduleTime(latestExecutionTime, estimatedJobDuration);
        if (!scheduleOptions.HasForecast)
        {
            return client.Enqueue(methodCall);
        }
        return client.Schedule(methodCall, scheduleOptions.BestScheduleTime);
    }

    /// <summary>
    /// Creates a background job based on a specified lambda expression
    /// and places it into its actual queue.
    /// Please, see the <see cref="T:Hangfire.QueueAttribute" /> to learn how to
    /// place the job on a non-default queue.
    /// </summary>
    /// <typeparam name="T">Type whose method will be invoked during job processing.</typeparam>
    /// <param name="client">A job client instance.</param>
    /// <param name="methodCall">Instance method call expression that will be marshalled to the Server.</param>
    /// <param name="latestExecutionTime">The latest possible time that the job should be finished</param>
    /// <param name="estimatedJobDuration">Estimated duration the job will take. The latest starting time is calculated as latestExecutionTime - estimatedJobDuration</param>
    /// <returns>Unique identifier of the created job.</returns>
    public static async Task<string> EnqueueWithCarbonAwarenessAsync<T>(this IBackgroundJobClient client, Expression<Func<T, Task>> methodCall, DateTimeOffset latestExecutionTime, TimeSpan estimatedJobDuration)
    {
        var scheduleOptions = await GetBestScheduleTime(latestExecutionTime, estimatedJobDuration);
        if (!scheduleOptions.HasForecast)
        {
            return client.Enqueue(methodCall);
        }
        return client.Schedule(methodCall, scheduleOptions.BestScheduleTime);
    }

    /// <summary>
    /// Creates a new background job based on a specified lambda expression
    /// and schedules it to be enqueued after a given delay.
    /// </summary>
    /// <param name="client">A job client instance.</param>
    /// <param name="methodCall">Instance method call expression that will be marshalled to the Server.</param>
    /// <param name="delay">Delay, after which the job will be enqueued.</param>
    /// <param name="earliestDelay">The earliest start date compare to the delay</param>
    /// <param name="latestDelay">The latest start date compare to the delay</param>
    /// <param name="estimatedJobDuration">Estimated duration the job will take. The latest starting time is calculated as latestExecutionTime - estimatedJobDuration</param>
    /// <returns>Unique identifier of the created job.</returns>
    public static async Task<string> ScheduleWithCarbonAwarenessAsync(this IBackgroundJobClient client, Expression<Action> methodCall, TimeSpan delay, TimeSpan earliestDelay, TimeSpan latestDelay, TimeSpan estimatedJobDuration)
    {
        var scheduleOptions = await GetBestScheduleTime(DateTimeOffset.Now - earliestDelay, DateTimeOffset.Now + latestDelay, estimatedJobDuration);
        if (!scheduleOptions.HasForecast)
        {
            return client.Schedule(methodCall, delay);
        }
        return client.Schedule(methodCall, scheduleOptions.BestScheduleTime);
    }
    /// <summary>
    /// Creates a new background job based on a specified lambda expression
    /// and schedules it to be enqueued after a given delay.
    /// </summary>
    /// <param name="client">A job client instance.</param>
    /// <param name="methodCall">Instance method call expression that will be marshalled to the Server.</param>
    /// <param name="delay">Delay, after which the job will be enqueued.</param>
    /// <param name="earliestDelay">The earliest start date compare to the delay</param>
    /// <param name="latestDelay">The latest start date compare to the delay</param>
    /// <param name="estimatedJobDuration">Estimated duration the job will take. The latest starting time is calculated as latestExecutionTime - estimatedJobDuration</param>
    /// <returns>Unique identifier of the created job.</returns>
    public static async Task<string> ScheduleWithCarbonAwarenessAsync(this IBackgroundJobClient client, Expression<Func<Task>> methodCall, TimeSpan delay, TimeSpan earliestDelay, TimeSpan latestDelay, TimeSpan estimatedJobDuration)
    {
        var scheduleOptions = await GetBestScheduleTime(DateTimeOffset.Now - earliestDelay, DateTimeOffset.Now + latestDelay, estimatedJobDuration);
        if (!scheduleOptions.HasForecast)
        {
            return client.Schedule(methodCall, delay);
        }
        return client.Schedule(methodCall, scheduleOptions.BestScheduleTime);
    }
    /// <summary>
    /// Creates a new background job based on a specified lambda expression
    /// and schedules it to be enqueued at the specified moment of time.
    /// </summary>
    /// <param name="client">A job client instance.</param>
    /// <param name="methodCall">Method call expression that will be marshalled to the Server.</param>
    /// <param name="enqueueAt">Moment of time at which the job will be enqueued.</param>
    /// <param name="earliestDelay">The earliest start date compare to the delay</param>
    /// <param name="latestDelay">The latest start date compare to the delay</param>
    /// <param name="estimatedJobDuration">Estimated duration the job will take. The latest starting time is calculated as latestExecutionTime - estimatedJobDuration</param>
    /// <returns>Unique identifier or a created job.</returns>
    public static async Task<string> ScheduleWithCarbonAwarenessAsync(this IBackgroundJobClient client, Expression<Action> methodCall, DateTimeOffset enqueueAt, TimeSpan earliestDelay, TimeSpan latestDelay, TimeSpan estimatedJobDuration)
    {
        var scheduleOptions = await GetBestScheduleTime(enqueueAt - earliestDelay, enqueueAt + latestDelay, estimatedJobDuration);
        if (!scheduleOptions.HasForecast)
        {
            return client.Schedule(methodCall, enqueueAt);
        }
        return client.Schedule(methodCall, scheduleOptions.BestScheduleTime);
    }

    /// <summary>
    /// Creates a new background job based on a specified lambda expression
    /// and schedules it to be enqueued at the specified moment of time.
    /// </summary>
    /// <param name="client">A job client instance.</param>
    /// <param name="methodCall">Method call expression that will be marshalled to the Server.</param>
    /// <param name="enqueueAt">Moment of time at which the job will be enqueued.</param>
    /// <param name="earliestDelay">The earliest start date compare to the delay</param>
    /// <param name="latestDelay">The latest start date compare to the delay</param>
    /// <param name="estimatedJobDuration">Estimated duration the job will take. The latest starting time is calculated as latestExecutionTime - estimatedJobDuration</param>
    /// <returns>Unique identifier or a created job.</returns>
    public static async Task<string> ScheduleWithCarbonAwarenessAsync(this IBackgroundJobClient client, Expression<Func<Task>> methodCall, DateTimeOffset enqueueAt, TimeSpan earliestDelay, TimeSpan latestDelay, TimeSpan estimatedJobDuration)
    {
        var scheduleOptions = await GetBestScheduleTime(enqueueAt - earliestDelay, enqueueAt + latestDelay, estimatedJobDuration);
        if (!scheduleOptions.HasForecast)
        {
            return client.Schedule(methodCall, enqueueAt);
        }
        return client.Schedule(methodCall, scheduleOptions.BestScheduleTime);
    }

    /// <summary>
    /// Creates a new background job based on a specified instance method
    /// call expression and schedules it to be enqueued after a given delay.
    /// </summary>
    /// <typeparam name="T">Type whose method will be invoked during job processing.</typeparam>
    /// <param name="client">A job client instance.</param>
    /// <param name="methodCall">Instance method call expression that will be marshalled to the Server.</param>
    /// <param name="delay">Delay, after which the job will be enqueued.</param>
    /// <param name="earliestDelay">The earliest start date compare to the delay</param>
    /// <param name="latestDelay">The latest start date compare to the delay</param>
    /// <param name="estimatedJobDuration">Estimated duration the job will take. The latest starting time is calculated as latestExecutionTime - estimatedJobDuration</param>
    /// <returns>Unique identifier of the created job.</returns>
    public static async Task<string> ScheduleWithCarbonAwarenessAsync<T>(this IBackgroundJobClient client, Expression<Action<T>> methodCall, TimeSpan delay, TimeSpan earliestDelay, TimeSpan latestDelay, TimeSpan estimatedJobDuration)
    {
        var scheduleOptions = await GetBestScheduleTime(DateTimeOffset.Now - earliestDelay, DateTimeOffset.Now + latestDelay, estimatedJobDuration);
        if (!scheduleOptions.HasForecast)
        {
            return client.Schedule(methodCall, delay);
        }
        return client.Schedule(methodCall, scheduleOptions.BestScheduleTime);
    }

    /// <summary>
    /// Creates a new background job based on a specified instance method
    /// call expression and schedules it to be enqueued after a given delay.
    /// </summary>
    /// <typeparam name="T">Type whose method will be invoked during job processing.</typeparam>
    /// <param name="client">A job client instance.</param>
    /// <param name="methodCall">Instance method call expression that will be marshalled to the Server.</param>
    /// <param name="delay">Delay, after which the job will be enqueued.</param>
    /// <param name="earliestDelay">The earliest start date compare to the delay</param>
    /// <param name="latestDelay">The latest start date compare to the delay</param>
    /// <param name="estimatedJobDuration">Estimated duration the job will take. The latest starting time is calculated as latestExecutionTime - estimatedJobDuration</param>
    /// <returns>Unique identifier of the created job.</returns>
    public static async Task<string> ScheduleWithCarbonAwarenessAsync<T>(this IBackgroundJobClient client, Expression<Func<T, Task>> methodCall, TimeSpan delay, TimeSpan earliestDelay, TimeSpan latestDelay, TimeSpan estimatedJobDuration)
    {
        var scheduleOptions = await GetBestScheduleTime(DateTimeOffset.Now - earliestDelay, DateTimeOffset.Now + latestDelay, estimatedJobDuration);
        if (!scheduleOptions.HasForecast)
        {
            return client.Schedule(methodCall, delay);
        }
        return client.Schedule(methodCall, scheduleOptions.BestScheduleTime);
    }

    /// <summary>
    /// Creates a new background job based on a specified lambda expression and schedules
    /// it to be enqueued at the specified moment.
    /// </summary>
    /// <typeparam name="T">Type whose method will be invoked during job processing.</typeparam>
    /// <param name="client">A job client instance.</param>
    /// <param name="methodCall">Method call expression that will be marshalled to the Server.</param>
    /// <param name="enqueueAt">Moment at which the job will be enqueued.</param>
    /// <param name="earliestDelay">The earliest start date compare to the delay</param>
    /// <param name="latestDelay">The latest start date compare to the delay</param>
    /// <param name="estimatedJobDuration">Estimated duration the job will take. The latest starting time is calculated as latestExecutionTime - estimatedJobDuration</param>
    /// <returns>Unique identifier of a created job.</returns>
    public static async Task<string> ScheduleWithCarbonAwarenessAsync<T>(this IBackgroundJobClient client, Expression<Action<T>> methodCall, DateTimeOffset enqueueAt, TimeSpan earliestDelay, TimeSpan latestDelay, TimeSpan estimatedJobDuration)
    {
        var scheduleOptions = await GetBestScheduleTime(enqueueAt - earliestDelay, enqueueAt + latestDelay, estimatedJobDuration);
        if (!scheduleOptions.HasForecast)
        {
            return client.Schedule(methodCall, enqueueAt);
        }
        return client.Schedule(methodCall, scheduleOptions.BestScheduleTime);
    }

    /// <summary>
    /// Creates a new background job based on a specified lambda expression and schedules
    /// it to be enqueued at the specified moment.
    /// </summary>
    /// <typeparam name="T">Type whose method will be invoked during job processing.</typeparam>
    /// <param name="client">A job client instance.</param>
    /// <param name="methodCall">Method call expression that will be marshalled to the Server.</param>
    /// <param name="enqueueAt">Moment at which the job will be enqueued.</param>
    /// <param name="earliestDelay">The earliest start date compare to the delay</param>
    /// <param name="latestDelay">The latest start date compare to the delay</param>
    /// <param name="estimatedJobDuration">Estimated duration the job will take. The latest starting time is calculated as latestExecutionTime - estimatedJobDuration</param>
    /// <returns>Unique identifier of a created job.</returns>
    public static async Task<string> ScheduleWithCarbonAwarenessAsync<T>(this IBackgroundJobClient client, Expression<Func<T, Task>> methodCall, DateTimeOffset enqueueAt, TimeSpan earliestDelay, TimeSpan latestDelay, TimeSpan estimatedJobDuration)
    {
        var scheduleOptions = await GetBestScheduleTime(enqueueAt - earliestDelay, enqueueAt + latestDelay, estimatedJobDuration);
        if (!scheduleOptions.HasForecast)
        {
            return client.Schedule(methodCall, enqueueAt);
        }
        return client.Schedule(methodCall, scheduleOptions.BestScheduleTime);
    }

    private static async Task<(bool HasForecast, DateTimeOffset BestScheduleTime)> GetBestScheduleTime(DateTimeOffset earliestExecutionTime, DateTimeOffset latestExecutionTime, TimeSpan estimatedJobDuration)
    {
        try
        {
            var filter = GlobalJobFilters.Filters.FirstOrDefault(f => f.Instance is CarbonAwareOptions);
            var options = filter?.Instance as CarbonAwareOptions;
            if (options == null)
            {
                return (false, DateTimeOffset.Now);
            }

            var provider = options.DataProvider;
            var bestScheduleTime = await provider.CalculateBestExecutionTime(earliestExecutionTime, latestExecutionTime - estimatedJobDuration, estimatedJobDuration);
            return (true, bestScheduleTime);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return (false, DateTimeOffset.Now);
        }
    }
    private static async Task<(bool HasForecast, DateTimeOffset BestScheduleTime)> GetBestScheduleTime(DateTimeOffset latestExecutionTime, TimeSpan estimatedJobDuration)
    => await GetBestScheduleTime(DateTimeOffset.Now, latestExecutionTime, estimatedJobDuration);
}