using System.Linq.Expressions;
using CarbonAwareComputing;
using Hangfire.Annotations;
using Hangfire.Community.CarbonAwareExecution;

// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace Hangfire;

public static class CarbonAwareBackgroundJob
{
    /// <summary>
    /// Creates a new fire-and-forget job based on a given method call expression.
    /// </summary>
    /// <param name="methodCall">Method call expression that will be marshalled to a server.</param>
    /// <returns>Unique identifier of a background job.</returns>
    /// 
    /// <exception cref="ArgumentNullException">
    /// <paramref name="methodCall"/> is <see langword="null"/>.
    /// </exception>
    /// <param name="latestExecutionTime">The latest possible time that the job should be finished</param>
    /// <param name="estimatedJobDuration">Estimated duration the job will take. The latest starting time is calculated as latestExecutionTime - estimatedJobDuration</param>
    /// 
    /// <seealso cref="O:Hangfire.IBackgroundJobClient.Enqueue"/>
    public static async Task<string> EnqueueAsync(Expression<Action> methodCall, DateTimeOffset latestExecutionTime, TimeSpan estimatedJobDuration)
    {
        var scheduleOptions = await GetBestScheduleTime(latestExecutionTime, estimatedJobDuration);
        return scheduleOptions.Match(
            _ => BackgroundJob.Enqueue(methodCall),
            bestExecutionTime => BackgroundJob.Schedule(methodCall, bestExecutionTime.ExecutionTime));
    }

    /// <summary>
    /// Creates a new fire-and-forget job based on a given method call expression and places it
    /// to the specified queue.
    /// </summary>
    /// <param name="queue">Default queue for the background job.</param>
    /// <param name="methodCall">Method call expression that will be marshalled to a server.</param>
    /// <returns>Unique identifier of a background job.</returns>
    /// <param name="latestExecutionTime">The latest possible time that the job should be finished</param>
    /// <param name="estimatedJobDuration">Estimated duration the job will take. The latest starting time is calculated as latestExecutionTime - estimatedJobDuration</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="methodCall"/> is <see langword="null"/>.
    /// </exception>
    ///
    /// <seealso cref="O:Hangfire.IBackgroundJobClient.Enqueue"/>
    public static async Task<string> EnqueueAsync([NotNull] string queue, Expression<Action> methodCall, DateTimeOffset latestExecutionTime, TimeSpan estimatedJobDuration)
    {
        var scheduleOptions = await GetBestScheduleTime(latestExecutionTime, estimatedJobDuration);
        return scheduleOptions.Match(
            _ => BackgroundJob.Enqueue(queue, methodCall),
            bestExecutionTime => BackgroundJob.Schedule(queue, methodCall, bestExecutionTime.ExecutionTime));
    }

    /// <summary>
    /// Creates a new fire-and-forget job based on a given method call expression.
    /// </summary>
    /// <param name="methodCall">Method call expression that will be marshalled to a server.</param>
    /// <param name="latestExecutionTime">The latest possible time that the job should be finished</param>
    /// <param name="estimatedJobDuration">Estimated duration the job will take. The latest starting time is calculated as latestExecutionTime - estimatedJobDuration</param>

    /// <returns>Unique identifier of a background job.</returns>
    /// 
    /// <exception cref="ArgumentNullException">
    /// <paramref name="methodCall"/> is <see langword="null"/>.
    /// </exception>
    /// 
    /// <seealso cref="O:Hangfire.IBackgroundJobClient.Enqueue"/>
    public static async Task<string> EnqueueAsync(Expression<Func<Task>> methodCall, DateTimeOffset latestExecutionTime, TimeSpan estimatedJobDuration)
    {
        var scheduleOptions = await GetBestScheduleTime(latestExecutionTime, estimatedJobDuration);
        return scheduleOptions.Match(
            _ => BackgroundJob.Enqueue(methodCall),
            bestExecutionTime => BackgroundJob.Schedule(methodCall, bestExecutionTime.ExecutionTime));
    }

