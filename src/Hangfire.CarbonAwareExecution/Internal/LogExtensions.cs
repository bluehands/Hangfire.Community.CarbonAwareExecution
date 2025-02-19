using Microsoft.Extensions.Logging;

namespace Hangfire.Community.CarbonAwareExecution.Internal;

internal static class LogExtensions
{
    public static void LogDebug(this ILogger? logger, Func<string> message)
    {
        if (logger?.IsEnabled(LogLevel.Debug) ?? false)
        {
            logger.LogDebug(message());
        }
    }
}