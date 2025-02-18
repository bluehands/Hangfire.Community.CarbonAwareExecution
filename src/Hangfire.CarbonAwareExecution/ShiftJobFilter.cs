﻿using Hangfire.States;
using Hangfire.Storage;

namespace Hangfire.Community.CarbonAwareExecution;

public sealed record ShiftedScheduleDate(DateTimeOffset Date, string? Reason);

public class ShiftJobFilter<TParameter>(
    Func<TParameter, Action<string>, ShiftedScheduleDate?> getShiftedScheduleDate,
    Action<string> logError) : IElectStateFilter
{
    const string ShiftParameterName = "__Shift";

    // ReSharper disable once NotAccessedPositionalProperty.Local
    sealed record ShiftParameterValue(string Shift, string? Cron, string? TimeZone);

    public void OnStateElection(ElectStateContext context)
    {
        if (context.CandidateState?.Name != EnqueuedState.StateName
            && context.CandidateState?.Name != ProcessingState.StateName) return; //check Processing state because rescheduled recurring jobs go from scheduled to processing directly

        var delayParameter = context.BackgroundJob.Job.GetArgument<TParameter>();
        if (delayParameter != null)
        {
            var shiftParameter = context.GetJobParameter<ShiftParameterValue>(ShiftParameterName);
            var recurringJobId = context.GetJobParameter<string>("RecurringJobId", true);
            if (recurringJobId != null)
            {
                if (shiftParameter != null) //recurring job shifted -> check if parent is still the same, otherwise delete
                {
                    var parentJobDeletedOrChanged = ParentJobDeletedOrChanged(context, recurringJobId, shiftParameter);
                    if (parentJobDeletedOrChanged)
                    {
                        context.CandidateState = new DeletedState();
                        return;
                    }
                }
            }

            if (shiftParameter == null) //not yet shifted -> calculate shift and reschedule
            {
                var now = DateTimeOffset.Now;
                var delayedScheduleTime = getShiftedScheduleDate(delayParameter, logError);

                if (delayedScheduleTime != null)
                {
                    string? recurringJobCron = null;
                    string? recurringJobTimeZone = null;
                    if (recurringJobId != null)
                    {
                        (recurringJobCron, recurringJobTimeZone) = context.Connection
                            .GetRecurringJob(recurringJobId)
                            .Match(
                                error: e => (e.Cron, e.TimeZone),
                                notFound: _ => (null, null),
                                job: j => (j.Cron, j.TimeZone)
                            );
                    }

                    var scheduleTo = delayedScheduleTime.Date;
                    context.SetJobParameter(ShiftParameterName,
                        new ShiftParameterValue(
                            Shift: $@"{(scheduleTo - now):hh\:mm\:ss}{(delayedScheduleTime.Reason != null ? $" - {delayedScheduleTime.Reason}" : "")}",
                            Cron: recurringJobCron, 
                            TimeZone: recurringJobTimeZone)
                    );
                    context.CandidateState = new ScheduledState(scheduleTo.UtcDateTime);
                }
            }
        }
    }

    static bool ParentJobDeletedOrChanged(ElectStateContext context, string recurringJobId,
        ShiftParameterValue shiftParameter)
    {
        var parentJobDeletedOrChanged = context.Connection
            .GetRecurringJob(recurringJobId)
            .Match(
                error: _ => false,
                notFound: _ => true,
                job: j =>
                {
                    var myInvocationData = InvocationData.SerializeJob(context.BackgroundJob.Job);
                    var recurringJobInvocationData = j.InvocationData;
                    //might be recreated with same recurring job id, so compare background job and recurring job definition by checking their serialized representation
                    var jobChanged = myInvocationData.Arguments != recurringJobInvocationData.Arguments
                                     || myInvocationData.Method != recurringJobInvocationData.Method
                                     || myInvocationData.ParameterTypes != recurringJobInvocationData.ParameterTypes
                                     || myInvocationData.Queue != recurringJobInvocationData.Queue
                                     || myInvocationData.Type != recurringJobInvocationData.Type
                                     || j.Cron != shiftParameter.Cron
                                     || j.TimeZone != shiftParameter.TimeZone;
                                    
                    return jobChanged;
                }
            );
        return parentJobDeletedOrChanged;
    }
}

internal static class DateExtensions
{
    public static DateTimeOffset CropSeconds(this DateTimeOffset date) => 
        new(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0, date.Offset);
}