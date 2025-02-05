using CarbonAwareComputing;
using Hangfire.Community.CarbonAwareExecution;
using Hangfire.States;

// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace Hangfire
{
    public static class GlobalConfigurationExtensions
    {
        public static IGlobalConfiguration UseCarbonAwareDataProvider(this IGlobalConfiguration configuration, CarbonAwareDataProvider dataProvider, ComputingLocation location)
        {
            var options = new CarbonAwareOptions(dataProvider, location);

            GlobalJobFilters.Filters.Add(options);
            GlobalJobFilters.Filters.Add(new DelayJobsFilter());
            return configuration;
        }
        public static IGlobalConfiguration UseCarbonAwareDataProvider(this IGlobalConfiguration configuration, Func<CarbonAwareExecutionOptions> configure)
        {
            var o = configure.Invoke();
            var options = new CarbonAwareOptions(o.DataProvider, o.Location);

            GlobalJobFilters.Filters.Add(options);
            GlobalJobFilters.Filters.Add(new DelayJobsFilter());
            return configuration;
        }
    }

    public sealed record CarbonAwareExecutionOptions(CarbonAwareDataProvider DataProvider, ComputingLocation Location);

    public class DelayJobsFilter : IElectStateFilter
    {
        const string CarbonAwareDelayParameterName = "CarbonAwareDelay";

        public void OnStateElection(ElectStateContext context)
        {
            if (context.CandidateState?.Name == EnqueuedState.StateName)
            {
                var delayParameter = context.BackgroundJob.Job.GetCarbonAwareDelayParameter();
                var recurringJobId = context.BackgroundJob.GetRecurringJobId();
                if (delayParameter != null)
                {
                    if (recurringJobId != null)
                    {
                        var parentJobDeleted = context.Connection.TryGetRecurringJob(recurringJobId)
                            .Match(
                                error: _ => false,
                                notFound: _ => true,
                                job: j => j.HangfireJob.GetCarbonAwareDelayParameter()?.JobUniqueId != delayParameter.JobUniqueId //might be recreated with same recurring job id, so check our unique id
                            );

                        if (parentJobDeleted)
                        {
                            context.CandidateState = new DeletedState();
                            return;
                        }
                    }

                    if (string.IsNullOrEmpty(context.GetJobParameter<string>(CarbonAwareDelayParameterName)))
                    {
                        var now = DateTimeOffset.Now;
                        var scheduleTo = CarbonAwareExecutionForecast
                            .GetBestScheduleTime(now, now.Add(delayParameter.MaxExecutionDelay), delayParameter.EstimatedJobDuration)
                            .GetAwaiter().GetResult()
                            .Match(
                                noForecast: _ => (DateTimeOffset?)null,
                                bestExecutionTime: b =>
                                {
                                    if (b.ExecutionTime - now < TimeSpan.FromMinutes(1))
                                        return null;

                                    return b.ExecutionTime;
                                });

                        if (scheduleTo != null)
                        {
                            context.SetJobParameter(CarbonAwareDelayParameterName, (scheduleTo - now).ToString());
                            context.CandidateState = new ScheduledState(scheduleTo.Value.UtcDateTime);
                        }
                    }
                }
            }
        }
    }
}