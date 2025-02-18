using CarbonAwareComputing;

namespace Hangfire.Community.CarbonAwareExecution;

public class ShiftJobCarbonAwareFilter(Action<string> logError) : ShiftJobFilter<CarbonAwareExecution>(GetUpdatedScheduleDate, logError)
{
    static ShiftedScheduleDate? GetUpdatedScheduleDate(CarbonAwareExecution execution, Action<string> logError)
    {
        var now = DateTimeOffset.Now;

        return CarbonAwareExecutionForecast
            .GetBestScheduleTime(now, now.Add(execution.MaxExecutionDelay), execution.EstimatedJobDuration, logError)
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