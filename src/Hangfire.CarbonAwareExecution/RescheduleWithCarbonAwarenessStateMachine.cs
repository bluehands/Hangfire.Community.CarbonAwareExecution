using System.Diagnostics.CodeAnalysis;
using CarbonAwareComputing;
using FunicularSwitch.Generators;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.States;
using Hangfire.Storage;

namespace Hangfire.Community.CarbonAwareExecution;

internal class RescheduleWithCarbonAwarenessStateMachine : IStateMachine
{
    private readonly IStateMachine m_Inner;
    private readonly Func<IBackgroundJobClient> m_GetBackgroundJobClient;
    private IBackgroundJobClient? m_Client;

    public RescheduleWithCarbonAwarenessStateMachine(IStateMachine inner, Func<IBackgroundJobClient> backgroundJobClient)
    {
        m_Inner = inner;
        m_GetBackgroundJobClient = backgroundJobClient;
    }

    public IState ApplyState(ApplyStateContext context)
    {
        return m_Inner.ApplyState(context);

        var backgroundJob = context.BackgroundJob;
        var job = backgroundJob.Job;
        var forecastParameter = job.GetCarbonAwareDelayParameter();
        if (forecastParameter == null)
        {
            return m_Inner.ApplyState(context);
        }

        //if (!RecurringJobExecutionWindowSerializer.TryDeserializeFromJob(context.BackgroundJob, out var forecastParameter))
        //{
        //    return m_Inner.ApplyState(context);
        //}

        var now = DateTimeOffset.Now;
        var scheduleOptions = CarbonAwareExecutionForecast.GetBestScheduleTime(now, now + forecastParameter.MaxExecutionDelay, forecastParameter.EstimatedJobDuration).GetAwaiter().GetResult();
        return scheduleOptions.Match(
            _ => m_Inner.ApplyState(context),
            bestExecutionTime =>
            {
                if (bestExecutionTime.ExecutionTime - now < TimeSpan.FromMinutes(1))
                    return m_Inner.ApplyState(context);

                forecastParameter.ParentRecurringJobId = JobExtensions.GetRecurringJobId(backgroundJob);
                var state = new ScheduledState(bestExecutionTime.ExecutionTime.ToUniversalTime().DateTime)
                {
                    Reason = $"Rescheduled job from {now} to {bestExecutionTime.ExecutionTime} for minimal carbon impact"
                };
                m_Client ??= m_GetBackgroundJobClient.Invoke();
                m_Client.Create(job, state);
                return state;
            }
        );
    }
}

[UnionType]
internal abstract partial record GetJobResult
{
    public record Job_(Job HangfireJob) : GetJobResult;
    public record Error_() : GetJobResult;
    public record NotFound_() : GetJobResult;
}

internal static class JobExtensions
{
    public static CarbonAwareDelayParameter? GetCarbonAwareDelayParameter(this Job job) => job.Args.OfType<CarbonAwareDelayParameter>().FirstOrDefault();
    public static string? GetRecurringJobId(this BackgroundJob backgroundJob) => backgroundJob.ParametersSnapshot.TryGetValue("RecurringJobId", out var recurringJobId) ? recurringJobId.Trim('\"') : null;
    public static GetJobResult TryGetRecurringJob(this IStorageConnection connection, string recurringJobId)
    {
        var parentJobEntry = connection.GetAllEntriesFromHash($"recurring-job:{recurringJobId}");
        if (parentJobEntry == null || parentJobEntry.TryGetValue("Job", out var payload) || !string.IsNullOrWhiteSpace(payload))
            return GetJobResult.NotFound();

        try
        {
            var invocationData = InvocationData.DeserializePayload(payload);
            var job = invocationData.DeserializeJob();
            return GetJobResult.Job(job);
        }
        catch (Exception)
        {
            return GetJobResult.Error();
        }
    }
}

internal class CustomBackgroundJobFactory : IBackgroundJobFactory
{

    private readonly IBackgroundJobFactory m_Inner;
    private readonly RescheduleWithCarbonAwarenessStateMachine m_StateMachine;

    public CustomBackgroundJobFactory(IBackgroundJobFactory inner, Func<IBackgroundJobClient> getBackgroundJobClient)
    {
        m_Inner = inner ?? throw new ArgumentNullException(nameof(inner));

        m_StateMachine = new RescheduleWithCarbonAwarenessStateMachine(m_Inner.StateMachine, getBackgroundJobClient);
    }

    public IStateMachine StateMachine => m_StateMachine;

    public BackgroundJob Create(CreateContext context)
    {
        //var recurringJobId = (string)context.Parameters["RecurringJobId"];
        //var lastExecution = context.Connection.GetNextExecution(recurringJobId);
        //var plannedDate = JobHelper.FromTimestamp((long)context.Parameters["Time"]);

        //var nextDate = plannedDate.AddMinutes(3);
        //var score = JobHelper.ToTimestamp(nextDate);

        //using var t = context.Connection.CreateWriteTransaction();
        //t.AddToSet("recurring-jobs", recurringJobId, score);
        //t.Commit();
        //return null!; //skip current execution

        Console.WriteLine($"Create: {context.Job.Type.FullName}.{context.Job.Method.Name} in {context.InitialState?.Name} state.");
        return m_Inner.Create(context);
    }
}

public static class HangfireInternals
{
    public static DateTime? GetNextExecution(
        this IStorageConnection connection,
        string recurringJobId)
    {
        if (connection == null) throw new ArgumentNullException(nameof(connection));
        if (recurringJobId == null) throw new ArgumentNullException(nameof(recurringJobId));

        var recurringJob = connection.GetAllEntriesFromHash($"recurring-job:{recurringJobId}");
        if (recurringJob == null || recurringJob.Count == 0) return null;

        if (recurringJob.TryGetValue("NextExecution", out var nextExecution) && !String.IsNullOrWhiteSpace(nextExecution))
        {
            return JobHelper.DeserializeDateTime(nextExecution);
        }

        return null;
    }
}

internal class CustomBackgroundJobPerformer : IBackgroundJobPerformer
{
    private readonly IBackgroundJobPerformer m_Inner;

    public CustomBackgroundJobPerformer(IBackgroundJobPerformer inner)
    {
        m_Inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    public object Perform(PerformContext context)
    {
        Console.WriteLine($"Perform {context.BackgroundJob.Id} ({context.BackgroundJob.Job.Type.FullName}.{context.BackgroundJob.Job.Method.Name})");
        return m_Inner.Perform(context);
    }
}

internal class CustomBackgroundJobStateChanger : IBackgroundJobStateChanger
{
    private readonly IBackgroundJobStateChanger m_Inner;

    public CustomBackgroundJobStateChanger(IBackgroundJobStateChanger inner)
    {
        m_Inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    public IState ChangeState(StateChangeContext context)
    {
        Console.WriteLine($"ChangeState {context.BackgroundJobId} to {context.NewState}");
        return m_Inner.ChangeState(context);
    }
}