    /// <summary>
    /// Creates a new fire-and-forget job based on a given method call expression and places it
    /// to the specified queue.
    /// </summary>
    /// <param name="queue">Default queue for the background job.</param>
    /// <param name="methodCall">Method call expression that will be marshalled to a server.</param>
    /// <param name="latestExecutionTime">The latest possible time that the job should be finished</param>
    /// <param name="estimatedJobDuration">Estimated duration the job will take. The latest starting time is calculated as latestExecutionTime - estimatedJobDuration</param>
    /// <returns>Unique identifier of a background job.</returns>
    ///
    /// <exception cref="ArgumentNullException">
    /// <paramref name="methodCall"/> is <see langword="null"/>.
    /// </exception>
    ///
    /// <seealso cref="O:Hangfire.IBackgroundJobClient.Enqueue"/>
    public static async Task<string> EnqueueAsync([NotNull] string queue, Expression<Func<Task>> methodCall, DateTimeOffset latestExecutionTime, TimeSpan estimatedJobDuration)
    {
        var scheduleOptions = await GetBestScheduleTime(latestExecutionTime, estimatedJobDuration);
        return scheduleOptions.Match(
            _ => BackgroundJob.Enqueue(queue, methodCall),
            bestExecutionTime => BackgroundJob.Schedule(queue, methodCall, bestExecutionTime.ExecutionTime));
    }

    /// <summary>
    /// Creates a new fire-and-forget job based on a given method call expression.
    /// </summary>
    /// <param name="methodCall">Method call expression that will be marshalled to a server.</param>
    /// <param name="latestExecutionTime">The latest possible time that the job should be finished</param>
    /// <param name="estimatedJobDuration">Estimated duration the job will take. The latest starting time is calculated as latestExecutionTime - estimatedJobDuration</param>
    /// <returns>Unique identifier of a background job.</returns>
    /// 
    /// <exception cref="ArgumentNullException">
    /// <paramref name="methodCall"/> is <see langword="null"/>.
    /// </exception>
    /// 
    /// <seealso cref="O:Hangfire.IBackgroundJobClient.Enqueue"/>
    public static async Task<string> EnqueueAsync<T>(Expression<Action<T>> methodCall, DateTimeOffset latestExecutionTime, TimeSpan estimatedJobDuration)
    {
        var scheduleOptions = await GetBestScheduleTime(latestExecutionTime, estimatedJobDuration);
        return scheduleOptions.Match(
            _ => BackgroundJob.Enqueue(methodCall),
            bestExecutionTime => BackgroundJob.Schedule(methodCall, bestExecutionTime.ExecutionTime));
    }

    /// <summary>
    /// Creates a new fire-and-forget job based on a given method call expression and places it
    /// to the specified queue.
    /// </summary>
    /// <param name="queue">Default queue for the background job.</param>
    /// <param name="methodCall">Method call expression that will be marshalled to a server.</param>
    /// <param name="latestExecutionTime">The latest possible time that the job should be finished</param>
    /// <param name="estimatedJobDuration">Estimated duration the job will take. The latest starting time is calculated as latestExecutionTime - estimatedJobDuration</param>
    /// <returns>Unique identifier of a background job.</returns>
    ///
    /// <exception cref="ArgumentNullException">
    /// <paramref name="methodCall"/> is <see langword="null"/>.
    /// </exception>
    ///
    /// <seealso cref="O:Hangfire.IBackgroundJobClient.Enqueue"/>
    public static async Task<string> EnqueueAsync<T>([NotNull] string queue, Expression<Action<T>> methodCall, DateTimeOffset latestExecutionTime, TimeSpan estimatedJobDuration)
    {
        var scheduleOptions = await GetBestScheduleTime(latestExecutionTime, estimatedJobDuration);
        return scheduleOptions.Match(
            _ => BackgroundJob.Enqueue(queue, methodCall),
            bestExecutionTime => BackgroundJob.Schedule(queue, methodCall, bestExecutionTime.ExecutionTime));
    }

