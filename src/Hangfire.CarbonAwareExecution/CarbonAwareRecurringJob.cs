using System.Linq.Expressions;
using CarbonAwareComputing.Hangfire;

// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace Hangfire;

public static class CarbonAwareRecurringJob
{

    public static void AddOrUpdate(
        string recurringJobId,
        Expression<Action> methodCall,
        Func<string> cronExpression,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        AddOrUpdate(recurringJobId, methodCall, cronExpression, new RecurringJobOptions(), maxExecutionDelay, estimatedJobDuration);
    }

    public static void AddOrUpdate(
        string recurringJobId,
        Expression<Action> methodCall,
        Func<string> cronExpression,
        RecurringJobOptions options,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        if (cronExpression == null) throw new ArgumentNullException(nameof(cronExpression));
        AddOrUpdate(recurringJobId, methodCall, cronExpression(), options, maxExecutionDelay, estimatedJobDuration);
    }

    public static void AddOrUpdate(
        string recurringJobId,
        string queue,
        Expression<Action> methodCall,
        Func<string> cronExpression,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        AddOrUpdate(recurringJobId, queue, methodCall, cronExpression, new RecurringJobOptions(), maxExecutionDelay, estimatedJobDuration);
    }

    public static void AddOrUpdate(
        string recurringJobId,
        string queue,
        Expression<Action> methodCall,
        Func<string> cronExpression,
        RecurringJobOptions options,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        if (cronExpression == null) throw new ArgumentNullException(nameof(cronExpression));
        AddOrUpdate(recurringJobId, queue, methodCall, cronExpression(), options, maxExecutionDelay, estimatedJobDuration);
    }



    public static void AddOrUpdate<T>(
        string recurringJobId,
        Expression<Action<T>> methodCall,
        Func<string> cronExpression,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        AddOrUpdate(recurringJobId, methodCall, cronExpression, new RecurringJobOptions(), maxExecutionDelay, estimatedJobDuration);
    }

    public static void AddOrUpdate<T>(
        string recurringJobId,
        Expression<Action<T>> methodCall,
        Func<string> cronExpression,
        RecurringJobOptions options,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        if (cronExpression == null) throw new ArgumentNullException(nameof(cronExpression));
        AddOrUpdate(recurringJobId, methodCall, cronExpression(), options, maxExecutionDelay, estimatedJobDuration);
    }

    public static void AddOrUpdate<T>(
        string recurringJobId,
        string queue,
        Expression<Action<T>> methodCall,
        Func<string> cronExpression,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        AddOrUpdate(recurringJobId, queue, methodCall, cronExpression, new RecurringJobOptions(), maxExecutionDelay, estimatedJobDuration);
    }

    public static void AddOrUpdate<T>(
        string recurringJobId,
        string queue,
        Expression<Action<T>> methodCall,
        Func<string> cronExpression,
        RecurringJobOptions options,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        if (cronExpression == null) throw new ArgumentNullException(nameof(cronExpression));
        AddOrUpdate(recurringJobId, queue, methodCall, cronExpression(), options, maxExecutionDelay, estimatedJobDuration);
    }



    public static void AddOrUpdate(
        string recurringJobId,
        Expression<Action> methodCall,
        string cronExpression,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        AddOrUpdate(recurringJobId, methodCall, cronExpression, new RecurringJobOptions(), maxExecutionDelay, estimatedJobDuration);
    }

