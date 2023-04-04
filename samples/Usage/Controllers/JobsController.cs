using Hangfire;
using Hangfire.CarbonAwareExecution;
using Microsoft.AspNetCore.Mvc;

namespace SampleUsage.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly ILogger<JobsController> m_Logger;
        private readonly IBackgroundJobClient m_BackgroundJobs;

        public JobsController(ILogger<JobsController> logger, IBackgroundJobClient backgroundJobs)
        {
            m_Logger = logger;
            m_BackgroundJobs = backgroundJobs;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await m_BackgroundJobs.EnqueueWithCarbonAwarenessAsync(
                () => Console.WriteLine("Hello world from Hangfire!. Enqueue carbon aware jobs"),
                DateTimeOffset.Now + TimeSpan.FromHours(2),
                TimeSpan.FromMinutes(5));

            await m_BackgroundJobs.ScheduleWithCarbonAwarenessAsync(
                () => Console.WriteLine("Hello world from Hangfire!. Schedule carbon aware jobs"),
                DateTimeOffset.Now + TimeSpan.FromHours(2),
                TimeSpan.FromMinutes(20),
                TimeSpan.FromMinutes(30), TimeSpan.FromMinutes(5));
            return Ok();
        }
    }
}