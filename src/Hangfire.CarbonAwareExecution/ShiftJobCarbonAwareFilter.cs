using CarbonAwareComputing;

namespace Hangfire.Community.CarbonAwareExecution;

public class ShiftJobCarbonAwareFilter() : ShiftJobFilter<CarbonAwareExecution>(GetUpdatedScheduleDate)
{
    static ShiftedScheduleDate? GetUpdatedScheduleDate(CarbonAwareExecution execution)
    {
        var now = DateTimeOffset.Now;

        return CarbonAwareExecutionForecast
            .GetBestScheduleTime(now, now.Add(execution.MaxExecutionDelay), execution.EstimatedJobDuration)
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