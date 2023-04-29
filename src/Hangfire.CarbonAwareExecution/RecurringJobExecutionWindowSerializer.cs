using Hangfire;

namespace CarbonAwareComputing.Hangfire;

internal class RecurringJobExecutionWindowSerializer
{
    private const string JobIdDescriptor = " with carbon aware window of ";
    private static CarbonAwareForecastParameter zero = new CarbonAwareForecastParameter(string.Empty, TimeSpan.Zero, TimeSpan.Zero);
    public static string SerializeInJobId(string recurringJobId, TimeSpan maxExecutionDelay, TimeSpan estimatedJobDuration)
    {
        return $"{recurringJobId}{JobIdDescriptor}'{estimatedJobDuration} in {maxExecutionDelay}'";
    }
    public static bool TryDeserializeFromJob(BackgroundJob recurringJob, out CarbonAwareForecastParameter forecastParameter)
    {
        forecastParameter = zero;
        try
        {
            var recurringJobKv = recurringJob.ParametersSnapshot.FirstOrDefault(k => k.Key.Contains("RecurringJobId"));
            if (string.IsNullOrEmpty(recurringJobKv.Key))
            {
                return false;
            }
            var recurringJobId = recurringJobKv.Value;
            recurringJobId = recurringJobId.Trim('\"');
            var pos = recurringJobId.IndexOf(JobIdDescriptor, StringComparison.CurrentCultureIgnoreCase);
            if (pos == -1)
            {
                return false;
            }
            var posLast = recurringJobId.LastIndexOf("'", StringComparison.InvariantCultureIgnoreCase);
            if (posLast == -1)
            {
                return false;
            }
            var jobId = recurringJobId.Substring(0, pos);
            var start = pos + JobIdDescriptor.Length + 1;
            var len = posLast - start;
            var sub = recurringJobId.Substring(start, len);
            var parts = sub.Split(' ');
            var estimatedJobDurationString = parts.FirstOrDefault();
            var maxExecutionDelayString = parts.LastOrDefault();

            if (!TimeSpan.TryParse(maxExecutionDelayString, out var maxExecutionDelay))
            {
                return false;
            }
            if (!TimeSpan.TryParse(estimatedJobDurationString, out var estimatedJobDuration))
            {
                return false;
            }

            forecastParameter = new CarbonAwareForecastParameter(jobId, maxExecutionDelay, estimatedJobDuration);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}

internal record CarbonAwareForecastParameter(string JobId, TimeSpan MaxExecutionDelay, TimeSpan EstimatedJobDuration);