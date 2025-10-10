using Hangfire;
using Hangfire.Common;
using Hangfire.Community.CarbonAwareExecution;
using Hangfire.Server;
using Microsoft.AspNetCore.Mvc;
using static Hangfire.Community.CarbonAwareExecution.CarbonAwareExecution;

namespace Usage.Controllers
{
    /// <summary>
    /// Play around with carbon aware hangfire job here.
    /// </summary>
    /// <param name="backgroundJobs"></param>
    /// <param name="recurringJobs"></param>
    [ApiController]
    [Route("[controller]")]
    public class JobsController(IBackgroundJobClient backgroundJobs, IRecurringJobManager recurringJobs)
        : ControllerBase
    {
	    /// <summary>
	    /// Schedules all kind of carbon aware tests jobs. Navigate to /hangfire to see jobs in hangfire ui.
	    /// </summary>
	    /// <remarks>Awesomeness!</remarks>
	    /// <response code="200">Jobs scheduled</response>
		[HttpGet]
        public IActionResult Get()
        {
	        var maxExecutionDelay = TimeSpan.FromHours(2);
	        var estimatedJobDuration = TimeSpan.FromMinutes(10);
	        
	        backgroundJobs.Enqueue(
                () => HangfireActions.CarbonAwareJob(
                    "Hello world from Hangfire! Enqueue carbon aware jobs.",
                    new CarbonAwareExecution(maxExecutionDelay, estimatedJobDuration),
                    null)
            );

            backgroundJobs.Enqueue(
                () => HangfireActions.MyJob(
                    "Hello world from Hangfire! Enqueue carbon aware jobs",
                    ShiftCarbonAware(maxExecutionDelay, estimatedJobDuration),
                    null
                ));

            BackgroundJob.Schedule(
                () => HangfireActions.CarbonAwareJob(
                    "Hello world from Hangfire! Schedule carbon aware jobs",
                    new CarbonAwareExecution(maxExecutionDelay, estimatedJobDuration), null),
                TimeSpan.FromMinutes(20));

            var earliestExecutionTime = DateTimeOffset.Now + TimeSpan.FromHours(1);
            backgroundJobs.Schedule(
                () => HangfireActions.MyJob(
                    "Hello world from Hangfire! Schedule carbon aware jobs",
                    new CarbonAwareExecution(maxExecutionDelay, estimatedJobDuration), null),
                earliestExecutionTime);

            //every night between 2 and 5 am
            RecurringJob.AddOrUpdate(
                "CarbonAwareJob",
                () => HangfireActions.MyJob("Hello world from Hangfire! Recurring carbon aware jobs",
                    new CarbonAwareExecution(TimeSpan.FromHours(3), estimatedJobDuration), null),
                "2 0 * * *"
            );
            return Ok($"Test jobs send to hangfire. See hangfire dashboard: {Request.Scheme}://{Request.Host}/hangfire");
        }

        /// <summary>
        /// Carbon aware recurring job.
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="cronExpression"></param>
        /// <param name="maxExecutionDelayHours"></param>
        /// <param name="estimatedJobDurationMinutes"></param>
        /// <param name="queue"></param>
        /// <returns></returns>
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

		/// <summary>
		/// Recurring job.
		/// </summary>
		/// <param name="jobId"></param>
		/// <param name="cronExpression"></param>
		/// <returns></returns>
		[Route("[action]")]
        [HttpPost]
        public IActionResult AddOrUpdatedRecurringJob(string jobId, string cronExpression)
        {
            recurringJobs.AddOrUpdate(jobId, () => HangfireActions.MyJob("Recurring job with carbon aware delay", null, null), cronExpression);
            return Ok();
        }

		/// <summary>
		/// Carbon aware delayed job.
		/// </summary>
		/// <param name="minDelayTimeSpan"></param>
		/// <param name="maxExecutionDelayHours"></param>
		/// <param name="estimatedJobDurationMinutes"></param>
		/// <returns></returns>
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

        /// <summary>
        /// Delayed job.
        /// </summary>
        /// <param name="delayTimeSpan"></param>
        /// <returns></returns>
        [Route("[action]")]
        [HttpPost]
        public IActionResult ScheduleJob(string? delayTimeSpan)
        {
            var delay = delayTimeSpan != null ? TimeSpan.Parse(delayTimeSpan) : TimeSpan.Zero;
            var scheduledJobId = backgroundJobs.Schedule(() => HangfireActions.MyJob("Scheduled job without carbon aware delay", null, null), delay);
            return Ok(scheduledJobId);
        }

        /// <summary>
        /// Carbon aware enqueued job.
        /// </summary>
        /// <param name="maxExecutionDelayHours"></param>
        /// <param name="estimatedJobDurationMinutes"></param>
        /// <returns></returns>
        [Route("[action]")]
        [HttpPost]
        public IActionResult EnqueueJobCarbonAware(int maxExecutionDelayHours, int estimatedJobDurationMinutes)
        {
            var maxExecutionDelay = TimeSpan.FromHours(maxExecutionDelayHours);
            var estimatedJobDuration = TimeSpan.FromMinutes(estimatedJobDurationMinutes);
            var jobId = backgroundJobs.Enqueue(() => HangfireActions.MyJob("Enqueued job with carbon aware delay", new(maxExecutionDelay, estimatedJobDuration), null));
            return Ok(jobId);
        }

        /// <summary>
        /// Enqueued job.
        /// </summary>
        /// <returns></returns>
        [Route("[action]")]
        [HttpPost]
        public IActionResult EnqueueJob()
        {
            var jobId = backgroundJobs.Enqueue(() => HangfireActions.MyJob("Enqueued job without carbon aware delay", null, null));
            return Ok(jobId);
        }
    }

    /// <summary>
    /// Hangfire job helper.
    /// </summary>
    public static class HangfireActions
    {
        // ReSharper disable UnusedParameter.Global
        /// <summary>
        /// Actual test job executed by hangfire.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="carbonDelay"></param>
        /// <param name="performContext"></param>
        public static void CarbonAwareJob(
            string info, 
            CarbonAwareExecution? carbonDelay, 
            /*optional, provided by hangfire, you can access information about shift here */ PerformContext? performContext)
        {
            var shiftInfo = GetShiftInfo(performContext, carbonDelay);
            Console.WriteLine(info + shiftInfo);
        }

        static string? GetShiftInfo(PerformContext? performContext, CarbonAwareExecution? carbonDelay)
        {
            var shiftParameter = carbonDelay != null ? performContext?.GetShiftParameter<ShiftInfoJobParameter>() : null;
            var shiftInfo = shiftParameter != null
                ? $" Shifted from {shiftParameter.OriginalExecutionTime.Date} with carbon intensity {Math.Round(shiftParameter.OriginalExecutionTime.CarbonIntensity, 1)} to {shiftParameter.ShiftedExecutionTime.Date} with carbon intensity {Math.Round(shiftParameter.ShiftedExecutionTime.CarbonIntensity, 1)}"
                : null;
            return shiftInfo;
        }

		/// <summary>
		/// Actual test job executed by hangfire.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="carbonDelay"></param>
		/// <param name="performContext"></param>
		public static async Task MyJob(
            string info,
            CarbonAwareExecution? carbonDelay, 
            /*optional, provided by hangfire, you can access information about shift here */ PerformContext? performContext)
        {
            await Task.Delay(100);
            var shiftInfo = GetShiftInfo(performContext, carbonDelay);
            Console.WriteLine(info + shiftInfo);
        }
        // ReSharper restore UnusedParameter.Global
    }
}