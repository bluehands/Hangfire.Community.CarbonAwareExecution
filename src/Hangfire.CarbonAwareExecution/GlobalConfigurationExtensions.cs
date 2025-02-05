using System.Reflection.Metadata.Ecma335;
using CarbonAwareComputing;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Community.CarbonAwareExecution;
using Hangfire.States;
using Hangfire.Storage;

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
            GlobalJobFilters.Filters.Add(new JobFilter());
            return configuration;
        }
        public static IGlobalConfiguration UseCarbonAwareDataProvider(this IGlobalConfiguration configuration, Func<CarbonAwareExecutionOptions> configure)
        {
            var o = configure.Invoke();
            var options = new CarbonAwareOptions(o.DataProvider, o.Location);

            GlobalJobFilters.Filters.Add(options);
            GlobalJobFilters.Filters.Add(new JobFilter());
            return configuration;
        }
    }

    public record CarbonAwareExecutionOptions(CarbonAwareDataProvider DataProvider, ComputingLocation Location);

    public class JobFilter : IElectStateFilter
    {
        const string CarbonDelayedParameterName = "CarbonDelayed";

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
                                job: j => j.HangfireJob.GetCarbonAwareDelayParameter()?.JobUniqueId != delayParameter.JobUniqueId //might be recreated with same recurring job id, so check out unique id
                            );

                        if (parentJobDeleted)
                        {
                            context.CandidateState = new DeletedState();
                            return;
                        }
                    }

                    if (!context.GetJobParameter<bool>(CarbonDelayedParameterName))
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
                            context.SetJobParameter(CarbonDelayedParameterName, true);
                            context.CandidateState = new ScheduledState(scheduleTo.Value.UtcDateTime);
                        }
                    }
                }
            }
        }
    }

    public class CarbonAwareJob(IBackgroundJobClient jobClient)
    {
        public async Task ScheduleCarbonAware(string actualJobId, string jobInfo, TimeSpan maxExecutionDelay,
            TimeSpan estimatedJobDuration)
        {
            var now = DateTimeOffset.Now;

            var backgroundJobId =
                (await CarbonAwareExecutionForecast.GetBestScheduleTime(now, DateTimeOffset.Now.Add(maxExecutionDelay), estimatedJobDuration))
                .Match(
                    noForecast: _ => RecurringJob.TriggerJob(actualJobId),
                    bestExecutionTime: b =>
                    {
                        var rescheduleInfo = $"{jobInfo} - Rescheduled from {now} to {b.ExecutionTime} to improve carbon footprint.";
                        return jobClient.Schedule<CarbonAwareJob>(c =>
                            c.TriggerCarbonAware(actualJobId, rescheduleInfo), b.ExecutionTime);
                    });
        }

        public void TriggerCarbonAware(string actualJobId, string jobInfo) => RecurringJob.TriggerJob(actualJobId);
    }
}

