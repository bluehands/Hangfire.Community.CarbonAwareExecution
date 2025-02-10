using FunicularSwitch.Generators;
using Hangfire.Common;
using Hangfire.Storage;

namespace Hangfire.Community.CarbonAwareExecution;

[UnionType]
internal abstract partial record GetJobResult
{
    public record Job_(InvocationData InvocationData, string? Cron, string? TimeZone) : GetJobResult;
    public record Error_(string? Cron, string? TimeZone) : GetJobResult;
    public record NotFound_ : GetJobResult;
}

internal static class JobExtensions
{
    public static T? GetArgument<T>(this Job job) => job.Args.OfType<T>().FirstOrDefault();
    public static GetJobResult GetRecurringJob(this IStorageConnection connection, string recurringJobId)
    {
        var recurringJobEntry = connection.GetAllEntriesFromHash($"recurring-job:{recurringJobId}");
        if (recurringJobEntry == null || !recurringJobEntry.TryGetValue("Job", out var payload) || string.IsNullOrWhiteSpace(payload))
            return GetJobResult.NotFound();

        var cron = recurringJobEntry.GetValueOrDefault("Cron");
        var timeZoneId = recurringJobEntry.GetValueOrDefault("TimeZoneId");

        try
        {
            var invocationData = InvocationData.DeserializePayload(payload);
            return GetJobResult.Job(invocationData, cron, timeZoneId);
        }
        catch (Exception)
        {
            return GetJobResult.Error(cron, timeZoneId);
        }
    }
}