    public static void AddOrUpdate(
        string recurringJobId,
        Expression<Action> methodCall,
        string cronExpression,
        RecurringJobOptions options,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        RecurringJob.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            methodCall,
            cronExpression,
            options
            );
    }

    public static void AddOrUpdate(
        string recurringJobId,
        string queue,
        Expression<Action> methodCall,
        string cronExpression,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        AddOrUpdate(recurringJobId, queue, methodCall, cronExpression, new RecurringJobOptions(), maxExecutionDelay, estimatedJobDuration);
    }

    public static void AddOrUpdate(
        string recurringJobId,
        string queue,
        Expression<Action> methodCall,
        string cronExpression,
        RecurringJobOptions options,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        RecurringJob.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            queue,
            methodCall,
            cronExpression,
            options
        );
    }


    public static void AddOrUpdate<T>(
        string recurringJobId,
        Expression<Action<T>> methodCall,
        string cronExpression,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        AddOrUpdate(recurringJobId, methodCall, cronExpression, new RecurringJobOptions(), maxExecutionDelay, estimatedJobDuration);
    }

    public static void AddOrUpdate<T>(
        string recurringJobId,
        Expression<Action<T>> methodCall,
        string cronExpression,
        RecurringJobOptions options,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        RecurringJob.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            methodCall,
            cronExpression,
            options
        );
    }

    public static void AddOrUpdate<T>(
        string recurringJobId,
        string queue,
        Expression<Action<T>> methodCall,
        string cronExpression,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        AddOrUpdate(recurringJobId, queue, methodCall, cronExpression, new RecurringJobOptions(), maxExecutionDelay, estimatedJobDuration);
    }

    public static void AddOrUpdate<T>(
        string recurringJobId,
        string queue,
        Expression<Action<T>> methodCall,
        string cronExpression,
        RecurringJobOptions options,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        RecurringJob.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
          queue,
            methodCall,
            cronExpression,
            options
        );
    }
    public static void AddOrUpdate(
        string recurringJobId,
        Expression<Func<Task>> methodCall,
        Func<string> cronExpression,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        AddOrUpdate(recurringJobId, methodCall, cronExpression, new RecurringJobOptions(), maxExecutionDelay, estimatedJobDuration);
    }

    public static void AddOrUpdate(
        string recurringJobId,
        Expression<Func<Task>> methodCall,
        Func<string> cronExpression,
        RecurringJobOptions options,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        RecurringJob.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            methodCall,
            cronExpression,
            options
        );
    }

    public static void AddOrUpdate(
        string recurringJobId,
        string queue,
        Expression<Func<Task>> methodCall,
        Func<string> cronExpression,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        AddOrUpdate(recurringJobId, queue, methodCall, cronExpression, new RecurringJobOptions(), maxExecutionDelay, estimatedJobDuration);
    }

    public static void AddOrUpdate(
        string recurringJobId,
        string queue,
        Expression<Func<Task>> methodCall,
        Func<string> cronExpression,
        RecurringJobOptions options,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        if (cronExpression == null) throw new ArgumentNullException(nameof(cronExpression));
        AddOrUpdate(recurringJobId, queue, methodCall, cronExpression(), options, maxExecutionDelay, estimatedJobDuration);
    }

    public static void AddOrUpdate<T>(
        string recurringJobId,
        Expression<Func<T, Task>> methodCall,
        Func<string> cronExpression,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        AddOrUpdate(recurringJobId, methodCall, cronExpression, new RecurringJobOptions(), maxExecutionDelay, estimatedJobDuration);
    }

    public static void AddOrUpdate<T>(
        string recurringJobId,
        Expression<Func<T, Task>> methodCall,
        Func<string> cronExpression,
        RecurringJobOptions options,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        if (cronExpression == null) throw new ArgumentNullException(nameof(cronExpression));
        AddOrUpdate(recurringJobId, methodCall, cronExpression(), options, maxExecutionDelay, estimatedJobDuration);
    }

    public static void AddOrUpdate<T>(
        string recurringJobId,
        string queue,
        Expression<Func<T, Task>> methodCall,
        Func<string> cronExpression,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        AddOrUpdate(recurringJobId, queue, methodCall, cronExpression, new RecurringJobOptions(), maxExecutionDelay, estimatedJobDuration);
    }

    public static void AddOrUpdate<T>(
        string recurringJobId,
        string queue,
        Expression<Func<T, Task>> methodCall,
        Func<string> cronExpression,
        RecurringJobOptions options,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        if (cronExpression == null) throw new ArgumentNullException(nameof(cronExpression));
        AddOrUpdate(recurringJobId, queue, methodCall, cronExpression(), options, maxExecutionDelay, estimatedJobDuration);
    }

    public static void AddOrUpdate(
         string recurringJobId,
         Expression<Func<Task>> methodCall,
         string cronExpression,
         TimeSpan maxExecutionDelay,
         TimeSpan estimatedJobDuration)
    {
        RecurringJob.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            methodCall,
            cronExpression
        );
    }

    public static void AddOrUpdate(
        string recurringJobId,
        string queue,
        Expression<Func<Task>> methodCall,
        string cronExpression,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        AddOrUpdate(recurringJobId, queue, methodCall, cronExpression, new RecurringJobOptions(), maxExecutionDelay, estimatedJobDuration);
    }

    public static void AddOrUpdate(
        string recurringJobId,
        string queue,
        Expression<Func<Task>> methodCall,
        string cronExpression,
        RecurringJobOptions options,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        RecurringJob.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            queue,
            methodCall,
            cronExpression,
            options
        );
    }

    public static void AddOrUpdate<T>(
         string recurringJobId,
         Expression<Func<T, Task>> methodCall,
         string cronExpression,
         TimeSpan maxExecutionDelay,
         TimeSpan estimatedJobDuration)
    {
        AddOrUpdate(recurringJobId, methodCall, cronExpression, new RecurringJobOptions(), maxExecutionDelay, estimatedJobDuration);
    }

    public static void AddOrUpdate<T>(
        string recurringJobId,
        Expression<Func<T, Task>> methodCall,
        string cronExpression,
        RecurringJobOptions options,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        RecurringJob.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            methodCall,
            cronExpression,
            options
        );
    }

    public static void AddOrUpdate<T>(
        string recurringJobId,
        string queue,
        Expression<Func<T, Task>> methodCall,
        string cronExpression,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        AddOrUpdate(recurringJobId, queue, methodCall, cronExpression, new RecurringJobOptions(), maxExecutionDelay, estimatedJobDuration);
    }

    public static void AddOrUpdate<T>(
        string recurringJobId,
        string queue,
        Expression<Func<T, Task>> methodCall,
        string cronExpression,
        RecurringJobOptions options,
        TimeSpan maxExecutionDelay,
        TimeSpan estimatedJobDuration)
    {
        RecurringJob.AddOrUpdate(
            RecurringJobExecutionWindowSerializer.SerializeInJobId(recurringJobId, maxExecutionDelay, estimatedJobDuration),
            queue,
            methodCall,
            cronExpression,
            options
        );
    }

}