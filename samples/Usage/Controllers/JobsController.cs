using Hangfire;
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

        public void DoJob()
        {
            Console.WriteLine("Transparent!");
        }
    }
}