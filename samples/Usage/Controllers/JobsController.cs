using Hangfire;
using Hangfire.Common;
using Hangfire.Community.CarbonAwareExecution;
using Hangfire.Server;
using Microsoft.AspNetCore.Mvc;
using static Hangfire.Community.CarbonAwareExecution.CarbonAwareExecution;

namespace Usage.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobsController(IBackgroundJobClient backgroundJobs, IRecurringJobManager recurringJobs)
        : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            backgroundJobs.Enqueue(
                () => HangfireActions.CarbonAwareJob(
                    "Hello world from Hangfire! Enqueue carbon aware jobs.",
                    new CarbonAwareExecution(TimeSpan.FromHours(2), TimeSpan.FromMinutes(5)),
                    null)
            );

            backgroundJobs.Enqueue(
                () => HangfireActions.MyJob(
                    "Hello world from Hangfire! Enqueue carbon aware jobs",
                    ShiftCarbonAware(TimeSpan.FromHours(2), TimeSpan.FromMinutes(5)),
                    null
                ));

            BackgroundJob.Schedule(
                () => HangfireActions.CarbonAwareJob(
                    "Hello world from Hangfire! Schedule carbon aware jobs",
                    new CarbonAwareExecution(TimeSpan.FromHours(2), TimeSpan.FromMinutes(5)), null),
                TimeSpan.FromMinutes(20));

            var earliestExecutionTime = DateTimeOffset.Now + TimeSpan.FromHours(1);
            backgroundJobs.Schedule(
                () => HangfireActions.MyJob(
                    "Hello world from Hangfire! Schedule carbon aware jobs",
                    new CarbonAwareExecution(TimeSpan.FromHours(2), TimeSpan.FromMinutes(5)), null),
                earliestExecutionTime);

            //every night between 2 and 5 am
            RecurringJob.AddOrUpdate(
                "CarbonAwareJob",
                () => HangfireActions.MyJob("Hello world from Hangfire! Recurring carbon aware jobs",
                    new CarbonAwareExecution(TimeSpan.FromHours(3), TimeSpan.FromMinutes(5)), null),
                "2 0 * * *"
            );
            return Ok();
        }

        [Route("[action]")]
        [HttpPost]
        public IActionResult AddOrUpdatedRecurringJobCarbonAware(string jobId, string cronExpression,
            int maxExecutionDelayHours, int estimatedJobDurationMinutes, string queue = "default")
        {
            var maxExecutionDelay = TimeSpan.FromHours(maxExecutionDelayHours);
            var estimatedJobDuration = TimeSpan.FromMinutes(estimatedJobDurationMinutes);

            var job = Job.FromExpression(() => HangfireActions.MyJob(
                "Recurring job with potential carbon delay",
                new CarbonAwareExecution(maxExecutionDelay, estimatedJobDuration),
                null
            ), queue);

            recurringJobs.AddOrUpdate(jobId, job, cronExpression);

            return Ok();
        }

        [Route("[action]")]
        [HttpPost]
        public IActionResult AddOrUpdatedRecurringJob(string jobId, string cronExpression)
        {
            recurringJobs.AddOrUpdate(jobId, () => HangfireActions.MyJob("Recurring job with carbon aware delay", null, null), cronExpression);
            return Ok();
        }

        [Route("[action]")]
        [HttpPost]
        public IActionResult ScheduleJobCarbonAware(string? minDelayTimeSpan, int maxExecutionDelayHours, int estimatedJobDurationMinutes)
        {
            var delay = minDelayTimeSpan != null ? TimeSpan.Parse(minDelayTimeSpan) : TimeSpan.Zero;

            var latestDelay = TimeSpan.FromHours(maxExecutionDelayHours);
            var estimatedJobDuration = TimeSpan.FromMinutes(estimatedJobDurationMinutes);
            var scheduledJobId = backgroundJobs
                .Schedule(() => HangfireActions.MyJob("Scheduled job with carbon aware delay", new CarbonAwareExecution(latestDelay, estimatedJobDuration), null), delay);

            return Ok(scheduledJobId);
        }

        [Route("[action]")]
        [HttpPost]
        public IActionResult ScheduleJob(string? delayTimeSpan)
        {
            var delay = delayTimeSpan != null ? TimeSpan.Parse(delayTimeSpan) : TimeSpan.Zero;
            var scheduledJobId = backgroundJobs.Schedule(() => HangfireActions.MyJob("Scheduled job without carbon aware delay", null, null), delay);
            return Ok(scheduledJobId);
        }

        [Route("[action]")]
        [HttpPost]
        public IActionResult EnqueueJobCarbonAware(int maxExecutionDelayHours, int estimatedJobDurationMinutes)
        {
            var maxExecutionDelay = TimeSpan.FromHours(maxExecutionDelayHours);
            var estimatedJobDuration = TimeSpan.FromMinutes(estimatedJobDurationMinutes);
            var jobId = backgroundJobs.Enqueue(() => HangfireActions.MyJob("Enqueued job with carbon aware delay", new(maxExecutionDelay, estimatedJobDuration), null));
            return Ok(jobId);
        }

        [Route("[action]")]
        [HttpPost]
        public IActionResult EnqueueJob()
        {
            var jobId = backgroundJobs.Enqueue(() => HangfireActions.MyJob("Enqueued job without carbon aware delay", null, null));
            return Ok(jobId);
        }
    }

    public static class HangfireActions
    {
        // ReSharper disable UnusedParameter.Global
        public static void CarbonAwareJob(string info, CarbonAwareExecution? carbonDelay, /*optional, provided by hangfire, you can access information about shift here */ PerformContext? performContext)
        {
            var shiftInfo = GetShiftInfo(performContext);
            Console.WriteLine(info + shiftInfo);
        }

        static string? GetShiftInfo(PerformContext? performContext)
        {
            var shiftParameter = performContext?.GetShiftParameter<ShiftInfoJobParameter>();
            var shiftInfo = shiftParameter != null
                ? $" Shifted from {shiftParameter.OriginalExecutionTime.Date} with carbon intensity {shiftParameter.OriginalExecutionTime.CarbonIntensity} to {shiftParameter.ShiftedExecutionTime.Date} with carbon intensity {shiftParameter.ShiftedExecutionTime.CarbonIntensity}"
                : null;
            return shiftInfo;
        }

        public static async Task MyJob(string info, CarbonAwareExecution? carbonDelay, /*optional, provided by hangfire, you can access information about shift here */ PerformContext? performContext)
        {
            await Task.Delay(100);
            var shiftInfo = GetShiftInfo(performContext);
            Console.WriteLine(info + shiftInfo);
        }
        // ReSharper restore UnusedParameter.Global
    }
}