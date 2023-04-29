using System.Linq.Expressions;
using Hangfire.CarbonAwareExecution;
using Hangfire.Common;

// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace Hangfire;

public static class RecurringJobManagerExtensions
{
    public static void AddOrUpdateCarbonAware(
        this IRecurringJobManager manager,
        string recurringJobId,
        Job job,
        string cronExpression,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        manager.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            job,
            cronExpression);
    }

    public static void AddOrUpdateCarbonAware(
        this IRecurringJobManager manager,
        string recurringJobId,
        Expression<Action> methodCall,
        Func<string> cronExpression,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration
        )
    {
        manager.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration), 
            methodCall, 
            cronExpression);
    }

    public static void AddOrUpdateCarbonAware(
        this IRecurringJobManager manager,
        string recurringJobId,
        Expression<Action> methodCall,
        Func<string> cronExpression,
        RecurringJobOptions options,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        manager.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            methodCall,
            cronExpression,
            options);
    }

    public static void AddOrUpdateCarbonAware(
        this IRecurringJobManager manager,
        string recurringJobId,
        string queue,
        Expression<Action> methodCall,
        Func<string> cronExpression,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        manager.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            queue,
            methodCall,
            cronExpression);
    }

    public static void AddOrUpdateCarbonAware(
        this IRecurringJobManager manager,
        string recurringJobId,
        string queue,
        Expression<Action> methodCall,
        Func<string> cronExpression,
        RecurringJobOptions options,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        manager.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            queue,
            methodCall,
            cronExpression,
            options);
    }

    public static void AddOrUpdateCarbonAware<T>(
         this IRecurringJobManager manager,
         string recurringJobId,
         Expression<Action<T>> methodCall,
         Func<string> cronExpression,
         TimeSpan maxExecutionDelay,
         TimeSpan estimatedJobDuration)
    {
        manager.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration), 
            methodCall, 
            cronExpression);
    }

    public static void AddOrUpdateCarbonAware<T>(
        this IRecurringJobManager manager,
        string recurringJobId,
        Expression<Action<T>> methodCall,
        Func<string> cronExpression,
        RecurringJobOptions options,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        manager.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            methodCall,
            cronExpression,
            options);
    }

    public static void AddOrUpdateCarbonAware<T>(
        this IRecurringJobManager manager,
        string recurringJobId,
        string queue,
        Expression<Action<T>> methodCall,
        Func<string> cronExpression,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        manager.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            queue,
            methodCall,
            cronExpression);
    }

    public static void AddOrUpdateCarbonAware<T>(
        this IRecurringJobManager manager,
        string recurringJobId,
        string queue,
        Expression<Action<T>> methodCall,
        Func<string> cronExpression,
        RecurringJobOptions options,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        manager.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            queue,
            methodCall,
            cronExpression, 
            options);
    }

    public static void AddOrUpdateCarbonAware(
        this IRecurringJobManager manager,
        string recurringJobId,
        Expression<Action> methodCall,
        string cronExpression,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        manager.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            methodCall,
            cronExpression);
    }

    public static void AddOrUpdateCarbonAware(
        this IRecurringJobManager manager,
        string recurringJobId,
        Expression<Action> methodCall,
        string cronExpression,
        RecurringJobOptions options,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        manager.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            methodCall,
            cronExpression,
            options);
    }

    public static void AddOrUpdateCarbonAware(
        this IRecurringJobManager manager,
        string recurringJobId,
        string queue,
        Expression<Action> methodCall,
        string cronExpression,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        manager.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            queue, 
            methodCall,
            cronExpression);
    }

    public static void AddOrUpdateCarbonAware(
        this IRecurringJobManager manager,
        string recurringJobId,
        string queue,
        Expression<Action> methodCall,
        string cronExpression,
        RecurringJobOptions options,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        manager.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            queue,
            methodCall,
            cronExpression, 
            options);
    }

    public static void AddOrUpdateCarbonAware<T>(
        this IRecurringJobManager manager,
        string recurringJobId,
        Expression<Action<T>> methodCall,
        string cronExpression,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        manager.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            methodCall,
            cronExpression);
    }

    public static void AddOrUpdateCarbonAware<T>(
        this IRecurringJobManager manager,
        string recurringJobId,
        Expression<Action<T>> methodCall,
        string cronExpression,
        RecurringJobOptions options,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        manager.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            methodCall,
            cronExpression, 
            options);
    }

    public static void AddOrUpdateCarbonAware<T>(
        this IRecurringJobManager manager,
        string recurringJobId,
        string queue,
        Expression<Action<T>> methodCall,
        string cronExpression,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        manager.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            queue, 
            methodCall,
            cronExpression);
    }

    public static void AddOrUpdateCarbonAware<T>(
        this IRecurringJobManager manager,
        string recurringJobId,
        string queue,
        Expression<Action<T>> methodCall,
        string cronExpression,
        RecurringJobOptions options,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        manager.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            queue,
            methodCall,
            cronExpression,
            options);
    }

    public static void AddOrUpdateCarbonAware(
        this IRecurringJobManager manager,
        string recurringJobId,
        Expression<Func<Task>> methodCall,
        Func<string> cronExpression,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        manager.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            methodCall,
            cronExpression);
    }

    public static void AddOrUpdateCarbonAware(
        this IRecurringJobManager manager,
        string recurringJobId,
        Expression<Func<Task>> methodCall,
        Func<string> cronExpression,
        RecurringJobOptions options,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        manager.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            methodCall,
            cronExpression,
            options);
    }

    public static void AddOrUpdateCarbonAware(
        this IRecurringJobManager manager,
        string recurringJobId,
        string queue,
        Expression<Func<Task>> methodCall,
        Func<string> cronExpression,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        manager.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            queue,
            methodCall,
            cronExpression);
    }

    public static void AddOrUpdateCarbonAware(
        this IRecurringJobManager manager,
        string recurringJobId,
        string queue,
        Expression<Func<Task>> methodCall,
        Func<string> cronExpression,
        RecurringJobOptions options,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        manager.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            queue,
            methodCall,
            cronExpression,
            options);
    }

    public static void AddOrUpdateCarbonAware<T>(
        this IRecurringJobManager manager,
        string recurringJobId,
        Expression<Func<T, Task>> methodCall,
        Func<string> cronExpression,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        manager.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            methodCall,
            cronExpression);
    }

    public static void AddOrUpdateCarbonAware<T>(
        this IRecurringJobManager manager,
        string recurringJobId,
        Expression<Func<T, Task>> methodCall,
        Func<string> cronExpression,
        RecurringJobOptions options,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        manager.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            methodCall,
            cronExpression,
            options);
    }

    public static void AddOrUpdateCarbonAware<T>(
        this IRecurringJobManager manager,
        string recurringJobId,
        string queue,
        Expression<Func<T, Task>> methodCall,
        Func<string> cronExpression,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration
        )
    {
        manager.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            queue,
            methodCall,
            cronExpression);
    }

    public static void AddOrUpdateCarbonAware<T>(
        this IRecurringJobManager manager,
        string recurringJobId,
        string queue,
        Expression<Func<T, Task>> methodCall,
        Func<string> cronExpression,
        RecurringJobOptions options,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        manager.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            queue,
            methodCall,
            cronExpression, 
            options
            );
    }

    public static void AddOrUpdateCarbonAware(
        this IRecurringJobManager manager,
        string recurringJobId,
        Expression<Func<Task>> methodCall,
        string cronExpression,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        manager.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            methodCall,
            cronExpression);
    }

    public static void AddOrUpdateCarbonAware(
        this IRecurringJobManager manager,
        string recurringJobId,
        Expression<Func<Task>> methodCall,
        string cronExpression,
        RecurringJobOptions options,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        manager.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            methodCall,
            cronExpression,
            options);
    }

    public static void AddOrUpdateCarbonAware(
        this IRecurringJobManager manager,
        string recurringJobId,
        string queue,
        Expression<Func<Task>> methodCall,
        string cronExpression,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        manager.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            queue,
            methodCall,
            cronExpression);
    }

    public static void AddOrUpdateCarbonAware(
        this IRecurringJobManager manager,
        string recurringJobId,
        string queue,
        Expression<Func<Task>> methodCall,
        string cronExpression,
        RecurringJobOptions options,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        manager.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            queue,
            methodCall,
            cronExpression,
            options);
    }

    public static void AddOrUpdateCarbonAware<T>(
        this IRecurringJobManager manager,
        string recurringJobId,
        Expression<Func<T, Task>> methodCall,
        string cronExpression,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        manager.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            methodCall,
            cronExpression);
    }

    public static void AddOrUpdateCarbonAware<T>(
        this IRecurringJobManager manager,
        string recurringJobId,
        Expression<Func<T, Task>> methodCall,
        string cronExpression,
        RecurringJobOptions options,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        manager.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            methodCall,
            cronExpression,
            options);
    }

    public static void AddOrUpdateCarbonAware<T>(
        this IRecurringJobManager manager,
        string recurringJobId,
        string queue,
        Expression<Func<T, Task>> methodCall,
        string cronExpression,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        manager.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            queue,
            methodCall,
            cronExpression);
    }

    public static void AddOrUpdateCarbonAware<T>(
        this IRecurringJobManager manager,
        string recurringJobId,
        string queue,
        Expression<Func<T, Task>> methodCall,
        string cronExpression,
        RecurringJobOptions options,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        manager.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            queue,
            methodCall,
            cronExpression,
            options);
    }

}