    /// <summary>
    /// Creates a new fire-and-forget job based on a given method call expression.
    /// </summary>
    /// <param name="methodCall">Method call expression that will be marshalled to a server.</param>
    /// <param name="latestExecutionTime">The latest possible time that the job should be finished</param>
    /// <param name="estimatedJobDuration">Estimated duration the job will take. The latest starting time is calculated as latestExecutionTime - estimatedJobDuration</param>
    /// <returns>Unique identifier of a background job.</returns>
    /// 
    /// <exception cref="ArgumentNullException">
    /// <paramref name="methodCall"/> is <see langword="null"/>.
    /// </exception>
    /// 
    /// <seealso cref="O:Hangfire.IBackgroundJobClient.Enqueue"/>
    public static async Task<string> EnqueueAsync<T>(Expression<Func<T, Task>> methodCall, DateTimeOffset latestExecutionTime, TimeSpan estimatedJobDuration)
    {
        var scheduleOptions = await GetBestScheduleTime(latestExecutionTime, estimatedJobDuration);
        return scheduleOptions.Match(
            _ => BackgroundJob.Enqueue(methodCall),
            bestExecutionTime => BackgroundJob.Schedule(methodCall, bestExecutionTime.ExecutionTime));
    }

    /// <summary>
    /// Creates a new fire-and-forget job based on a given method call expression and places it
    /// to the specified queue.
    /// </summary>
    /// <param name="queue">Default queue for the background job.</param>
    /// <param name="methodCall">Method call expression that will be marshalled to a server.</param>
    /// <param name="latestExecutionTime">The latest possible time that the job should be finished</param>
    /// <param name="estimatedJobDuration">Estimated duration the job will take. The latest starting time is calculated as latestExecutionTime - estimatedJobDuration</param>
    /// <returns>Unique identifier of a background job.</returns>
    ///
    /// <exception cref="ArgumentNullException">
    /// <paramref name="methodCall"/> is <see langword="null"/>.
    /// </exception>
    ///
    /// <seealso cref="O:Hangfire.IBackgroundJobClient.Enqueue"/>
    public static async Task<string> EnqueueAsync<T>([NotNull] string queue, Expression<Func<T, Task>> methodCall, DateTimeOffset latestExecutionTime, TimeSpan estimatedJobDuration)
    {
        var scheduleOptions = await GetBestScheduleTime(latestExecutionTime, estimatedJobDuration);
        return scheduleOptions.Match(
            _ => BackgroundJob.Enqueue(queue, methodCall),
            bestExecutionTime => BackgroundJob.Schedule(queue, methodCall, bestExecutionTime.ExecutionTime));
    }

    /// <summary>
    /// Creates a new background job based on a specified method
    /// call expression and schedules it to be enqueued after a given delay.
    /// </summary>
    /// 
    /// <param name="methodCall">Instance method call expression that will be marshalled to the Server.</param>
    /// <param name="delay">Delay, after which the job will be enqueued.</param>
    /// <param name="earliestDelay">The earliest start date compare to the delay</param>
    /// <param name="latestDelay">The latest start date compare to the delay</param>
    /// <param name="estimatedJobDuration">Estimated duration the job will take. The latest starting time is calculated as latestExecutionTime - estimatedJobDuration</param>
    /// <returns>Unique identifier of the created job.</returns>
    public static async Task<string> ScheduleAsync(
        Expression<Action> methodCall, TimeSpan delay, TimeSpan earliestDelay, TimeSpan latestDelay, TimeSpan estimatedJobDuration
        )
    {
        var scheduleOptions = await GetBestScheduleTime(DateTimeOffset.Now - earliestDelay, DateTimeOffset.Now + latestDelay, estimatedJobDuration);
        return scheduleOptions.Match(
            _ => BackgroundJob.Schedule(methodCall, delay),
            bestExecutionTime => BackgroundJob.Schedule(methodCall, bestExecutionTime.ExecutionTime));
    }

    /// <summary>
    /// Creates a new background job based on a specified method call expression and schedules it
    /// to be enqueued to the specified queue after a given delay.
    /// </summary>
    ///
    /// <param name="queue">Default queue for the background job.</param>
    /// <param name="methodCall">Instance method call expression that will be marshalled to the Server.</param>
    /// <param name="delay">Delay, after which the job will be enqueued.</param>
    /// <param name="earliestDelay">The earliest start date compare to the delay</param>
    /// <param name="latestDelay">The latest start date compare to the delay</param>
    /// <param name="estimatedJobDuration">Estimated duration the job will take. The latest starting time is calculated as latestExecutionTime - estimatedJobDuration</param>
    /// <returns>Unique identifier of the created job.</returns>
    public static async Task<string> ScheduleAsync(
        [NotNull] string queue,
        Expression<Action> methodCall, TimeSpan delay, TimeSpan earliestDelay, TimeSpan latestDelay, TimeSpan estimatedJobDuration)
    {
        var scheduleOptions = await GetBestScheduleTime(DateTimeOffset.Now - earliestDelay, DateTimeOffset.Now + latestDelay, estimatedJobDuration);
        return scheduleOptions.Match(
            _ => BackgroundJob.Schedule(queue, methodCall, delay),
            bestExecutionTime => BackgroundJob.Schedule(queue, methodCall, bestExecutionTime.ExecutionTime));
    }

