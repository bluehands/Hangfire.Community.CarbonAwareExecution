using System.Linq.Expressions;
using CarbonAwareComputing;
using Hangfire;
using Hangfire.Common;
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
            m_RecurringJobs.AddOrUpdateCarbonAware("daily", () => Console.WriteLine("Hello, world!"), Cron.Daily, TimeSpan.FromHours(2),TimeSpan.FromMinutes(20));
            
            CarbonAwareRecurringJob.AddOrUpdate(
                "daily", 
                () => Console.WriteLine("Hello, world!"), 
                Cron.Daily, TimeSpan.FromHours(2), 
                TimeSpan.FromMinutes(20));


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

            m_RecurringJobs.AddOrUpdate(jobId, () => HangfireActions.DoRecurringJob(new CarbonAwareDelayParameter(maxExecutionDelay, estimatedJobDuration)), cronExpression);
            
            //Expression<Action> methodCall = () => HangfireActions.DoRecurringJobCarbonAware();
            //var job = Job.FromExpression(methodCall);
            //var actualJobId = $"{jobId}_1";
            //m_RecurringJobs.AddOrUpdate(actualJobId, methodCall, Cron.Never);

            //var typeName = job.Type?.Name;
            //var jobInfo = $"{(typeName == null ? "" : $"{typeName}.")}{job.Method.Name}({job.Args.Count} args)";


            //try
            //{
                
            //    m_RecurringJobs.AddOrUpdate<CarbonAwareJob>(actualJobId, c => c.ScheduleCarbonAware(actualJobId, jobInfo, maxExecutionDelay, estimatedJobDuration), cronExpression);
            //}
            //catch (Exception)
            //{
            //    RecurringJob.RemoveIfExists(actualJobId);
            //    throw;
            //}

            //m_RecurringJobs.CarbonAware(job, jobId,
            //    (actualJobId, jobInfo) =>
            //    {
            //        m_RecurringJobs.AddOrUpdate<CarbonAwareJob>(jobId,
            //            c => c.ScheduleCarbonAware(actualJobId, jobInfo, maxExecutionDelay, estimatedJobDuration),
            //            cronExpression);
            //        return actualJobId;
            //    }, new());


            //m_RecurringJobs.AddOrUpdateCarbonAware(jobId, methodCall, cronExpression,
            //    maxExecutionDelay, estimatedJobDuration);

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
        public async Task<IActionResult> ScheduleJobClassic(string? delayTimeSpan)
        {
            var delay = delayTimeSpan != null ? TimeSpan.Parse(delayTimeSpan) : TimeSpan.Zero;
            var scheduledJobId = m_BackgroundJobs.Schedule(() => HangfireActions.DoScheduledJob(null), delay);


            //var latestDelay = TimeSpan.FromHours(maxExecutionDelayHours);
            //var estimatedJobDuration = TimeSpan.FromMinutes(estimatedJobDurationMinutes);

            //var myJobId = Guid.NewGuid().ToString();
            //m_RecurringJobs.AddOrUpdate(myJobId, () => HangfireActions.DoScheduledJob(), Cron.Never);

            //var jobId = m_BackgroundJobs.Schedule<CarbonAwareJob>(
            //    j => j.ScheduleCarbonAware(myJobId, latestDelay, estimatedJobDuration), delay);

            //var job = Job.FromExpression(() => HangfireActions.DoScheduledJob());
            //var scheduledJobId = m_RecurringJobs.CarbonAware(job, 
            //    null, 
            //    (actualJobId, info) => m_BackgroundJobs.Schedule<CarbonAwareJob>(j =>
            //        j.ScheduleCarbonAware(actualJobId, info, latestDelay, estimatedJobDuration), delay), new());

            //var jobId = await m_BackgroundJobs.ScheduleWithCarbonAwarenessAsync(() => HangfireActions.DoScheduledJob(), DateTimeOffset.Now.Add(delay), TimeSpan.Zero,
            //    latestDelay, estimatedJobDuration);

            return Ok(scheduledJobId);
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> ScheduleJob(string? minDelayTimeSpan, int maxExecutionDelayHours, int estimatedJobDurationMinutes)
        {
            var delay = minDelayTimeSpan != null ? TimeSpan.Parse(minDelayTimeSpan) : TimeSpan.Zero;

            var latestDelay = TimeSpan.FromHours(maxExecutionDelayHours);
            var estimatedJobDuration = TimeSpan.FromMinutes(estimatedJobDurationMinutes);
            var scheduledJobId = m_BackgroundJobs
                .Schedule(() => HangfireActions.DoScheduledJob(new CarbonAwareDelayParameter(latestDelay, estimatedJobDuration)), delay);

            //var myJobId = Guid.NewGuid().ToString();
            //m_RecurringJobs.AddOrUpdate(myJobId, () => HangfireActions.DoScheduledJob(), Cron.Never);

            //var jobId = m_BackgroundJobs.Schedule<CarbonAwareJob>(
            //    j => j.ScheduleCarbonAware(myJobId, latestDelay, estimatedJobDuration), delay);

            //var job = Job.FromExpression(() => HangfireActions.DoScheduledJob(new CarbonAwareDelayParameter(latestDelay, estimatedJobDuration)));
            //var scheduledJobId = m_RecurringJobs.CarbonAware(job, 
            //    null, 
            //    (actualJobId, info) => m_BackgroundJobs.Schedule<CarbonAwareJob>(j =>
            //        j.ScheduleCarbonAware(actualJobId, info, latestDelay, estimatedJobDuration), delay), new());

            //var jobId = await m_BackgroundJobs.ScheduleWithCarbonAwarenessAsync(() => HangfireActions.DoScheduledJob(), DateTimeOffset.Now.Add(delay), TimeSpan.Zero,
            //    latestDelay, estimatedJobDuration);

            return Ok(scheduledJobId);
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> EnqueueJob(int maxExecutionDelayHours, int estimatedJobDurationMinutes)
        {
            var maxExecutionDelay = TimeSpan.FromHours(maxExecutionDelayHours);
            var estimatedJobDuration = TimeSpan.FromMinutes(estimatedJobDurationMinutes);
            var jobId = m_BackgroundJobs.Enqueue(() => HangfireActions.DoEnqueuedJob(new (maxExecutionDelay, estimatedJobDuration)));

            //var jobId = await m_BackgroundJobs.EnqueueWithCarbonAwarenessAsync(() => HangfireActions.DoEnqueuedJob(null), 
            //    DateTimeOffset.Now.Add(maxExecutionDelay), estimatedJobDuration);

            return Ok(jobId);
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> EnqueueJobClassic()
        {
            var jobId = m_BackgroundJobs.Enqueue(() => HangfireActions.DoEnqueuedJob(null));

            //var jobId = await m_BackgroundJobs.EnqueueWithCarbonAwarenessAsync(() => HangfireActions.DoEnqueuedJob(), 
            //    DateTimeOffset.Now.Add(TimeSpan.FromHours(maxExecutionDelayHours)), TimeSpan.FromMinutes(estimatedJobDurationMinutes));

            return Ok(jobId);
        }
    }

    public static class HangfireActions
    {
        // ReSharper disable once UnusedParameter.Global
        public static void DoRecurringJob(CarbonAwareDelayParameter? carbonDelay) => Console.WriteLine("Recurring!");
        public static void DoRecurringJobCarbonAware() => Console.WriteLine("Recurring carbon aware!");
        public static void DoScheduledJob(CarbonAwareDelayParameter? carbonDelay) => Console.WriteLine("Scheduled!");
        public static void DoEnqueuedJob(CarbonAwareDelayParameter? carbonDelay) => Console.WriteLine("Enqueued!");
    }

    public static class CarbonExtensions
    {
        public delegate string ScheduleCarbonAware(string jobToTrigger, string jobInfo);

        public static string CarbonAware(this IRecurringJobManager recurringJobManager, 
            Job job, 
            string? jobId, 
            ScheduleCarbonAware scheduleCarbonAware,
            RecurringJobOptions recurringJobOptions)
        {
            var actualJobId = jobId != null ? $"{jobId}_1" : Guid.NewGuid().ToString();
            recurringJobManager.AddOrUpdate(actualJobId, job, Cron.Never(), recurringJobOptions);

            try
            {
                var typeName = job.Type?.Name;
                var jobInfo = $"{(typeName == null ? "" : $"{typeName}.")}{job.Method.Name}({job.Args.Count} args)";

                return scheduleCarbonAware(actualJobId, jobInfo);
            }
            catch (Exception)
            {
                RecurringJob.RemoveIfExists(actualJobId);
                throw;
            }
        }
    }
    
}

