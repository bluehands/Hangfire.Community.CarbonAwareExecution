using Hangfire;
using Hangfire.Community.CarbonAwareExecution;
using Microsoft.AspNetCore.Mvc;

namespace Usage.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly ILogger<JobsController> m_Logger;
        private readonly IBackgroundJobClient m_BackgroundJobs;
        private readonly IRecurringJobManager m_RecurringJobs;

        public JobsController(ILogger<JobsController> logger, IBackgroundJobClient backgroundJobs, IRecurringJobManager recurringJobs)
        {
            m_Logger = logger;
            m_BackgroundJobs = backgroundJobs;
            m_RecurringJobs = recurringJobs;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await m_BackgroundJobs.EnqueueWithCarbonAwarenessAsync(
                () => Console.WriteLine("Hello world from Hangfire!. Enqueue carbon aware jobs"),
                DateTimeOffset.Now + TimeSpan.FromHours(2),
                TimeSpan.FromMinutes(5));
            
            await CarbonAwareBackgroundJob.EnqueueAsync(
                () => Console.WriteLine("Hello world from Hangfire!. Enqueue carbon aware jobs"),
                DateTimeOffset.Now + TimeSpan.FromHours(2),
                TimeSpan.FromMinutes(5));

            await m_BackgroundJobs.ScheduleWithCarbonAwarenessAsync(
                () => Console.WriteLine("Hello world from Hangfire!. Schedule carbon aware jobs"),
                DateTimeOffset.Now + TimeSpan.FromHours(2),
                TimeSpan.FromMinutes(20),
                TimeSpan.FromMinutes(30), TimeSpan.FromMinutes(5));

            await CarbonAwareBackgroundJob.ScheduleAsync(
                () => Console.WriteLine("Hello world from Hangfire!. Schedule carbon aware jobs"),
                DateTimeOffset.Now + TimeSpan.FromHours(2),
                TimeSpan.FromMinutes(20),
                TimeSpan.FromMinutes(30), TimeSpan.FromMinutes(5));
            return Ok();
        }

        [Route("[action]")]
        [HttpPost]
        public IActionResult AddOrUpdatedRecurringJobCarbonAware(string jobId, string cronExpression,
            int maxExecutionDelayHours, int estimatedJobDurationMinutes)
        {
            var maxExecutionDelay = TimeSpan.FromHours(maxExecutionDelayHours);
            var estimatedJobDuration = TimeSpan.FromMinutes(estimatedJobDurationMinutes);

            m_RecurringJobs
                .AddOrUpdate(jobId,
                    () => HangfireActions.DoRecurringJob(new CarbonAwareDelayParameter(maxExecutionDelay,
                        estimatedJobDuration)), cronExpression
                );

            return Ok();
        }

        [Route("[action]")]
        [HttpPost]
        public IActionResult AddOrUpdatedRecurringJob(string jobId, string cronExpression)
        {
            m_RecurringJobs.AddOrUpdate(jobId, () => HangfireActions.DoRecurringJob(null), cronExpression);
            return Ok();
        }

        [Route("[action]")]
        [HttpPost]
        public IActionResult ScheduleJob(string? delayTimeSpan)
        {
            var delay = delayTimeSpan != null ? TimeSpan.Parse(delayTimeSpan) : TimeSpan.Zero;
            var scheduledJobId = m_BackgroundJobs.Schedule(() => HangfireActions.DoScheduledJob(null), delay);
            return Ok(scheduledJobId);
        }

        [Route("[action]")]
        [HttpPost]
        public IActionResult ScheduleJobCarbonAware(string? minDelayTimeSpan, int maxExecutionDelayHours, int estimatedJobDurationMinutes)
        {
            var delay = minDelayTimeSpan != null ? TimeSpan.Parse(minDelayTimeSpan) : TimeSpan.Zero;

            var latestDelay = TimeSpan.FromHours(maxExecutionDelayHours);
            var estimatedJobDuration = TimeSpan.FromMinutes(estimatedJobDurationMinutes);
            var scheduledJobId = m_BackgroundJobs
                .Schedule(() => HangfireActions.DoScheduledJob(new CarbonAwareDelayParameter(latestDelay, estimatedJobDuration)), delay);

            return Ok(scheduledJobId);
        }

        [Route("[action]")]
        [HttpPost]
        public IActionResult EnqueueJobCarbonAware(int maxExecutionDelayHours, int estimatedJobDurationMinutes)
        {
            var maxExecutionDelay = TimeSpan.FromHours(maxExecutionDelayHours);
            var estimatedJobDuration = TimeSpan.FromMinutes(estimatedJobDurationMinutes);
            var jobId = m_BackgroundJobs.Enqueue(() => HangfireActions.DoEnqueuedJob(new (maxExecutionDelay, estimatedJobDuration)));
            return Ok(jobId);
        }

        [Route("[action]")]
        [HttpPost]
        public IActionResult EnqueueJob()
        {
            var jobId = m_BackgroundJobs.Enqueue(() => HangfireActions.DoEnqueuedJob(null));
            return Ok(jobId);
        }
    }

    public static class HangfireActions
    {
        // ReSharper disable UnusedParameter.Global
        public static void DoRecurringJob(CarbonAwareDelayParameter? carbonDelay) => Console.WriteLine("Recurring!");
        public static void DoRecurringJobCarbonAware() => Console.WriteLine("Recurring carbon aware!");
        public static void DoScheduledJob(CarbonAwareDelayParameter? carbonDelay) => Console.WriteLine("Scheduled!");
        public static void DoEnqueuedJob(CarbonAwareDelayParameter? carbonDelay) => Console.WriteLine("Enqueued!");
        // ReSharper restore UnusedParameter.Global
    }
}