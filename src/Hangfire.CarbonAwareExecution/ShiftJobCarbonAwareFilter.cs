using CarbonAwareComputing;
using Hangfire.Community.CarbonAwareExecution.Internal;

namespace Hangfire.Community.CarbonAwareExecution;

public class ShiftJobCarbonAwareFilter : ShiftJobFilter<CarbonAwareExecution, ShiftInfoJobParameter>
{
    internal ShiftJobCarbonAwareFilter(CarbonAwareServices services, ComputingLocation location) 
        : base(delayParameter => GetUpdatedScheduleDate(delayParameter, location, services), services.Logger)
    {
    }

    static ShiftedScheduleDate? GetUpdatedScheduleDate(CarbonAwareExecution execution, ComputingLocation location, CarbonAwareServices services)
    {
        var now = DateTimeOffset.Now;

        var shifted = services
            .GetBestScheduleTime(location, now, now.Add(execution.MaxExecutionDelay), execution.EstimatedJobDuration)
            .GetAwaiter().GetResult();

        if (shifted == null)
            return null;
        
        var shiftToDate = shifted.ShiftedExecutionTime.Date;
        var shiftTo = shiftToDate.CropSeconds();
        if (shiftTo - now < TimeSpan.FromMinutes(1))
            return null;

        return new(shiftTo, shifted);
    }
}

public record ShiftInfoJobParameter(ExecutionTimeWithIntensity OriginalExecutionTime, ExecutionTimeWithIntensity ShiftedExecutionTime);

public record ExecutionTimeWithIntensity(DateTimeOffset Date, double CarbonIntensity);