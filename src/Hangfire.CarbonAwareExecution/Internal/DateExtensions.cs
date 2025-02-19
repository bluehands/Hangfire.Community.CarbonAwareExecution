namespace Hangfire.Community.CarbonAwareExecution.Internal;

internal static class DateExtensions
{
    public static DateTimeOffset CropSeconds(this DateTimeOffset date) =>
        new(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0, date.Offset);
}