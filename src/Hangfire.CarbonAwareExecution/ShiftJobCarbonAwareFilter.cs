using CarbonAwareComputing;
using Hangfire.Community.CarbonAwareExecution.Internal;

namespace Hangfire.Community.CarbonAwareExecution;

public class ShiftJobCarbonAwareFilter : ShiftJobFilter<CarbonAwareExecution>
{
    internal ShiftJobCarbonAwareFilter(CarbonAwareServices services, ComputingLocation location) 
        : base(delayParameter => GetUpdatedScheduleDate(delayParameter, location, services), services.Logger)
    {
    }

    static ShiftedScheduleDate? GetUpdatedScheduleDate(CarbonAwareExecution execution, ComputingLocation location, CarbonAwareServices services)
    {
        var now = DateTimeOffset.Now;

        return services
            .GetBestScheduleTime(location, now, now.Add(execution.MaxExecutionDelay), execution.EstimatedJobDuration)
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