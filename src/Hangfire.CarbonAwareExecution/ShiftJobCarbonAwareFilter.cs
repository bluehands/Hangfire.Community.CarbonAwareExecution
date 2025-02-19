using CarbonAwareComputing;
using Hangfire.Community.CarbonAwareExecution.Internal;
using Microsoft.Extensions.Logging;

namespace Hangfire.Community.CarbonAwareExecution;

public class ShiftJobCarbonAwareFilter(ILogger? logger = null) 
    : ShiftJobFilter<CarbonAwareExecution>(delayParameter => GetUpdatedScheduleDate(delayParameter, logger), logger)
{
    static ShiftedScheduleDate? GetUpdatedScheduleDate(CarbonAwareExecution execution, ILogger? logger)
    {
        var now = DateTimeOffset.Now;

        return CarbonAwareExecutionForecast
            .GetBestScheduleTime(now, now.Add(execution.MaxExecutionDelay), execution.EstimatedJobDuration, logger)
            .GetAwaiter().GetResult()
            .Match(
                noForecast: _ => null,
                bestExecutionTime: b =>
                {
                    var shiftTo = b.ExecutionTime.CropSeconds();
                    return shiftTo - now < TimeSpan.FromMinutes(1) 
                        ? null 
                        : new ShiftedScheduleDate(shiftTo, "Rescheduled for optimized carbon footprint");
                });
    }
}