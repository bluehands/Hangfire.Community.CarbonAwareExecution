using FunicularSwitch.Generators;
using Hangfire.Common;
using Hangfire.Storage;

namespace Hangfire.Community.CarbonAwareExecution;

[UnionType]
internal abstract partial record GetJobResult
{
    public record Job_(Job HangfireJob) : GetJobResult;
    public record Error_() : GetJobResult;
    public record NotFound_() : GetJobResult;
}

internal static class JobExtensions
{
    public static CarbonAwareDelayParameter? GetCarbonAwareDelayParameter(this Job job) => job.Args.OfType<CarbonAwareDelayParameter>().FirstOrDefault();
    public static string? GetRecurringJobId(this BackgroundJob backgroundJob) => backgroundJob.ParametersSnapshot.TryGetValue("RecurringJobId", out var recurringJobId) ? recurringJobId.Trim('\"') : null;
    public static GetJobResult TryGetRecurringJob(this IStorageConnection connection, string recurringJobId)
    {
        var parentJobEntry = connection.GetAllEntriesFromHash($"recurring-job:{recurringJobId}");
        if (parentJobEntry == null || !parentJobEntry.TryGetValue("Job", out var payload) || string.IsNullOrWhiteSpace(payload))
            return GetJobResult.NotFound();

        try
        {
            var invocationData = InvocationData.DeserializePayload(payload);
            var job = invocationData.DeserializeJob();
            return GetJobResult.Job(job);
        }
        catch (Exception)
        {
            return GetJobResult.Error();
        }
    }
}