    /// <summary>
    /// Creates a new background job based on a specified method
    /// call expression and schedules it to be enqueued after a given delay.
    /// </summary>
    /// 
    /// <param name="methodCall">Instance method call expression that will be marshalled to the Server.</param>
    /// <param name="delay">Delay, after which the job will be enqueued.</param>
    /// <param name="earliestDelay">The earliest start date compare to the delay</param>
    /// <param name="latestDelay">The latest start date compare to the delay</param>
    /// <param name="estimatedJobDuration">Estimated duration the job will take. The latest starting time is calculated as latestExecutionTime - estimatedJobDuration</param>
    /// <returns>Unique identifier of the created job.</returns>
    public static async Task<string> ScheduleAsync(
        Expression<Func<Task>> methodCall, TimeSpan delay, TimeSpan earliestDelay, TimeSpan latestDelay, TimeSpan estimatedJobDuration)
    {
        var scheduleOptions = await GetBestScheduleTime(DateTimeOffset.Now - earliestDelay, DateTimeOffset.Now + latestDelay, estimatedJobDuration);
        return scheduleOptions.Match(
            _ => BackgroundJob.Schedule(methodCall, delay),
            bestExecutionTime => BackgroundJob.Schedule(methodCall, bestExecutionTime.ExecutionTime));
    }

    /// <summary>
    /// Creates a new background job based on a specified method call expression and schedules it
    /// to be enqueued to the specified queue after a given delay.
    /// </summary>
    ///
    /// <param name="queue">Default queue for the background job.</param>
    /// <param name="methodCall">Instance method call expression that will be marshalled to the Server.</param>
    /// <param name="delay">Delay, after which the job will be enqueued.</param>
    /// <param name="earliestDelay">The earliest start date compare to the delay</param>
    /// <param name="latestDelay">The latest start date compare to the delay</param>
    /// <param name="estimatedJobDuration">Estimated duration the job will take. The latest starting time is calculated as latestExecutionTime - estimatedJobDuration</param>
    /// <returns>Unique identifier of the created job.</returns>
    public static async Task<string> ScheduleAsync(
        [NotNull] string queue,
        Expression<Func<Task>> methodCall, TimeSpan delay, TimeSpan earliestDelay, TimeSpan latestDelay, TimeSpan estimatedJobDuration)
    {
        var scheduleOptions = await GetBestScheduleTime(DateTimeOffset.Now - earliestDelay, DateTimeOffset.Now + latestDelay, estimatedJobDuration);
        return scheduleOptions.Match(
            _ => BackgroundJob.Schedule(queue, methodCall, delay),
            bestExecutionTime => BackgroundJob.Schedule(queue, methodCall, bestExecutionTime.ExecutionTime));
    }

    /// <summary>
    /// Creates a new background job based on a specified method call expression
    /// and schedules it to be enqueued at the given moment of time.
    /// </summary>
    /// 
    /// <param name="methodCall">Method call expression that will be marshalled to the Server.</param>
    /// <param name="enqueueAt">The moment of time at which the job will be enqueued.</param>
    /// <param name="earliestDelay">The earliest start date compare to the delay</param>
    /// <param name="latestDelay">The latest start date compare to the delay</param>
    /// <param name="estimatedJobDuration">Estimated duration the job will take. The latest starting time is calculated as latestExecutionTime - estimatedJobDuration</param>
    /// <returns>Unique identifier of a created job.</returns>
    public static async Task<string> ScheduleAsync(
        Expression<Action> methodCall,
        DateTimeOffset enqueueAt, TimeSpan earliestDelay, TimeSpan latestDelay, TimeSpan estimatedJobDuration)
    {
        var scheduleOptions = await GetBestScheduleTime(DateTimeOffset.Now - earliestDelay, DateTimeOffset.Now + latestDelay, estimatedJobDuration);
        return scheduleOptions.Match(
            _ => BackgroundJob.Schedule(methodCall, enqueueAt),
            bestExecutionTime => BackgroundJob.Schedule(methodCall, bestExecutionTime.ExecutionTime));
    }

