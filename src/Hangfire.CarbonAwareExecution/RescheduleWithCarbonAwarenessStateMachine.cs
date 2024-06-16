using CarbonAwareComputing;
using Hangfire.Client;
using Hangfire.Server;
using Hangfire.States;

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
        if (!RecurringJobExecutionWindowSerializer.TryDeserializeFromJob(context.BackgroundJob, out var forecastParameter))
        {
            return m_Inner.ApplyState(context);
        }

        var scheduleOptions = CarbonAwareExecutionForecast.GetBestScheduleTime(DateTimeOffset.Now, DateTimeOffset.Now + forecastParameter.MaxExecutionDelay, forecastParameter.EstimatedJobDuration).GetAwaiter().GetResult();
        return scheduleOptions.Match(
            _ => m_Inner.ApplyState(context),
            bestExecutionTime =>
            {
                var state = new ScheduledState(bestExecutionTime.ExecutionTime.ToUniversalTime().DateTime)
                {
                    Reason = $"Schedule recurring job '{forecastParameter.JobId}' for minimal carbon impact"
                };
                m_Client ??= m_GetBackgroundJobClient.Invoke();
                m_Client.Create(context.BackgroundJob.Job, state);
                return state;
            }
        );
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
        Console.WriteLine($"Create: {context.Job.Type.FullName}.{context.Job.Method.Name} in {context.InitialState?.Name} state");
        return m_Inner.Create(context);
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