    /// <summary>
    /// Creates a new background job based on a specified method call expression and schedules it
    /// to be enqueued to the specified queue at the given moment of time.
    /// </summary>
    ///
    /// <param name="queue">Default queue for the background job.</param>
    /// <param name="methodCall">Method call expression that will be marshalled to the Server.</param>
    /// <param name="enqueueAt">The moment of time at which the job will be enqueued.</param>
    /// <param name="earliestDelay">The earliest start date compare to the delay</param>
    /// <param name="latestDelay">The latest start date compare to the delay</param>
    /// <param name="estimatedJobDuration">Estimated duration the job will take. The latest starting time is calculated as latestExecutionTime - estimatedJobDuration</param>
    /// <returns>Unique identifier of a created job.</returns>
    public static async Task<string> ScheduleAsync(
        [NotNull] string queue,
        Expression<Action> methodCall,
        DateTimeOffset enqueueAt, TimeSpan earliestDelay, TimeSpan latestDelay, TimeSpan estimatedJobDuration)
    {
        var scheduleOptions = await GetBestScheduleTime(DateTimeOffset.Now - earliestDelay, DateTimeOffset.Now + latestDelay, estimatedJobDuration);
        return scheduleOptions.Match(
            _ => BackgroundJob.Schedule(queue, methodCall, enqueueAt),
            bestExecutionTime => BackgroundJob.Schedule(queue, methodCall, bestExecutionTime.ExecutionTime));
    }

    /// <summary>
    /// Creates a new background job based on a specified method call expression
    /// and schedules it to be enqueued at the given moment of time.
    /// </summary>
    /// 
    /// <param name="methodCall">Method call expression that will be marshalled to the Server.</param>
    /// <param name="enqueueAt">The moment of time at which the job will be enqueued.</param>
    /// <param name="earliestDelay">The earliest start date compare to the delay</param>
    /// <param name="latestDelay">The latest start date compare to the delay</param>
    /// <param name="estimatedJobDuration">Estimated duration the job will take. The latest starting time is calculated as latestExecutionTime - estimatedJobDuration</param>
    /// <returns>Unique identifier of a created job.</returns>
    public static async Task<string> ScheduleAsync(
        Expression<Func<Task>> methodCall,
        DateTimeOffset enqueueAt, TimeSpan earliestDelay, TimeSpan latestDelay, TimeSpan estimatedJobDuration)
    {
        var scheduleOptions = await GetBestScheduleTime(DateTimeOffset.Now - earliestDelay, DateTimeOffset.Now + latestDelay, estimatedJobDuration);
        return scheduleOptions.Match(
            _ => BackgroundJob.Schedule(methodCall, enqueueAt),
            bestExecutionTime => BackgroundJob.Schedule(methodCall, bestExecutionTime.ExecutionTime));
    }

    /// <summary>
    /// Creates a new background job based on a specified method call expression and schedules it
    /// to be enqueued to the specified queue at the given moment of time.
    /// </summary>
    ///
    /// <param name="queue">Default queue for the background job.</param>
    /// <param name="methodCall">Method call expression that will be marshalled to the Server.</param>
    /// <param name="enqueueAt">The moment of time at which the job will be enqueued.</param>
    /// <param name="earliestDelay">The earliest start date compare to the delay</param>
    /// <param name="latestDelay">The latest start date compare to the delay</param>
    /// <param name="estimatedJobDuration">Estimated duration the job will take. The latest starting time is calculated as latestExecutionTime - estimatedJobDuration</param>
    /// <returns>Unique identifier of a created job.</returns>
    public static async Task<string> ScheduleAsync(
        [NotNull] string queue,
        Expression<Func<Task>> methodCall,
        DateTimeOffset enqueueAt, TimeSpan earliestDelay, TimeSpan latestDelay, TimeSpan estimatedJobDuration)
    {
        var scheduleOptions = await GetBestScheduleTime(DateTimeOffset.Now - earliestDelay, DateTimeOffset.Now + latestDelay, estimatedJobDuration);
        return scheduleOptions.Match(
            _ => BackgroundJob.Schedule(queue, methodCall, enqueueAt),
            bestExecutionTime => BackgroundJob.Schedule(queue, methodCall, bestExecutionTime.ExecutionTime));
    }

    /// <summary>
    /// Creates a new background job based on a specified instance method
    /// call expression and schedules it to be enqueued after a given delay.
    /// </summary>
    /// 
    /// <typeparam name="T">Type whose method will be invoked during job processing.</typeparam>
    /// <param name="methodCall">Instance method call expression that will be marshalled to the Server.</param>
    /// <param name="delay">Delay, after which the job will be enqueued.</param>
    /// <param name="earliestDelay">The earliest start date compare to the delay</param>
    /// <param name="latestDelay">The latest start date compare to the delay</param>
    /// <param name="estimatedJobDuration">Estimated duration the job will take. The latest starting time is calculated as latestExecutionTime - estimatedJobDuration</param>
    /// <returns>Unique identifier of the created job.</returns>
    public static async Task<string> ScheduleAsync<T>(
        Expression<Action<T>> methodCall,
        TimeSpan delay, TimeSpan earliestDelay, TimeSpan latestDelay, TimeSpan estimatedJobDuration)
    {
        var scheduleOptions = await GetBestScheduleTime(DateTimeOffset.Now - earliestDelay, DateTimeOffset.Now + latestDelay, estimatedJobDuration);
        return scheduleOptions.Match(
            _ => BackgroundJob.Schedule(methodCall, delay),
            bestExecutionTime => BackgroundJob.Schedule(methodCall, bestExecutionTime.ExecutionTime));
    }

    /// <summary>
    /// Creates a new background job based on a specified instance method call expression and schedules
    /// it to be enqueued to the specified queue after a given delay.
    /// </summary>
    ///
    /// <typeparam name="T">Type whose method will be invoked during job processing.</typeparam>
    /// <param name="queue">Default queue for the background job.</param>
    /// <param name="methodCall">Instance method call expression that will be marshalled to the Server.</param>
    /// <param name="delay">Delay, after which the job will be enqueued.</param>
    /// <param name="earliestDelay">The earliest start date compare to the delay</param>
    /// <param name="latestDelay">The latest start date compare to the delay</param>
    /// <param name="estimatedJobDuration">Estimated duration the job will take. The latest starting time is calculated as latestExecutionTime - estimatedJobDuration</param>
    /// <returns>Unique identifier of the created job.</returns>
    public static async Task<string> ScheduleAsync<T>(
        [NotNull] string queue,
        Expression<Action<T>> methodCall,
        TimeSpan delay, TimeSpan earliestDelay, TimeSpan latestDelay, TimeSpan estimatedJobDuration)
    {
        var scheduleOptions = await GetBestScheduleTime(DateTimeOffset.Now - earliestDelay, DateTimeOffset.Now + latestDelay, estimatedJobDuration);
        return scheduleOptions.Match(
            _ => BackgroundJob.Schedule(queue, methodCall, delay),
            bestExecutionTime => BackgroundJob.Schedule(queue, methodCall, bestExecutionTime.ExecutionTime));
    }

    /// <summary>
    /// Creates a new background job based on a specified instance method
    /// call expression and schedules it to be enqueued after a given delay.
    /// </summary>
    /// 
    /// <typeparam name="T">Type whose method will be invoked during job processing.</typeparam>
    /// <param name="methodCall">Instance method call expression that will be marshalled to the Server.</param>
    /// <param name="delay">Delay, after which the job will be enqueued.</param>
    /// <param name="earliestDelay">The earliest start date compare to the delay</param>
    /// <param name="latestDelay">The latest start date compare to the delay</param>
    /// <param name="estimatedJobDuration">Estimated duration the job will take. The latest starting time is calculated as latestExecutionTime - estimatedJobDuration</param>
    /// <returns>Unique identifier of the created job.</returns>
    public static async Task<string> ScheduleAsync<T>(
        Expression<Func<T, Task>> methodCall,
        TimeSpan delay, TimeSpan earliestDelay, TimeSpan latestDelay, TimeSpan estimatedJobDuration)
    {
        var scheduleOptions = await GetBestScheduleTime(DateTimeOffset.Now - earliestDelay, DateTimeOffset.Now + latestDelay, estimatedJobDuration);
        return scheduleOptions.Match(
            _ => BackgroundJob.Schedule(methodCall, delay),
            bestExecutionTime => BackgroundJob.Schedule(methodCall, bestExecutionTime.ExecutionTime));
    }

    /// <summary>
    /// Creates a new background job based on a specified instance method call expression and schedules
    /// it to be enqueued to the specified queue after a given delay.
    /// </summary>
    ///
    /// <typeparam name="T">Type whose method will be invoked during job processing.</typeparam>
    /// <param name="queue">Default queue for the background job.</param>
    /// <param name="methodCall">Instance method call expression that will be marshalled to the Server.</param>
    /// <param name="delay">Delay, after which the job will be enqueued.</param>
    /// <param name="earliestDelay">The earliest start date compare to the delay</param>
    /// <param name="latestDelay">The latest start date compare to the delay</param>
    /// <param name="estimatedJobDuration">Estimated duration the job will take. The latest starting time is calculated as latestExecutionTime - estimatedJobDuration</param>
    /// <returns>Unique identifier of the created job.</returns>
    public static async Task<string> ScheduleAsync<T>(
        [NotNull] string queue,
        Expression<Func<T, Task>> methodCall,
        TimeSpan delay, TimeSpan earliestDelay, TimeSpan latestDelay, TimeSpan estimatedJobDuration)
    {
        var scheduleOptions = await GetBestScheduleTime(DateTimeOffset.Now - earliestDelay, DateTimeOffset.Now + latestDelay, estimatedJobDuration);
        return scheduleOptions.Match(
            _ => BackgroundJob.Schedule(queue, methodCall, delay),
            bestExecutionTime => BackgroundJob.Schedule(queue, methodCall, bestExecutionTime.ExecutionTime));
    }

    /// <summary>
    /// Creates a new background job based on a specified method call expression
    /// and schedules it to be enqueued at the given moment of time.
    /// </summary>
    /// 
    /// <typeparam name="T">The type whose method will be invoked during the job processing.</typeparam>
    /// <param name="methodCall">Method call expression that will be marshalled to the Server.</param>
    /// <param name="enqueueAt">The moment of time at which the job will be enqueued.</param>
    /// <param name="earliestDelay">The earliest start date compare to the delay</param>
    /// <param name="latestDelay">The latest start date compare to the delay</param>
    /// <param name="estimatedJobDuration">Estimated duration the job will take. The latest starting time is calculated as latestExecutionTime - estimatedJobDuration</param>
    /// <returns>Unique identifier of a created job.</returns>
    public static async Task<string> ScheduleAsync<T>(
        Expression<Action<T>> methodCall,
        DateTimeOffset enqueueAt, TimeSpan earliestDelay, TimeSpan latestDelay, TimeSpan estimatedJobDuration)
    {
        var scheduleOptions = await GetBestScheduleTime(DateTimeOffset.Now - earliestDelay, DateTimeOffset.Now + latestDelay, estimatedJobDuration);
        return scheduleOptions.Match(
            _ => BackgroundJob.Schedule(methodCall, enqueueAt),
            bestExecutionTime => BackgroundJob.Schedule(methodCall, bestExecutionTime.ExecutionTime));
    }

    /// <summary>
    /// Creates a new background job based on a specified method call expression and schedules it
    /// to be enqueued to the specified queue at the given moment of time.
    /// </summary>
    ///
    /// <typeparam name="T">The type whose method will be invoked during the job processing.</typeparam>
    /// <param name="queue">Default queue for the background job.</param>
    /// <param name="methodCall">Method call expression that will be marshalled to the Server.</param>
    /// <param name="enqueueAt">The moment of time at which the job will be enqueued.</param>
    /// <param name="earliestDelay">The earliest start date compare to the delay</param>
    /// <param name="latestDelay">The latest start date compare to the delay</param>
    /// <param name="estimatedJobDuration">Estimated duration the job will take. The latest starting time is calculated as latestExecutionTime - estimatedJobDuration</param>
    /// <returns>Unique identifier of a created job.</returns>
    public static async Task<string> ScheduleAsync<T>(
        [NotNull] string queue,
        Expression<Action<T>> methodCall,
        DateTimeOffset enqueueAt, TimeSpan earliestDelay, TimeSpan latestDelay, TimeSpan estimatedJobDuration)
    {
        var scheduleOptions = await GetBestScheduleTime(DateTimeOffset.Now - earliestDelay, DateTimeOffset.Now + latestDelay, estimatedJobDuration);
        return scheduleOptions.Match(
            _ => BackgroundJob.Schedule(queue, methodCall, enqueueAt),
            bestExecutionTime => BackgroundJob.Schedule(queue, methodCall, bestExecutionTime.ExecutionTime));
    }

    /// <summary>
    /// Creates a new background job based on a specified method call expression
    /// and schedules it to be enqueued at the given moment of time.
    /// </summary>
    /// 
    /// <typeparam name="T">The type whose method will be invoked during the job processing.</typeparam>
    /// <param name="methodCall">Method call expression that will be marshalled to the Server.</param>
    /// <param name="enqueueAt">The moment of time at which the job will be enqueued.</param>
    /// <param name="earliestDelay">The earliest start date compare to the delay</param>
    /// <param name="latestDelay">The latest start date compare to the delay</param>
    /// <param name="estimatedJobDuration">Estimated duration the job will take. The latest starting time is calculated as latestExecutionTime - estimatedJobDuration</param>
    /// <returns>Unique identifier of a created job.</returns>
    public static async Task<string> ScheduleAsync<T>(
        Expression<Func<T, Task>> methodCall,
        DateTimeOffset enqueueAt, TimeSpan earliestDelay, TimeSpan latestDelay, TimeSpan estimatedJobDuration)
    {
        var scheduleOptions = await GetBestScheduleTime(DateTimeOffset.Now - earliestDelay, DateTimeOffset.Now + latestDelay, estimatedJobDuration);
        return scheduleOptions.Match(
            _ => BackgroundJob.Schedule(methodCall, enqueueAt),
            bestExecutionTime => BackgroundJob.Schedule(methodCall, bestExecutionTime.ExecutionTime));
    }

    /// <summary>
    /// Creates a new background job based on a specified method call expression and schedules it
    /// to be enqueued to the specified queue at the given moment of time.
    /// </summary>
    ///
    /// <typeparam name="T">The type whose method will be invoked during the job processing.</typeparam>
    /// <param name="queue">Default queue for the background job.</param>
    /// <param name="methodCall">Method call expression that will be marshalled to the Server.</param>
    /// <param name="enqueueAt">The moment of time at which the job will be enqueued.</param>
    /// <param name="earliestDelay">The earliest start date compare to the delay</param>
    /// <param name="latestDelay">The latest start date compare to the delay</param>
    /// <param name="estimatedJobDuration">Estimated duration the job will take. The latest starting time is calculated as latestExecutionTime - estimatedJobDuration</param>
    /// <returns>Unique identifier of a created job.</returns>
    public static async Task<string> ScheduleAsync<T>(
        [NotNull] string queue,
        Expression<Func<T, Task>> methodCall,
        DateTimeOffset enqueueAt, TimeSpan earliestDelay, TimeSpan latestDelay, TimeSpan estimatedJobDuration)
    {
        var scheduleOptions = await GetBestScheduleTime(DateTimeOffset.Now - earliestDelay, DateTimeOffset.Now + latestDelay, estimatedJobDuration);
        return scheduleOptions.Match(
            _ => BackgroundJob.Schedule(queue, methodCall, enqueueAt),
            bestExecutionTime => BackgroundJob.Schedule(queue, methodCall, bestExecutionTime.ExecutionTime));
    }
    private static async Task<ExecutionTime> GetBestScheduleTime(DateTimeOffset earliestExecutionTime, DateTimeOffset latestExecutionTime, TimeSpan estimatedJobDuration)
    {
        return await CarbonAwareExecutionForecast.GetBestScheduleTime(earliestExecutionTime, latestExecutionTime, estimatedJobDuration);
    }
    private static async Task<ExecutionTime> GetBestScheduleTime(DateTimeOffset latestExecutionTime, TimeSpan estimatedJobDuration)
        => await GetBestScheduleTime(DateTimeOffset.Now, latestExecutionTime, estimatedJobDuration);
}